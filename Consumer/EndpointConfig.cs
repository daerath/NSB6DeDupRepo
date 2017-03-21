using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NServiceBus;

namespace Nsb6DedupeRepro.Consumer
{
    public class EndpointConfig : IConfigureThisEndpoint, AsA_Worker
    {
        public void Customize(EndpointConfiguration configuration)
        {
            var serviceBusConnectionString = ConfigurationManager.AppSettings["Microsoft.ServiceBus.ConnectionString"];
            var transport = configuration.UseTransport<AzureServiceBusTransport>();
            transport.ConnectionString(serviceBusConnectionString);
            transport.UseEndpointOrientedTopology();

            configuration.UsePersistence<InMemoryPersistence>();
            configuration.UseSerialization<JsonSerializer>();
            configuration.SendFailedMessagesTo("Nsb6DedupeRepro.error");
            configuration.EnableInstallers();
        }
    }
}
