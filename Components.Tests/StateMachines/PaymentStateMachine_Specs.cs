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
    public class PaymentStateMachine_Specs
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public async Task Should_insert_new_state_instance_on_submitted()
        {
            var paymentStateMachine = new PaymentStateMachine();

            var harness = new InMemoryTestHarness {TestTimeout = TimeSpan.FromSeconds(5)};
            var saga = harness.StateMachineSaga<PaymentState, PaymentStateMachine>(paymentStateMachine);

            await harness.Start();
            try
            {
                var paymentId = NewId.NextGuid();

                await harness.Bus.Publish<IPaymentAuthorized>(new
                {

                    PaymentId = paymentId,
                    AuthorizationCode = "PASS",
                    TransactionId = "2452354325"
                });

                Assert.That(saga.Created.Select(x => x.CorrelationId == paymentId).Any(), Is.True);

                var instanceId = await saga.Exists(paymentId, x => x.Authorized);
                Assert.That(instanceId, Is.Not.Null);
                
                var instance = saga.Sagas.Contains(instanceId.Value);
                Assert.That(instance.AuthorizationCode, Is.EqualTo("PASS"));
                Assert.That(instance.TransactionId, Is.EqualTo("2452354325"));
            }
            finally
            {
                await harness.Stop();
            }
        }
    }
}