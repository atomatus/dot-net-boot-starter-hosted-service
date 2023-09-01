using System.Threading;
using System.Threading.Tasks;

namespace Com.Atomatus.Bootstarter.Hosting
{
    /// <summary>
    /// <para>
    /// This contract class is used to
    /// execute specific actions at <see cref="HostedService"/> derived classes,
    /// as like as, <see cref="OneTimedHostedService"/> for one time only execution,
    /// or <see cref="TimedHostedService"/> to be executed at regular time interval.
    /// </para>
    /// <para>
    /// The <see cref="OneTimedHostedService"/> and <see cref="TimedHostedService"/>
    /// would loop through the collection of registered callback services
    /// and invoke their respective Invoke methods.
    /// </para>
    /// </summary>
    public interface IHostedServiceCallback : ICallback  { }

    /// <summary>
    /// <inheritdoc cref="IHostedServiceCallback"/>
    /// <para>
    /// <i>
    /// This interface implementation accept constructor injection of isolated scoped services
    /// to be executed in HostedService implementions.
    /// </i>
    /// </para>
    /// </summary>
    public interface IHostedServiceScopedCallback : ICallback { }

    /// <summary>
    /// Base callback implementation.
    /// </summary>
    public interface ICallback
    {
        /// <summary>
        /// This method will be invoked at HostedService implementation.
        /// </summary>
        /// <param name="stoppingToken">A cancellation token that signals when the service is requested to stop.</param>
        /// <returns>execution action task</returns>
        Task InvokeAsync(CancellationToken stoppingToken);
    }
}
