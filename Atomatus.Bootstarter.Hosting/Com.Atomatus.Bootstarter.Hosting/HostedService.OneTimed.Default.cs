using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Com.Atomatus.Bootstarter.Hosting
{
    /// <summary>
    /// Default implementation for <see cref="OneTimedHostedService"/> to
    /// make usage of callbacks (<see cref="IOneTimedHostedServiceCallback"/> or
    /// <see cref="IOneTimedHostedServiceScopedCallback"/>).
    /// </summary>
    internal sealed class DefaultOneTimedHostedService : OneTimedHostedService, IHostedServiceDelayed
	{
        private readonly HostedServiceHelper helper;
        private readonly DateTime lastInvokeTime;

        /// <summary>
        /// Construct an OneTimedHostedService for <see cref="IOneTimedHostedServiceCallback"/>
        /// collections.
        /// </summary>
        /// <param name="callbacks">callbacks to be executed after timeout</param>
        /// <param name="serviceScopeFactory">service scope factory to be used in attempt of recover some <see cref="IHostedServiceScopedCallback"/></param>
        /// <param name="timeout">The timeout after which the hosted service's logic should be executed.</param>
        public DefaultOneTimedHostedService(
            [MaybeNull] IEnumerable<IOneTimedHostedServiceCallback>? callbacks,
            [NotNull] IServiceScopeFactory serviceScopeFactory,
            TimeSpan timeout) : base(timeout)
        {
            this.helper = new HostedServiceHelper(callbacks, serviceScopeFactory);
            this.lastInvokeTime = DateTime.UtcNow;
        }

        /// <inheritdoc />
        protected override Task OnTimedBackgroundAsync(CancellationToken stoppingToken)
        {
            return this.helper.InvokeCallbacksAsync<IOneTimedHostedServiceScopedCallback>(this, stoppingToken);
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
