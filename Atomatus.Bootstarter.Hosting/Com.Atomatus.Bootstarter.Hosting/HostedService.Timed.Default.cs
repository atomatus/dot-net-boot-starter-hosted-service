using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;

namespace Com.Atomatus.Bootstarter.Hosting
{
    /// <summary>
    /// Default implementation for <see cref="TimedHostedService"/> to
    /// make usage of callbacks (<see cref="ITimedHostedServiceCallback"/> or
    /// <see cref="ITimedHostedServiceScopedCallback"/>) in each period of time
    /// reapting it.
    /// </summary>
    internal sealed class DefaultTimedHostedService : TimedHostedService, IHostedServiceDelayed
    {
        private readonly HostedServiceHelper helper;
        private readonly SemaphoreSlim semaphore;
        private DateTime lastInvokeTime;

        /// <inheritdoc />
        public DefaultTimedHostedService(
            [MaybeNull] IEnumerable<ITimedHostedServiceCallback>? callbacks,
            [NotNull] IServiceProvider serviceProvider,
            TimeSpan dueTime,
            TimeSpan period) : base(dueTime, period)
        {
            this.helper = new HostedServiceHelper(callbacks, serviceProvider);
            this.semaphore = new SemaphoreSlim(1);
            this.lastInvokeTime = DateTime.UtcNow;
        }

        /// <inheritdoc />
        protected override async void OnWork(CancellationToken token)
        {
            try
            {
                await this.semaphore.WaitAsync(token);
                await this.helper.InvokeCallbacksAsync<ITimedHostedServiceScopedCallback>(this, token);
                this.lastInvokeTime = this.lastInvokeTime.AddTicks(
                    DateTime.UtcNow.Ticks - this.lastInvokeTime.Ticks);
            }
            finally
            {
                this.semaphore.Release();
            }
        }

        /// <inheritdoc />
        DateTime IHostedServiceDelayed.GetLastInvokeUtcTime()
        {
            return lastInvokeTime;
        }

        protected override void OnDispose()
        {
            base.OnDispose();
            this.helper.Dispose();
        }
    }
}
