using System;
using System.Linq;
using System.Threading.Tasks;
using Components.StateMachines;
using Contracts.Events;
using MassTransit;
using MassTransit.Testing;
using NUnit.Framework;

namespace Components.Tests.StateMachines
{
    public class OrderStateMachineTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public async Task Should_insert_new_state_instance_on_submitted()
        {
            var orderStateMachine = new OrderStateMachine();

            var harness = new InMemoryTestHarness {TestTimeout = TimeSpan.FromSeconds(5)};
            var saga = harness.StateMachineSaga<OrderState, OrderStateMachine>(orderStateMachine);

            await harness.Start();
            try
            {
                var orderId = NewId.NextGuid();

                await harness.Bus.Publish<IOrderSubmitted>(new
                {
                    OrderId = orderId,
                    ExternalOrderId = "00000001"
                });

                Assert.That(saga.Created.Select(x => x.CorrelationId == orderId).Any(), Is.True);

                var instanceId = await saga.Exists(orderId, x => x.Submitted);
                Assert.That(instanceId, Is.Not.Null);
                
                var instance = saga.Sagas.Contains(instanceId.Value);
                Assert.That(instance.ExternalOrderId, Is.EqualTo("00000001"));
            }
            finally
            {
                await harness.Stop();
            }
        }
    }
}