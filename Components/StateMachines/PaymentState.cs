using System;
using Automatonymous;
using MassTransit.Saga;
using MongoDB.Bson.Serialization.Attributes;

namespace Components.StateMachines
{
    public class PaymentState : SagaStateMachineInstance, ISagaVersion
    {
        [BsonId]
        public Guid CorrelationId { get; set; }
        public int Version { get; set; }
        public string CurrentState { get; set; }

        public string AuthorizationCode { get; set; }
        public string TransactionId { get; set; }

        public Guid? AuthorizationExpiredTokenId { get; set; }
    }
}