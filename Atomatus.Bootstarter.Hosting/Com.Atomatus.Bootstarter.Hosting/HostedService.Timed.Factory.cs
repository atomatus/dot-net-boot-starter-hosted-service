using System;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;

namespace Com.Atomatus.Bootstarter.Hosting
{
    /// <summary>
    /// <see cref="TimedHostedService"/> Factory.
    /// </summary>
	internal sealed class TimedHostedServiceFactory : HostedServiceFactory<TimedHostedService>
    {
        private readonly TimeSpan dueTime;
        private readonly TimeSpan period;

        /// <summary>
        /// Construct a Timed Hosted Service Factory.
        /// </summary>
        /// <param name="dueTime">The initial delay before starting the first execution.</param>
        /// <param name="period">The interval between successive executions.</param>
        public TimedHostedServiceFactory(TimeSpan dueTime, TimeSpan period)
		{
            this.dueTime = dueTime;
            this.period = period;
        }

        public override TimedHostedService Create(IServiceProvider provider)
        {
            return new DefaultTimedHostedService(
                provider.GetService<IEnumerable<ITimedHostedServiceCallback>>(),
                provider.GetRequiredService<IServiceScopeFactory>(),
                this.dueTime,
                this.period);
        }
    }
}

