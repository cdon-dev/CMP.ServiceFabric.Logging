using System.Collections.Generic;
using System.Fabric;
using System.IO;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.ApplicationInsights.ServiceFabric;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.ServiceFabric.Services.Communication.AspNetCore;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using Serilog;

namespace SfWeb
{
    internal sealed class SfWeb : StatelessService
    {
        public SfWeb(StatelessServiceContext context)
            : base(context)
        {}

        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
            return new ServiceInstanceListener[]
            {
                new ServiceInstanceListener(serviceContext =>
                    new KestrelCommunicationListener(serviceContext, "ServiceEndpoint", (url, listener) =>
                    {
                        ServiceEventSource.Current.ServiceMessage(serviceContext, $"Starting Kestrel on {url}");

                        return new WebHostBuilder()
                                    .UseKestrel()
                                    .ConfigureServices(
                                        services => services
                                            .AddSingleton(serviceContext)
                                            .AddSingleton<ITelemetryInitializer>(serviceProvider => 
                                                FabricTelemetryInitializerExtension.CreateFabricTelemetryInitializer(serviceContext)))
                                    //NOTE https://github.com/microsoft/ApplicationInsights-ServiceFabric#net-core
                                    //NOTE see startup
                                    //NOTE : https://github.com/MicrosoftDocs/azure-docs/issues/27395#issuecomment-473767218
                                    .UseContentRoot(Directory.GetCurrentDirectory())
                                    .UseStartup<Startup>()
                                    .UseServiceFabricIntegration(listener, ServiceFabricIntegrationOptions.None)
                                    .UseUrls(url)
                                    .UseSerilog() //uses the static log, provides core ILogger - https://github.com/serilog/serilog-aspnetcore
                                    .Build();
                    }))
            };
        }
    }
}
