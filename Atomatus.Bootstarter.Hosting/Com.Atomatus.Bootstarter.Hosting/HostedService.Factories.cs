using System;

namespace Com.Atomatus.Bootstarter.Hosting
{
    /// <summary>
    /// Factory of factories for HostedService.
    /// </summary>
    internal static class HostedServiceFactories
	{
        /// <summary>
        /// A factory for One Timed Hosted Service.
        /// </summary>
        /// <param name="timeout">hosted service timeout</param>
        /// <returns>one timed hosted service factory</returns>
        public static OneTimedHostedServiceFactory OneTimedFactory(TimeSpan timeout)
		{
			return new OneTimedHostedServiceFactory(timeout);
		}

        /// <summary>
        /// A factory for Timed Hosted Service.
        /// </summary>
        /// <param name="dueTime">The initial delay before starting the first execution.</param>
        /// <param name="period">The interval between successive executions.</param>
        /// <returns>timed hosted service factory</returns>
        public static TimedHostedServiceFactory TimedFactory(TimeSpan dueTime, TimeSpan period)
        {
            return new TimedHostedServiceFactory(dueTime, period);
        }

        /// <summary>
        /// A factory for Generic One Timed Hosted Service.
        /// </summary>
        /// <param name="timeout">hosted service timeout</param>
        /// <returns>one timed hosted service factory</returns>
        public static GenericHostedServiceFactory<T> GenericOneTimedFactory<T>(TimeSpan timeout)
            where T : OneTimedHostedService
        {
            return new GenericHostedServiceFactory<T>(timeout);
        }

        /// <summary>
        /// A factory for Generic Timed Hosted Service.
        /// </summary>
        /// <param name="dueTime">The initial delay before starting the first execution.</param>
        /// <param name="period">The interval between successive executions.</param>
        /// <returns>timed hosted service factory</returns>
        public static GenericHostedServiceFactory<T> GenericTimedFactory<T>(TimeSpan dueTime, TimeSpan period)
            where T : TimedHostedService
        {
            return new GenericHostedServiceFactory<T>(dueTime, period);
        }
	}
}
