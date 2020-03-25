using System;
using System.Fabric;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.ApplicationInsights.ServiceFabric;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using Serilog.Exceptions;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace CMP.ServiceFabric.Logging
{
    public static class Extensions
    {
        public static LoggerConfiguration DefaultCmp(
            this LoggerConfiguration loggerConfiguration,
            TelemetryConfiguration telemetryConfiguration,
            string env)
        {
            var level = string.Equals(env, EnvironmentName.Development, StringComparison.OrdinalIgnoreCase)
                ? LogEventLevel.Debug
                : LogEventLevel.Information;

            return loggerConfiguration
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .MinimumLevel.Override("System", LogEventLevel.Warning)
                //.WriteTo.ColoredConsole().MinimumLevel.Is(level)
                .WriteTo.ApplicationInsights(telemetryConfiguration, TelemetryConverter.Traces).MinimumLevel.Is(level)
                .Enrich.FromLogContext()
                .Enrich.WithExceptionDetails();
        }

        public static ILogger ConfigureLogging(
            this ServiceContext context,
            TelemetryConfiguration telemetryConfiguration,
            Serilog.ILogger serilogLogger,
            string logCategoryName)
            => telemetryConfiguration.ConfigureLogging(
                serilogLogger,
                config => config.TelemetryInitializers.Add(
                    FabricTelemetryInitializerExtension.CreateFabricTelemetryInitializer(context)),
                logCategoryName);

        public static ILogger ConfigureLogging(
            this TelemetryConfiguration telemetryConfiguration,
            Serilog.ILogger serilogLogger,
            string logCategoryName)
            => ConfigureLogging(
                telemetryConfiguration,
                serilogLogger,
                config => { },
                logCategoryName);

        public static ILogger ConfigureLogging(
            this TelemetryConfiguration telemetryConfiguration,
            Serilog.ILogger serilogLogger,
            Action<TelemetryConfiguration> additionalConfig,
            string logCategoryName)
        {
            if (string.IsNullOrWhiteSpace(telemetryConfiguration.InstrumentationKey))
                throw new ArgumentNullException(nameof(telemetryConfiguration), "InstrumentationKey required");

            additionalConfig(telemetryConfiguration);

            var logger = new LoggerFactory()
                .AddSerilog(serilogLogger)
                .CreateLogger(logCategoryName);

            return logger;
        }
    }
}
