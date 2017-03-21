using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nsb6DedupeRepro.Shared.Messages;
using NServiceBus;
using NServiceBus.Logging;

namespace Nsb6DedupeRepro.Consumer
{
    public class MessageHandlers : IHandleMessages<ProcessGeneratedData>
    {
        private static ILog Logger = LogManager.GetLogger<WorkerRole>();

        public Task Handle(ProcessGeneratedData message, IMessageHandlerContext context)
        {
            Logger.Info($"Processed saga {message.UniqueSagaId} for user {message.Username}");
            return Task.CompletedTask;
        }
    }
}
