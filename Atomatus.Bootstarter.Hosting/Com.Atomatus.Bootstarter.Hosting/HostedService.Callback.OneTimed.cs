namespace Com.Atomatus.Bootstarter.Hosting
{
    /// <summary>
    /// <para>
    /// This contract class is used to
    /// Timed Hosted Service added as
    /// <see cref="OneTimedHostedServiceExtensions.AddOneTimedHostedService(Microsoft.Extensions.DependencyInjection.IServiceCollection, System.TimeSpan)" />
    /// </para>
    /// </summary>
    public interface IOneTimedHostedServiceCallback : IHostedServiceCallback
    {

    }

    /// <summary>
    /// <inheritdoc cref="IOneTimedHostedServiceCallback"/>
    /// <para>
    /// <i>
    /// This interface implementation accept constructor injection of isolated scoped services
    /// to be executed in HostedService implementions.
    /// </i>
    /// </para>
    /// </summary>
    public interface IOneTimedHostedServiceScopedCallback : IHostedServiceScopedCallback
    {

    }
}
