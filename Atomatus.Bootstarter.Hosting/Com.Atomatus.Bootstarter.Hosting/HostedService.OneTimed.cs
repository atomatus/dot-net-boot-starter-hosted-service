using System;
using System.Threading;
using System.Threading.Tasks;

namespace Com.Atomatus.Bootstarter.Hosting
{
    /// <summary>
    /// <para>
    /// A hosted service that executes (<see cref="OnTimedBackgroundAsync(CancellationToken)"/>)
    /// its logic only once during application startup after timeout.
    /// </para>
    /// <para>
    /// The <c>OneTimedHostedService</c> class is designed to provide a simple way to execute a
    /// set of tasks exactly once during the startup of an <c>ASP.NET</c> Core application.
    /// </para>
    /// <para>
    /// This can be useful for scenarios where you want to perform one-time initialization
    /// tasks or startup routines that should run when the application starts.
    /// </para>
    ///
    /// <list type="number">
    /// <item>
    /// <term>Inheritance and Initialization:</term>
    /// <description>
    /// <para>
    /// You'll need to create a class that inherits from OneTimedHostedService.
    /// In the constructor of your derived class, you define the timeout after
    /// which the one-time execution should occur. For example:
    /// </para>
    /// <code>
    /// public class MyOneTimeService : OneTimedHostedService
    /// {
    /// 
    ///     public MyOneTimeService() : base(TimeSpan.FromSeconds(10)) { }
    /// 
    ///     protected override void OnWork(CancellationToken token)
    ///     {
    ///          // Your one-time background logic here
    ///     }
    /// 
    /// }
    /// </code>
    /// </description>
    /// </item>
    /// </list>
    /// </summary>
    public abstract class OneTimedHostedService : HostedService
    {
        private readonly TimeSpan timeout;

        /// <summary>
        /// Initializes a new instance of the OneTimedHostedService class with the specified timeout.
        /// </summary>
        /// <param name="timeout">The timeout after which the hosted service's logic should be executed.</param>
        /// <exception cref="ArgumentOutOfRangeException">throws when timeout is zero-less or too large (int.MaxValue in millis)</exception>
        public OneTimedHostedService(TimeSpan timeout)
        {
            this.timeout = timeout.ThrowsIfZeroLessOrTooLarge(nameof(timeout));
        }

        /// <summary>
        /// The method that provides the logic to be executed once during application startup.
        /// </summary>
        /// <param name="stoppingToken">A cancellation token that signals when the service is requested to stop.</param>
        /// <returns>A Task representing the execution of the logic.</returns>
        protected sealed override async Task OnBackgroundAsync(CancellationToken stoppingToken)
        {
            // Wait for the specified timeout or until the stopping token triggers
            await Task.WhenAny(Task.Delay(timeout, stoppingToken));

            if(!stoppingToken.IsCancellationRequested)
            {
                await this.OnTimedBackgroundAsync(stoppingToken);
            }
        }

        /// <summary>
        /// The method that provides the logic to be executed after timeout one timed only
        /// during application startup.
        /// </summary>
        /// <param name="stoppingToken">A cancellation token that signals when the service is requested to stop.</param>
        /// <returns>A Task representing the execution of the logic.</returns>
        protected abstract Task OnTimedBackgroundAsync(CancellationToken stoppingToken);
    }

}
