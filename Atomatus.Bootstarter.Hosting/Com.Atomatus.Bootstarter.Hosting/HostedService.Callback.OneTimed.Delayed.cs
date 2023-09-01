using System;
using System.Threading;
using System.Threading.Tasks;

namespace Com.Atomatus.Bootstarter.Hosting
{
    /// <summary>
    /// Represents an abstract base class for
    /// implementing a delayed timer callback service
    /// for Timed Hosted Service added in <see cref="OneTimedHostedServiceExtensions.AddOneTimedHostedService(
    /// Microsoft.Extensions.DependencyInjection.IServiceCollection, TimeSpan)"/>.
    /// </summary>
    public abstract class DelayedOneTimedHostedServiceCallback : IOneTimedHostedServiceCallback, IOneTimedHostedServiceScopedCallback
    {
        private readonly TimeSpan delayInterval;
        private DateTime lastInvokeTime;

        /// <summary>
        /// Initializes a new instance of the <see cref="DelayedOneTimedHostedServiceCallback"/> class.
        /// </summary>
        /// <param name="delayInterval">The delay interval before allowing the invocation.</param>
        protected DelayedOneTimedHostedServiceCallback(TimeSpan delayInterval)
        {
            this.delayInterval = delayInterval;
            this.lastInvokeTime = DateTime.MinValue;
        }

        /// <summary>
        /// Invokes the callback method if the
        /// specified delay interval has passed.
        /// </summary>
        /// <param name="stoppingToken">A cancellation token that signals when the service is requested to stop.</param>
        /// <returns>execution action task</returns>
        Task ICallback.InvokeAsync(CancellationToken stoppingToken)
        {
            DateTime currTime = DateTime.UtcNow;
            TimeSpan timeSinceLastInvoke = currTime - lastInvokeTime;

            if (timeSinceLastInvoke < delayInterval)
            {
                //The target is OneTimedHostedService,
                // then this will not more fired again.
                // Schedule in remaing timeout.
                TimeSpan remainingTime = delayInterval - timeSinceLastInvoke;

                Timer? timer = null;
                timer = new Timer(_ =>
                {
                    timer?.Dispose();
                    this.InvokeAsync(stoppingToken);
                }, null, remainingTime, Timeout.InfiniteTimeSpan);

                return Task.CompletedTask;
            }

            this.lastInvokeTime = currTime;
            return this.InvokeAsync(stoppingToken);
        }

        /// <summary>
        /// The method to be implemented by derived classes for
        /// performing the callback action after <see cref="delayInterval"/>.
        /// </summary>
        /// <param name="stoppingToken">A cancellation token that signals when the service is requested to stop.</param>
        /// <returns>execution action task</returns>
        protected abstract Task InvokeAsync(CancellationToken stoppingToken);
    }
}

