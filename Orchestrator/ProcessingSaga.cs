using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nsb6DedupeRepro.Shared.Messages;
using NServiceBus;
using NServiceBus.Logging;

namespace Nsb6DedupeRepro.Orchestrator
{
    public class ProcessingSaga : Saga<ProcessingSagaData>,
        IAmStartedByMessages<StartProcessingSaga>,
        IHandleMessages<CompleteProcessingSaga>,
        IHandleTimeouts<CheckForDataTimeout>
    {
        private static ILog Logger = LogManager.GetLogger<WorkerRole>();

        public async Task Handle(StartProcessingSaga message, IMessageHandlerContext context)
        {
            Data.UniqueSagaId = message.UniqueSagaId;
            Data.Username = message.Username;
            
            //Setup the first timeout
            var timeout = CreateCheckForDataTimeout(context);
            Logger.Info($"Created saga {Data.UniqueSagaId} for user {Data.Username}.");
            await timeout.ConfigureAwait(false);
        }
        
        public async Task Timeout(CheckForDataTimeout state, IMessageHandlerContext context)
        {
            Logger.Info($"Processing timeout for saga {Data.UniqueSagaId} for user {Data.Username}.");

            var message = new ProcessGeneratedData
            {
                UniqueSagaId = Data.UniqueSagaId,
                Username = Data.Username,
                RandomIrrelevantData = new Random().Next()  //Pretend we have done some sort of work or calculation
            };

            //Send a process generated data message and set a new timeout
            var timeoutTask = CreateCheckForDataTimeout(context);
            var sendTask = context.Send(message);
            await Task.WhenAll(timeoutTask, sendTask).ConfigureAwait(false);
        }
        
        protected override void ConfigureHowToFindSaga(SagaPropertyMapper<ProcessingSagaData> mapper)
        {
            mapper.ConfigureMapping<StartProcessingSaga>(
                    message => message.UniqueSagaId)
                .ToSaga(sagaData => sagaData.UniqueSagaId);

            mapper.ConfigureMapping<CompleteProcessingSaga>(
                    message => message.UniqueSagaId)
                .ToSaga(sagaData => sagaData.UniqueSagaId);
        }

        private Task CreateCheckForDataTimeout(IMessageHandlerContext context)
        {
            return RequestTimeout<CheckForDataTimeout>(context, DateTime.UtcNow.AddMinutes(6));
        }

        public Task Handle(CompleteProcessingSaga message, IMessageHandlerContext context)
        {
            Logger.Info($"Completed saga {Data.UniqueSagaId} for user {Data.Username}.");
            MarkAsComplete();
            return Task.CompletedTask;
        }
    }
}
