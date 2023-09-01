using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using Microsoft.Extensions.DependencyInjection;

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
        private DateTime lastInvokeTime;

        /// <inheritdoc />
        public DefaultTimedHostedService(
            [MaybeNull] IEnumerable<ITimedHostedServiceCallback>? callbacks,
            [NotNull] IServiceScopeFactory serviceScopeFactory,
            TimeSpan dueTime,
            TimeSpan period) : base(dueTime, period)
        {
            this.helper = new HostedServiceHelper(callbacks, serviceScopeFactory);
            this.lastInvokeTime = DateTime.UtcNow;
        }

        /// <inheritdoc />
        protected override void OnWork(CancellationToken token)
        {
            this.helper.InvokeCallbacksAsync<ITimedHostedServiceScopedCallback>(this, token);
            this.lastInvokeTime = this.lastInvokeTime.AddTicks(DateTime.UtcNow.Ticks);
        }

        /// <inheritdoc />
        DateTime IHostedServiceDelayed.GetLastInvokeUtcTime()
        {
            return lastInvokeTime;
        }
    }
}
