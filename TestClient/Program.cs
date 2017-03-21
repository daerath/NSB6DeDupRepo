using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading.Tasks;
using Nsb6DedupeRepro.Shared.Messages;
using NServiceBus;
using NServiceBus.Logging;

namespace Nsb6DedupeRepro.TestClient
{
    class Program
    {
        private static ILog Logger = LogManager.GetLogger("Nsb6DedupeRepro.TestClient");

        static void Main(string[] args)
        {
            AsyncMain().GetAwaiter().GetResult();
        }

        static async Task AsyncMain()
        {
            Console.Title = "Nsb6DedupeRepro.TestClient";

            var configuration = new EndpointConfiguration("Nsb6DedupeRepro.Orchestrator");
            var serviceBusConnectionString = ConfigurationManager.AppSettings["Microsoft.ServiceBus.ConnectionString"];
            var transport = configuration.UseTransport<AzureServiceBusTransport>();

            configuration.SendOnly();
            transport.ConnectionString(serviceBusConnectionString);
            transport.UseEndpointOrientedTopology();
            var routing = transport.Routing();
            routing.RouteToEndpoint(typeof(StartProcessingSaga), "Nsb6DedupeRepro.Orchestrator");

            configuration.UsePersistence<InMemoryPersistence>();
            configuration.UseSerialization<JsonSerializer>();
            configuration.SendFailedMessagesTo("Nsb6DedupeRepro.error");
            configuration.EnableInstallers();

            var endpointInstance = await Endpoint.Start(configuration).ConfigureAwait(false);
            
            await RunLoop(endpointInstance).ConfigureAwait(false);

            await endpointInstance.Stop()
                .ConfigureAwait(false);
        }

        static async Task RunLoop(IEndpointInstance endpointInstance)
        {
            while (true)
            {
                Console.WriteLine("Enter a number of sagas to start then press enter.");

                int numberToSend;

                if (int.TryParse(Console.ReadLine(), out numberToSend))
                {
                    Console.WriteLine();
                    List<Task> tasks = new List<Task>();
                    for (var i = 0; i < numberToSend; i++)
                    {
                        var command = new StartProcessingSaga
                        {
                            UniqueSagaId = Guid.NewGuid(),
                            Username = $"User-{i}"
                        };
                        Logger.Info($"Starting saga = {command.Username}");

                        tasks.Add(endpointInstance.Send(command));
                    }

                    await Task.WhenAll(tasks).ConfigureAwait(false);
                    Console.WriteLine($"Started {numberToSend} sagas.");
                }
            }
        }
    }
}
