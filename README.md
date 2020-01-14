[![NuGet Version and Downloads count](https://buildstats.info/nuget/CMP.ServiceFabric.Logging?includePreReleases=true)](https://www.nuget.org/packages/CMP.ServiceFabric.Logging/)

# CMP.Logging
Opinionated logging setup for .NET Core and Service Fabric.

### Service Fabric Context

```csharp
var serilogLogger = new LoggerConfiguration()
    .DefaultCmp(telemetryConfig, environment)
    .CreateLogger();
Log.Logger = serilogLogger;

var logger = context.ConfigureLogging(telemetryConfig, serilogLogger, "LogName");
```

### Telemetry Client

```csharp
var telemetryConfig = TelemetryConfiguration.CreateDefault();
telemetryConfig.InstrumentationKey = config["ApplicationInsights:InstrumentationKey"];
var logger = telemetryConfig.ConfigureLogging(serilogLogger, "LogName", dependencyLoggingEnabled: true);
```

### Serilog only

```csharp
Log.Logger = new LoggerConfiguration()
    .DefaultCmp(telemetryConfig, environment)
    .CreateLogger();
```