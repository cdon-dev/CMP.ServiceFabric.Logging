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

            Log.Logger = new LoggerConfiguration()
                .DefaultCmp(telemetryConfiguration, "development")
                .CreateLogger();

            var coreLogger = telemetryConfiguration.ConfigureLogging(Log.Logger, "ConsoleSample");
            //NOTE in SF: coreLogger = context.ConfigureLogging(telemetryConfiguration, Log.Logger, "ConsoleSample");

            coreLogger.LogInformation("Hello world");
            Console.WriteLine("Hello World! - press a key to end.");
            Console.ReadLine();
        }
    }
}
