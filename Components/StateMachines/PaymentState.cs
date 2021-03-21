using System;
using Automatonymous;
using MassTransit.Saga;

namespace Components.StateMachines
{
    public class PaymentState : SagaStateMachineInstance, ISagaVersion
    {
        public Guid CorrelationId { get; set; }
        public int Version { get; set; }
        public string CurrentState { get; set; }

        public string AuthorizationCode { get; set; }
        public string TransactionId { get; set; }
    }
}