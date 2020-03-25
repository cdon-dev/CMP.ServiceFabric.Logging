using Serilog;
using System;
using CMP.ServiceFabric.Logging;
using Microsoft.ApplicationInsights.Extensibility;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ConsoleSample
{
    class Program
    {
        static void Main(string[] args)
        {
            var instrumentationKey = args.First();

            var serviceProvider = new ServiceCollection()
              .AddApplicationInsightsTelemetryWorkerService(instrumentationKey)
              .AddApplicationInsightsTelemetryProcessor<SuccessfulDependencyFilter>()
              .BuildServiceProvider();

            var telemetryConfiguration = serviceProvider.GetRequiredService<TelemetryConfiguration>();

            var seriLogger = new LoggerConfiguration()
                .DefaultCmp(telemetryConfiguration, "development")
                .CreateLogger();
            Log.Logger = seriLogger;

            var coreLogger = telemetryConfiguration.ConfigureLogging(Log.Logger, "ConsoleSample");
            //NOTE in SF: coreLogger = context.ConfigureLogging(telemetryConfiguration, Log.Logger, "ConsoleSample");

            Console.WriteLine("Hello World!");
            coreLogger.LogInformation("Hello world");
        }
    }
}
