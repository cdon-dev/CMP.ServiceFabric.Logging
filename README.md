[![NuGet Version and Downloads count](https://buildstats.info/nuget/CMP.ServiceFabric.Logging?includePreReleases=true)](https://www.nuget.org/packages/CMP.ServiceFabric.Logging/)

# CMP.Logging
Opinionated logging setup for .NET Core and Service Fabric.

### Service Fabric Context

    var logger = context.ConfigureLogging(telemetryConfiguration, environment);

### Telemetry Client

    var telemetryConfig = TelemetryConfiguration.CreateDefault();
    telemetryConfig.InstrumentationKey = config["ApplicationInsights:InstrumentationKey"];
    var logger = telemetryConfig.ConfigureLogging(environment, dependencyLoggingEnabled: true);

All helper currently setup SeriLog global logger. If you just want a SeriLogger, use the following.

### Serilog only

  
    Log.Logger = new LoggerConfiguration()
        .DefaultCmp(telemetryConfig, environment)
        .CreateLogger();

