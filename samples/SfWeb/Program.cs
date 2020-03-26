using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.ServiceFabric.Services.Runtime;
using Serilog;
using System;
using System.Diagnostics;
using System.Threading;
using CMP.ServiceFabric.Logging;

namespace SfWeb
{
    internal static class Program
    {
        private static void Main()
        {
            var telemetryConfiguration = TelemetryConfiguration.CreateDefault();
            telemetryConfiguration.InstrumentationKey = "key";

            var telemetryClient = new TelemetryClient(telemetryConfiguration);

            Log.Logger = new LoggerConfiguration()
                .DefaultCmp(telemetryConfiguration, "development")
                .CreateLogger();

            try
            {
                ServiceRuntime.RegisterServiceAsync("SfWebType",
                    context =>
                    {
                        context.ConfigureLogging(telemetryConfiguration, Log.Logger, "SfWebType");
                        return new SfWeb(context);
                    }).GetAwaiter().GetResult();

                ServiceEventSource.Current.ServiceTypeRegistered(Process.GetCurrentProcess().Id, typeof(SfWeb).Name);

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
