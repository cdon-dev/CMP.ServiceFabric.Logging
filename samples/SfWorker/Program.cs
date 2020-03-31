using System;
using System.Diagnostics;
using System.Fabric;
using System.Threading;
using System.Threading.Tasks;
using CMP.ServiceFabric.Logging;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.ServiceFabric.Services.Runtime;
using Serilog;

namespace SfWorker
{
    internal static class Program
    {
        private static void Main()
        {
            var serviceProvider = new ServiceCollection()
                .AddApplicationInsightsTelemetryWorkerService("key")
                .AddApplicationInsightsTelemetryProcessor<SuccessfulDependencyFilter>()
                .BuildServiceProvider();

            var telemetryConfiguration = serviceProvider.GetRequiredService<TelemetryConfiguration>();

            Log.Logger = new LoggerConfiguration()
                .DefaultCmp(telemetryConfiguration, "development")
                .CreateLogger();

            try
            {
                ServiceRuntime.RegisterServiceAsync("SfWorkerType",
                    context => {
                        var coreLogger = context.ConfigureLogging(telemetryConfiguration, Log.Logger, "SfWorkerType");
                        return new SfWorker(context, coreLogger); 
                    }
                    
                    ).GetAwaiter().GetResult();

                ServiceEventSource.Current.ServiceTypeRegistered(Process.GetCurrentProcess().Id, typeof(SfWorker).Name);

                Thread.Sleep(Timeout.Infinite);
            }
            catch (Exception e)
            {
                ServiceEventSource.Current.ServiceHostInitializationFailed(e.ToString());
                throw;
            }
        }
    }
}
