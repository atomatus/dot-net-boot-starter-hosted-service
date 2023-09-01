using System;
using Microsoft.Extensions.Hosting;

namespace Com.Atomatus.Bootstarter.Hosting
{
    /// <summary>
    /// Hosted Service factory base class.
    /// </summary>
    /// <typeparam name="T">hosted service type</typeparam>
    internal abstract class HostedServiceFactory<T>
        where T : IHostedService
    {
        /// <summary>
        /// Create a hosted service from current factory context rules.
        /// </summary>
        /// <param name="provider">application service provider in engine context</param>
        /// <returns>created hosted service by factory</returns>
        public abstract T Create(IServiceProvider provider);
    }

    /// <summary>
    /// Generic HostedService Factory.
    /// </summary>
    /// <typeparam name="THostedService">hosted service type</typeparam>
    internal sealed class GenericHostedServiceFactory<THostedService> : HostedServiceFactory<THostedService>
        where THostedService : HostedService
    {
        private readonly object[] args;

        /// <summary>
        /// Create instance of factory.
        /// </summary>
        /// <param name="args">hosted service constructor arguments</param>
        public GenericHostedServiceFactory(params object[] args)
        {
            this.args = args;
        }

        /// <inheritdoc />
        /// <exception cref="InvalidOperationException">
        /// throws when is not possible create an instance
        /// </exception>
        public override THostedService Create(IServiceProvider provider)
        {
            return (THostedService)(Activator.CreateInstance(
                typeof(THostedService), args) ??
                throw new InvalidOperationException("Was not possible to create " +
                $"instance of {typeof(THostedService).Name}!"));
        }
    }
}
