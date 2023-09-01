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
    internal sealed class DefaultTimedHostedService : TimedHostedService
    {
        private readonly HostedServiceHelper helper;

        /// <inheritdoc />
        public DefaultTimedHostedService(
            [MaybeNull] IEnumerable<ITimedHostedServiceCallback>? callbacks,
            [NotNull] IServiceScopeFactory serviceScopeFactory,
            TimeSpan dueTime,
            TimeSpan period)
            : base(dueTime, period)
        {
            this.helper = new HostedServiceHelper(callbacks, serviceScopeFactory);
        }

        /// <inheritdoc />
        protected override void OnWork(CancellationToken token)
        {
            this.helper.OnCallbacksAsync<ITimedHostedServiceScopedCallback>(token);
        }
    }
}

