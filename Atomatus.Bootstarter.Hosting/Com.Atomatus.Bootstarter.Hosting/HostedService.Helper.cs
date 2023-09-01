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
	internal sealed class HostedServiceHelper
	{
        private readonly IEnumerable<IHostedServiceCallback> callbacks;
        private readonly IServiceScopeFactory serviceScopeFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="HostedServiceHelper"/> class with the provided callbacks and service scope factory.
        /// </summary>
        /// <param name="callbacks">A collection of general <see cref="IHostedServiceCallback"/> instances.</param>
        /// <param name="serviceScopeFactory">The service scope factory used to create scoped service scopes.</param>
        public HostedServiceHelper(
            [MaybeNull] IEnumerable<IHostedServiceCallback>? callbacks,
            [NotNull] IServiceScopeFactory serviceScopeFactory)
        {
            this.callbacks = callbacks ?? Enumerable.Empty<IHostedServiceCallback>();
            this.serviceScopeFactory = serviceScopeFactory ?? throw new ArgumentNullException(nameof(callbacks));
        }

        /// <summary>
        /// Invokes both general and scoped callbacks asynchronously.
        /// </summary>
        /// <param name="stoppingToken">A cancellation token that signals when the service is requested to stop.</param>
        /// <returns>A task representing the asynchronous execution of callbacks.</returns>
        internal Task OnCallbacksAsync<TScopedCallback>(CancellationToken stoppingToken)
            where TScopedCallback : IHostedServiceScopedCallback
        {
            return Task.WhenAll(
                OnHostedServiceCallbacks(stoppingToken),
                OnHostedServiceScopedCallbacks<TScopedCallback>(stoppingToken));
        }

        private async Task OnHostedServiceCallbacks(CancellationToken stoppingToken)
        {
            foreach (var callback in callbacks)
            {
                if (!stoppingToken.IsCancellationRequested)
                {
                    await callback.InvokeAsync(stoppingToken);
                }
            }
        }

        private async Task OnHostedServiceScopedCallbacks<TScopedCallback>(CancellationToken stoppingToken)
            where TScopedCallback : IHostedServiceScopedCallback
        {
            try
            {
                using var scope = serviceScopeFactory.CreateAsyncScope();
                var scopedCallbacks = scope.ServiceProvider.GetService<IEnumerable<TScopedCallback>>();
                if (scopedCallbacks != null)
                {
                    foreach (var callback in scopedCallbacks)
                    {
                        if (!stoppingToken.IsCancellationRequested)
                        {
                            await callback.InvokeAsync(stoppingToken);
                        }
                    }
                }
            }
            catch (ObjectDisposedException) { }
        }
    }
}
