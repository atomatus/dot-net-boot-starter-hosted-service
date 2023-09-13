using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace Com.Atomatus.Bootstarter.Hosting
{
    /// <summary>
    /// <see cref="OneTimedHostedService"/> Factory.
    /// </summary>
    internal sealed class OneTimedHostedServiceFactory : HostedServiceFactory<OneTimedHostedService>
    {
        private readonly TimeSpan timeout;

        /// <summary>
        /// Construct an OneTimed Hosted Service Factory.
        /// </summary>
        /// <param name="timeout">hosted service timeout</param>
        public OneTimedHostedServiceFactory(TimeSpan timeout)
        {
            this.timeout = timeout;
        }

        /// <summary>
        /// Create an instance of <see cref="DefaultOneTimedHostedService"/>
        /// </summary>
        /// <param name="provider">service provider in engine context</param>
        /// <returns>instance of default one timed hosted service</returns>
        public override OneTimedHostedService Create(IServiceProvider provider)
        {
            return new DefaultOneTimedHostedService(
                provider.GetService<IEnumerable<IOneTimedHostedServiceCallback>>(),
                provider,
                this.timeout);
        }
    }
}
