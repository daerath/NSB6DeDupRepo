﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NServiceBus;

namespace Nsb6DedupeRepro.Shared.Messages
{
    public class ProcessingSagaData : IContainSagaData
    {
        public Guid Id { get; set; }
        public string Originator { get; set; }
        public string OriginalMessageId { get; set; }
        public Guid UniqueSagaId { get; set; }
        public string Username { get; set; }
    }
}
