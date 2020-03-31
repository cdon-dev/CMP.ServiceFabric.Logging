using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using Serilog;

namespace SfWorker
{
    internal sealed class SfWorker : StatelessService
    {
        private readonly Microsoft.Extensions.Logging.ILogger logger;

        public SfWorker(StatelessServiceContext context, Microsoft.Extensions.Logging.ILogger logger)
            : base(context)
        {
            this.logger = logger;
        }

        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
            return new ServiceInstanceListener[0];
        }

        protected override async Task RunAsync(CancellationToken cancellationToken)
        {
            long iterations = 0;

            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();
                ++iterations;
                ServiceEventSource.Current.ServiceMessage(this.Context, "Working-{0}", iterations);
                Log.Logger.Information("Serilog Logger Working-{0}", iterations);
                this.logger.LogInformation(".NET Core Logger Working-{0}", iterations);
                await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
            }
        }
    }
}
