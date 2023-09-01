using System;
using Microsoft.Extensions.DependencyInjection;

namespace Com.Atomatus.Bootstarter.Hosting
{
    /// <summary>
    /// <see cref="OneTimedHostedService"/> Extensions.
    /// </summary>
    public static class OneTimedHostedServiceExtensions
    {
        /// <summary>
        /// <para>
        /// Adds an OneTimedHostedService to consume <see cref="IOneTimedHostedServiceCallback"/>
        /// added as <c>Singleton</c> or <c>Transient</c> to the services collection.
        /// </para>
        /// <para>
        /// <i>
        ///  Warning: Does not add a callback as Scoped because we are in Engine provider scope,
        ///  and application was not fully started yet. <br/>
        ///  <b>To make usage of callbacks with Scoped services:</b>
        ///  <list type="bullet">
        ///  <item>
        ///  use <see cref="IServiceScopeFactory"/> in your <see cref = "IOneTimedHostedServiceCallback" /> implementation class;
        ///  </item>
        ///  <item>
        ///  [Or] make usage of <see cref="IOneTimedHostedServiceScopedCallback"/> (best pratices).
        ///  </item>
        ///  </list>
        /// </i>
        /// </para>
        /// </summary>
        /// <param name="services">The IServiceCollection to add the service to.</param>
        /// <param name="timeout">The timeout after which the hosted service's logic should be executed.</param>
        /// <returns>The modified IServiceCollection.</returns>
        public static IServiceCollection AddOneTimedHostedService(
            this IServiceCollection services, TimeSpan timeout)
        {
            return services.AddHostedService(
                HostedServiceFactories.OneTimedFactory(timeout).Create);
        }

        /// <summary>
        /// <para>
        /// Adds an OneTimedHostedService to consume
        /// <see cref="OneTimedHostedService.OnTimedBackgroundAsync(System.Threading.CancellationToken)"/>
        /// after <paramref name="timeout"/> one time only.
        /// </para>
        /// </summary>
        /// <param name="services">The IServiceCollection to add the service to.</param>
        /// <param name="timeout">The timeout after which the hosted service's logic should be executed.</param>
        /// <typeparam name="THostedService">target one timed hosted service type.</typeparam>
        /// <returns>The modified IServiceCollection.</returns>
        public static IServiceCollection AddOneTimedHostedService<THostedService>(
            this IServiceCollection services, TimeSpan timeout)
            where THostedService : OneTimedHostedService
        {
            return services.AddHostedService(
                HostedServiceFactories.GenericOneTimedFactory<THostedService>(timeout).Create);
        }
    }
}
