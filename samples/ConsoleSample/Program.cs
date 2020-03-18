using Serilog;
using System;
using CMP.ServiceFabric.Logging;
using Microsoft.ApplicationInsights.Extensibility;
using System.Linq;

namespace ConsoleSample
{
    class Program
    {
        static void Main(string[] args)
        {
            var telemetryConfig = TelemetryConfiguration.CreateDefault();
            telemetryConfig.InstrumentationKey = args.First();

            var serilogLogger = new LoggerConfiguration()
                .DefaultCmp(telemetryConfig, "development")
                .CreateLogger();
            
            Log.Logger = serilogLogger;

            Console.WriteLine("Hello World!");
        }
    }
}
