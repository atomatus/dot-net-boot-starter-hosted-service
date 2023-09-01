using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Com.Atomatus.Bootstarter.Hosting
{
    /// <summary>
    /// <para>
    /// The <c>TimedHostedService</c> class is an abstract base class that extends the
    /// <c>HostedService</c> class, providing a structured pattern for implementing timed
    /// background execution tasks in an <c>ASP.NET</c> Core application.
    /// </para>
    /// <para>
    /// It's designed to simplify the creation of services that need to perform
    /// tasks at regular intervals.
    /// </para>
    /// </summary>
    public abstract class TimedHostedService : HostedService
    {
        private Timer? timer;
        private readonly TimeSpan dueTime, period;

        /// <summary>
        /// Initializes a new instance of the TimedHostedService class with the specified due time and period.
        /// </summary>
        /// <param name="dueTime">The initial delay before starting the first execution.</param>
        /// <param name="period">The interval between successive executions.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Throws when <paramref name="dueTime"/> is a negative or too large value;
        /// Throws when <paramref name="period"/> is a non-positive (non-zero) or too large value;
        /// </exception>
        public TimedHostedService(TimeSpan dueTime, TimeSpan period)
        {
            this.dueTime = dueTime.ThrowsIfNegativeOrTooLarge(nameof(period));
            this.period = period.ThrowsIfZeroLessOrTooLarge(nameof(period));
        }

        /// <summary>
        /// Invoked when the application is starting up. Initializes the timer for timed background execution.
        /// </summary>
        /// <param name="stoppingToken">A cancellation token that signals when the service is requested to stop.</param>
        /// <returns>A Task representing the start-up process.</returns>
        protected sealed override Task OnBackgroundAsync(CancellationToken stoppingToken)
        {
            // Dispose of any existing timer and create a new one
            this.timer?.Dispose();
            this.timer = new Timer(OnWorkInternal, stoppingToken, dueTime, period);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Invoked when the application is shutting down. Stops the timer and performs cleanup.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token that signals when the service is requested to stop.</param>
        /// <returns>A Task representing the stopping process.</returns>
        public override Task StopAsync(CancellationToken cancellationToken)
        {
            // Stop the timer
            this.timer?.Change(Timeout.Infinite, Timeout.Infinite);
            return base.StopAsync(cancellationToken);
        }

        /// <summary>
        /// Invoked when the service is being disposed. Disposes of the timer and performs cleanup.
        /// </summary>
        protected override void OnDispose()
        {
            this.timer?.Dispose();
            this.timer = null;
        }

        /// <summary>
        /// Internal callback method for the timer's execution. Calls the OnWork method with the provided CancellationToken.
        /// </summary>
        /// <param name="state">The state object provided by the timer, expected to be a CancellationToken.</param>
        private void OnWorkInternal(object? state)
        {
            Debug.Assert(state is CancellationToken, "State must be a Cancellation Token!");
            this.OnWork((CancellationToken)state);
        }

        /// <summary>
        /// The method that contains the timed background logic to be executed.
        /// </summary>
        /// <param name="token">A cancellation token that signals when the service is requested to stop.</param>
        protected abstract void OnWork(CancellationToken token);
    }
}
