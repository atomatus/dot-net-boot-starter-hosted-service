using System;
using Microsoft.Extensions.DependencyInjection;

namespace Com.Atomatus.Bootstarter.Hosting
{
    /// <summary>
    /// <see cref="TimedHostedService"/> Extensions.
    /// </summary>
    public static class TimedHostedServiceExtensions
    {
        /// <summary>
        /// <para>
        /// Adds a TimedHostedService to consume <see cref="ITimedHostedServiceCallback"/>
        /// added as <c>Singleton</c> or <c>Transient</c> to the services collection,
        /// after a <paramref name="period"/>, starting in <paramref name="dueTime"/>
        /// until application ends.
        /// </para>
        /// <para>
        /// <i>
        ///  Warning: Does not add a callback as Scoped because we are in Engine provider scope,
        ///  and application was not fully started yet. <br/>
        ///  <b>To make usage of callbacks with Scoped services:</b>
        ///  <list type="bullet">
        ///  <item>
        ///  use <see cref="IServiceScopeFactory"/> in your <see cref = "ITimedHostedServiceCallback" /> implementation class;
        ///  </item>
        ///  <item>
        ///  [Or] make usage of <see cref="ITimedHostedServiceScopedCallback"/> (best pratices).
        ///  </item>
        ///  </list>
        /// </i>
        /// </para>
        /// </summary>
        /// <param name="services">The IServiceCollection to add the service to.</param>
        /// <param name="dueTime">The initial delay before starting the first execution.</param>
        /// <param name="period">The interval between successive executions.</param>
        /// <returns>The modified IServiceCollection.</returns>
        public static IServiceCollection AddTimedHostedService(
            this IServiceCollection services, TimeSpan dueTime, TimeSpan period)
        {
            return services.AddHostedService(
                HostedServiceFactories.TimedFactory(dueTime, period).Create);
        }

        /// <summary>
        /// <para>
        /// Adds a TimedHostedService to consume
        /// <see cref="TimedHostedService.OnWork(System.Threading.CancellationToken)"/>
        /// after each <paramref name="period"/> of time starting after <paramref name="dueTime"/>
        /// until application ends.
        /// </para>
        /// </summary>
        /// <param name="services">The IServiceCollection to add the service to.</param>
        /// <param name="dueTime">The initial delay before starting the first execution.</param>
        /// <param name="period">The interval between successive executions.</param>
        /// <typeparam name="THostedService">target timed hosted service type.</typeparam>
        /// <returns>The modified IServiceCollection.</returns>
        public static IServiceCollection AddTimedHostedService<THostedService>(
            this IServiceCollection services, TimeSpan dueTime, TimeSpan period)
            where THostedService : TimedHostedService
        {
            return services.AddHostedService(
                HostedServiceFactories.GenericTimedFactory<THostedService>(dueTime, period).Create);
        }
    }
}
