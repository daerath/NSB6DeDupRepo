using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NServiceBus;

namespace Nsb6DedupeRepro.Shared.Messages
{
    public class CompleteProcessingSaga : ICommand
    {
        public string Username { get; set; }
        public Guid UniqueSagaId { get; set; }
    }
}
