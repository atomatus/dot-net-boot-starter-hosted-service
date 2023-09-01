using System;

namespace Com.Atomatus.Bootstarter.Hosting
{
    /// <summary>
    /// Setup delayed timeout last execution and identify that
    /// callback is delayed type.
    /// </summary>
    public interface IHostedServiceDelayedCallback : ICallback
    {
        /// <summary>
        /// Update last invoke time by <see cref="IHostedServiceDelayed"/>.
        /// </summary>
        /// <param name="lastInvokeTime">last invoke UTC time</param>
        void SetLastInvokeUtcTime(DateTime lastInvokeTime);
    }

    /// <summary>
    /// Recover last service execution time to be consumed
    /// in <see cref="IHostedServiceDelayedCallback"/>.
    /// </summary>
    public interface IHostedServiceDelayed
    {
        /// <summary>
        /// Get last invoke time in HostedService.
        /// </summary>
        /// <returns>last invoke UTC time</returns>
        DateTime GetLastInvokeUtcTime();
    }
}

