namespace Com.Atomatus.Bootstarter.Hosting
{
     /// <summary>
     /// <para>
     /// This contract class is used to
     /// execute specific actions at 
     /// Timed Hosted Service added as <see cref="TimedHostedServiceExtensions
     /// .AddTimedHostedService(Microsoft.Extensions.DependencyInjection.IServiceCollection, System.TimeSpan, System.TimeSpan)" />
     /// </para>
     /// </summary>
    public interface ITimedHostedServiceCallback : IHostedServiceCallback
    {

    }

    /// <summary>
    /// <inheritdoc cref="ITimedHostedServiceCallback"/>
    /// <para>
    /// <i>
    /// This interface implementation accept constructor injection of isolated scoped services
    /// to be executed in HostedService implementions.
    /// </i>
    /// </para>
    /// </summary>
    public interface ITimedHostedServiceScopedCallback : IHostedServiceScopedCallback
    {

    }
}
