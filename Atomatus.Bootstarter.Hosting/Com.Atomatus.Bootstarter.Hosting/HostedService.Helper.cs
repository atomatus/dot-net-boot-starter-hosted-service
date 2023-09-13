using System;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Com.Atomatus.Bootstarter.Hosting
{
    /// <summary>
    /// <para>
    /// The HostedServiceHelper class appears to be a utility class that assists
    /// in managing and executing callbacks for classes derived from <c>HostedService</c>.
    /// </para>
    /// <para>
    /// It encapsulates the logic for invoking both general IHostedServiceCallback
    /// instances and scoped <c>IHostedServiceScopedCallback</c> instances within the
    /// context of a hosted service.
    /// </para>
    /// </summary>
	internal sealed class HostedServiceHelper : IDisposable
	{
        private readonly IEnumerable<IHostedServiceCallback> callbacks;
        private readonly IServiceProvider serviceProvider;
        private readonly IDictionary<int, long> lastInvokeUtcScopedCallbacksCache;

        /// <summary>
        /// Initializes a new instance of the <see cref="HostedServiceHelper"/> class with the provided callbacks and service scope factory.
        /// </summary>
        /// <param name="callbacks">A collection of general <see cref="IHostedServiceCallback"/> instances.</param>
        /// <param name="serviceProvider">The service provider to create, service scope factory used to create scoped service scopes.</param>
        public HostedServiceHelper(
            [MaybeNull] IEnumerable<IHostedServiceCallback>? callbacks,
            [NotNull] IServiceProvider serviceProvider)
        {
            this.callbacks = callbacks ?? Enumerable.Empty<IHostedServiceCallback>();
            this.serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            this.lastInvokeUtcScopedCallbacksCache = new Dictionary<int, long>();
        }

        /// <summary>
        /// Invokes both general and scoped callbacks asynchronously.
        /// </summary>
        /// <param name="delayedOwner">hosted service context who implements <see cref="IHostedServiceDelayed"/> to update last invoke time in callbacks</param>
        /// <param name="stoppingToken">A cancellation token that signals when the service is requested to stop.</param>
        /// <returns>A task representing the asynchronous execution of callbacks.</returns>
        internal Task InvokeCallbacksAsync<TScopedCallback>(
            [NotNull] IHostedServiceDelayed delayedOwner,
            CancellationToken stoppingToken)
            where TScopedCallback : IHostedServiceScopedCallback
        {
            return Task.WhenAll(
                OnHostedServiceCallbacks(delayedOwner, stoppingToken),
                OnHostedServiceScopedCallbacks<TScopedCallback>(delayedOwner, stoppingToken));
        }

        private async Task OnHostedServiceCallbacks(
            [NotNull] IHostedServiceDelayed delayedOwner,
            CancellationToken stoppingToken)
        {
            foreach (var callback in callbacks)
            {
                if (!stoppingToken.IsCancellationRequested)
                {
                    TryGetDelayedCallbackAndSetNotInitializedLastInvokeUtc(
                        callback, delayedOwner.GetLastInvokeUtcTime, out _);
                    await callback.InvokeAsync(stoppingToken);
                }
            }
        }

        private async Task OnHostedServiceScopedCallbacks<TScopedCallback>(
            [NotNull] IHostedServiceDelayed delayedOwner,
            CancellationToken stoppingToken)
            where TScopedCallback : IHostedServiceScopedCallback
        {
            try
            {
                using (var scope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
                {
                    var scopedCallbacks = scope.ServiceProvider.GetService<IEnumerable<TScopedCallback>>();
                    if (scopedCallbacks != null)
                    {
                        foreach (var callback in scopedCallbacks)
                        {
                            try
                            {
                                if (!stoppingToken.IsCancellationRequested)
                                {
                                    if (TryGetDelayedCallbackAndSetNotInitializedLastInvokeUtc(callback,
                                        () => GetLastInvokeUtcScopedCallbacksCacheOrDelayedOwner(callback, delayedOwner),
                                        out IHostedServiceDelayedCallback? delayedCallback))
                                    {
                                        if (await delayedCallback!.InvokeDelayedAsync(stoppingToken))
                                        {
                                            UpdateLastInvokeUtcScopedCallbacksCache(delayedCallback);
                                        }
                                    }
                                    else
                                    {
                                        await callback.InvokeAsync(stoppingToken);
                                    }
                                }
                            }
                            catch (ObjectDisposedException)
                            {
#if DEBUG
                                throw;
#endif
                            }
                        }
                    }
                }
            }
            catch (ObjectDisposedException) { }
        }

        private DateTime GetLastInvokeUtcScopedCallbacksCacheOrDelayedOwner(
            [NotNull] ICallback callback,
            [NotNull] IHostedServiceDelayed delayedOwner)
        {
            int key = callback.GetType().GetHashCode();

            if(lastInvokeUtcScopedCallbacksCache.TryGetValue(key, out long ticks))
            {
                return new DateTime(ticks, DateTimeKind.Utc);
            }
            else
            {
                DateTime dateTime = delayedOwner.GetLastInvokeUtcTime();
                lastInvokeUtcScopedCallbacksCache.Add(key, dateTime.Ticks);
                return dateTime;
            }
        }

        private void UpdateLastInvokeUtcScopedCallbacksCache(ICallback callback)
        {
            int key = callback.GetType().GetHashCode();
            lastInvokeUtcScopedCallbacksCache[key] = DateTime.UtcNow.Ticks;
        }

        private static bool TryGetDelayedCallbackAndSetNotInitializedLastInvokeUtc(
            [NotNull] ICallback callback, Func<DateTime> lastInvokeUtcTimeFun,
            out IHostedServiceDelayedCallback? delayedCallback)
        {
            delayedCallback = null;
            if (callback is IHostedServiceDelayedCallback _delayedCallback)
            {
                if(!_delayedCallback.HasLastInvokeUtcTime())
                {
                    _delayedCallback.SetLastInvokeUtcTime(
                    lastInvokeUtcTimeFun.Invoke());
                }
                
                delayedCallback = _delayedCallback;
            }
            return delayedCallback != null;
        }

        public void Dispose()
        {
            this.lastInvokeUtcScopedCallbacksCache.Clear();
        }
    }
}
