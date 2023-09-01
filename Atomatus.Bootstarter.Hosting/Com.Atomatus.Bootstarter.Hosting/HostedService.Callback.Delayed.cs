using System;
using System.Threading;
using System.Threading.Tasks;

namespace Com.Atomatus.Bootstarter.Hosting
{
    /// <summary>
    /// Setup delayed timeout last execution and identify that
    /// callback is delayed type.
    /// </summary>
    public interface IHostedServiceDelayedCallback : ICallback
    {
        /// <summary>
        /// Check if current callback delayed already has a last time execution
        /// stored, when never executed before returns false.
        /// </summary>
        /// <returns>true already executed, otherwise false.</returns>
        bool HasLastInvokeUtcTime();

        /// <summary>
        /// Update last invoke time by <see cref="IHostedServiceDelayed"/>.
        /// </summary>
        /// <param name="lastInvokeTime">last invoke UTC time</param>
        void SetLastInvokeUtcTime(DateTime lastInvokeTime);

        /// <summary>
        /// This method will be invoked at HostedService implementation.
        /// </summary>
        /// <param name="stoppingToken">A cancellation token that signals when the service is requested to stop.</param>
        /// <returns>execution action task with result, true delayed was scheduled to run, otherwise false</returns>
        Task<bool> InvokeDelayedAsync(CancellationToken stoppingToken);
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

