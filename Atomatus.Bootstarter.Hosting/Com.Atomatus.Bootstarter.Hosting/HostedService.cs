using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace Com.Atomatus.Bootstarter.Hosting
{
    /// <summary>
    /// An abstract base class that simplifies the implementation of IHostedService by providing a common execution pattern.
    /// </summary>
    public abstract class HostedService : IHostedService, IDisposable
    {
        private Task? executingTask;
        private readonly CancellationTokenSource cancellation;

        /// <summary>
        /// Initializes a new instance of the HostedService class.
        /// </summary>
        public HostedService()
        {
            this.cancellation = new CancellationTokenSource();
        }

        /// <summary>
        /// The method that should be overridden to provide the background execution logic.
        /// </summary>
        /// <param name="stoppingToken">A cancellation token that signals when the service is requested to stop.</param>
        /// <returns>A Task representing the execution of the background logic.</returns>
        protected abstract Task OnBackgroundAsync(CancellationToken stoppingToken);

        /// <summary>
        /// Called when the application is starting up. Initiates the background execution.
        /// </summary>
        /// <param name="_">A cancellation token for this method, which is not used in this implementation.</param>
        /// <returns>A Task representing the start-up process.</returns>
        public virtual Task StartAsync(CancellationToken _)
        {
            if (!this.cancellation.IsCancellationRequested)
            {
                var aux = this.executingTask = OnBackgroundAsync(this.cancellation.Token);

                // If the executing task is already completed, return it. Otherwise, return a completed task.
                if (aux.IsCompleted)
                {
                    return aux;
                }
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// Called when the application is shutting down. Stops the background execution gracefully.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token that signals when the service is requested to stop.</param>
        /// <returns>A Task representing the stopping process.</returns>
        public virtual async Task StopAsync(CancellationToken cancellationToken)
        {
            if (this.executingTask != null)
            {
                Task aux = this.executingTask;
                this.executingTask = null;

                try
                {
                    // Request cancellation for OnBackgroundAsync method.
                    this.cancellation.Cancel();
                }
                catch (ObjectDisposedException) { }
                finally
                {
                    // Wait until the task completes or the stop token triggers
                    await Task.WhenAny(aux,
                        Task.Delay(Timeout.Infinite, cancellationToken));
                }
            }
        }

        /// <summary>
        /// Fired when dispose is requested.
        /// </summary>
        protected virtual void OnDispose() { }

        /// <summary>
        /// Disposes of resources used by the class.
        /// </summary>
        public void Dispose()
        {
            this.OnDispose();
            this.cancellation.Cancel();
            this.executingTask = null;
            GC.SuppressFinalize(this);
        }
    }
}
