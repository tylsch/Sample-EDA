using System;
using Automatonymous;
using MassTransit.Saga;
using MongoDB.Bson.Serialization.Attributes;

namespace Components.StateMachines
{
    public class OrderState : SagaStateMachineInstance, ISagaVersion
    {
        [BsonId]
        public Guid CorrelationId { get; set; }
        public int Version { get; set; }
        public string CurrentState { get; set; }

        public string ExternalOrderId { get; set; }
    }
}