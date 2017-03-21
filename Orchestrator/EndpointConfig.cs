using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nsb6DedupeRepro.Shared.Messages;
using NServiceBus;

namespace Nsb6DedupeRepro.Orchestrator
{
    public class EndpointConfig : IConfigureThisEndpoint, AsA_Worker
    {
        public void Customize(EndpointConfiguration configuration)
        {
            var serviceBusConnectionString = ConfigurationManager.AppSettings["Microsoft.ServiceBus.ConnectionString"];
            var transport = configuration.UseTransport<AzureServiceBusTransport>();
            transport.ConnectionString(serviceBusConnectionString);
            transport.UseEndpointOrientedTopology();
            var routing = transport.Routing();
            routing.RouteToEndpoint(typeof(ProcessGeneratedData), "Nsb6DedupeRepro.Consumer");

            configuration.UsePersistence<InMemoryPersistence>();
            configuration.UseSerialization<JsonSerializer>();
            configuration.SendFailedMessagesTo("Nsb6DedupeRepro.error");
            configuration.EnableInstallers();
        }
    }
}
