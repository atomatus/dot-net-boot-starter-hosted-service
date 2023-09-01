
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Com.Atomatus.Bootstarter.Hosting
{
    /// <summary>
    /// Represents an abstract base class for
    /// implementing a delayed timer callback service
    /// for Timed Hosted Service added in <see cref="TimedHostedServiceExtensions.AddTimedHostedService(
    /// Microsoft.Extensions.DependencyInjection.IServiceCollection,
    /// TimeSpan, TimeSpan)"/>.
    /// </summary>
    public abstract class DelayedTimedHostedServiceCallback :
        ITimedHostedServiceCallback,
        ITimedHostedServiceScopedCallback,
        IHostedServiceDelayedCallback
    {
        private readonly TimeSpan delayInterval;
        private DateTime lastInvokeTime;

        /// <summary>
        /// Initializes a new instance of the <see cref="DelayedTimedHostedServiceCallback"/> class.
        /// </summary>
        /// <param name="delayInterval">The delay interval before allowing the invocation.</param>
        protected DelayedTimedHostedServiceCallback(TimeSpan delayInterval)
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
            IHostedServiceDelayedCallback self = this;
            return self.InvokeDelayedAsync(stoppingToken);
        }

        /// <summary>
        /// The method to be implemented by derived classes for
        /// performing the callback action after <see cref="delayInterval"/>.
        /// </summary>
        /// <param name="stoppingToken">A cancellation token that signals when the service is requested to stop.</param>
        /// <returns>execution action task</returns>
        protected abstract Task InvokeAsync(CancellationToken stoppingToken);

        /// <inheritdoc />
        void IHostedServiceDelayedCallback.SetLastInvokeUtcTime(DateTime lastInvokeTime)
        {
            this.lastInvokeTime = lastInvokeTime;
        }

        /// <inheritdoc />
        bool IHostedServiceDelayedCallback.HasLastInvokeUtcTime()
        {
            return this.lastInvokeTime != DateTime.MinValue;
        }

        /// <inheritdoc />
        Task<bool> IHostedServiceDelayedCallback.InvokeDelayedAsync(CancellationToken stoppingToken)
        {
            DateTime currTime = DateTime.UtcNow;
            TimeSpan timeSinceLastInvoke = currTime - lastInvokeTime;

            if (timeSinceLastInvoke < delayInterval)
            {
                //The target is TimedHostedService, this will be fired again later, just ignore it.
                return Task.FromResult(false);
            }

            this.lastInvokeTime = currTime;
            return this.InvokeAsync(stoppingToken)
                .ContinueWith(e => true);
        }
    }
}
