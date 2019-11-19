using Microsoft.ApplicationInsights.DependencyCollector;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.ApplicationInsights.ServiceFabric;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using Serilog.Exceptions;
using System;
using System.Fabric;

namespace CMP.Logging
{
    public static class Extensions
    {
        public static LoggerConfiguration DefaultCmp(
            this LoggerConfiguration loggerConfiguration,
            TelemetryConfiguration telemetryConfiguration,
            string env)
        {
            var level = env == EnvironmentName.Production ? LogEventLevel.Information : LogEventLevel.Debug;
            return loggerConfiguration
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .MinimumLevel.Override("System", LogEventLevel.Warning)
                //.WriteTo.ColoredConsole().MinimumLevel.Is(level)
                .WriteTo.ApplicationInsights(telemetryConfiguration, TelemetryConverter.Traces).MinimumLevel.Is(level)
                .Enrich.FromLogContext()
                .Enrich.WithExceptionDetails();
        }

        public static Microsoft.Extensions.Logging.ILogger ConfigureLogging(
        this ServiceContext context,
        TelemetryConfiguration telemetryConfiguration,
        string environment,
        bool dependencyLoggingEnabled = true)
            => telemetryConfiguration.ConfigureLogging(environment, config => config.TelemetryInitializers.Add(
                FabricTelemetryInitializerExtension.CreateFabricTelemetryInitializer(context)), dependencyLoggingEnabled);

        public static Microsoft.Extensions.Logging.ILogger ConfigureLogging(
            this TelemetryConfiguration telemetryConfiguration,
            string environment,
            bool dependencyLoggingEnabled = true)
                => ConfigureLogging(telemetryConfiguration, environment, config => { }, dependencyLoggingEnabled);

        public static Microsoft.Extensions.Logging.ILogger ConfigureLogging(
            this TelemetryConfiguration telemetryConfiguration,
            string environment,
            Action<TelemetryConfiguration> additionalConfig,
            bool dependencyLoggingEnabled = true)
        {
            if (string.IsNullOrWhiteSpace(telemetryConfiguration.InstrumentationKey))
                throw new ArgumentNullException("InstrumentationKey required");

            if (dependencyLoggingEnabled)
            {
                var module = new DependencyTrackingTelemetryModule();
                module.IncludeDiagnosticSourceActivities.Add("Microsoft.Azure.ServiceBus");
                module.IncludeDiagnosticSourceActivities.Add("Microsoft.Azure.EventHubs");

                module.Initialize(telemetryConfiguration);
            }

            additionalConfig(telemetryConfiguration);

            var seriLogger = new LoggerConfiguration()
                .DefaultCmp(telemetryConfiguration, environment)
                .CreateLogger();

            var logger = new LoggerFactory()
                .AddSerilog(seriLogger)
                .CreateLogger("CMP.Logging"); //TODO fix

            Log.Logger = seriLogger;

            return logger;
        }
    }
}