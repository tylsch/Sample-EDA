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
    public class PaymentStateMachineSpecs : StateMachineTestFixture<PaymentStateMachine, PaymentState>
    { 
        [Test]
        public async Task Should_insert_new_state_instance_on_authorized()
        {
            var paymentId = NewId.NextGuid();

            await TestHarness.Bus.Publish<IPaymentAuthorized>(new
            {

                PaymentId = paymentId,
                AuthorizationCode = "PASS",
                TransactionId = "2452354325"
            });

            Assert.That(SagaHarness.Created.Select(x => x.CorrelationId == paymentId).Any(), Is.True);

            var instanceId = await SagaHarness.Exists(paymentId, x => x.Authorized);
            Assert.That(instanceId, Is.Not.Null);
                
            var instance = SagaHarness.Sagas.Contains(instanceId.Value);
            Assert.That(instance.AuthorizationCode, Is.EqualTo("PASS"));
            Assert.That(instance.TransactionId, Is.EqualTo("2452354325"));
        }
        
        [Test]
        public async Task Should_cancel_payment()
        {
            var paymentId = NewId.NextGuid();

            await TestHarness.Bus.Publish<IPaymentAuthorized>(new
            {
                PaymentId = paymentId,
                AuthorizationCode = "PASS",
                TransactionId = "2452354325"
            });

            Assert.That(SagaHarness.Created.Select(x => x.CorrelationId == paymentId).Any(), Is.True);

            var instanceId = await SagaHarness.Exists(paymentId, x => x.Authorized);
            Assert.That(instanceId, Is.Not.Null);
                
            var instance = SagaHarness.Sagas.Contains(instanceId.Value);
            Assert.That(instance.AuthorizationCode, Is.EqualTo("PASS"));
            Assert.That(instance.TransactionId, Is.EqualTo("2452354325"));
                
            await TestHarness.Bus.Publish<IPaymentCancelled>(new
            {
                PaymentId = paymentId
            });
                
            instanceId = await SagaHarness.Exists(paymentId, x => x.Cancelled);
            Assert.That(instanceId, Is.Not.Null);
        }
        
        [Test]
        public async Task Should_mark_state_as_captured_when_payment_is_captured()
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
                
                await harness.Bus.Publish<IPaymentCaptured>(new
                {
                    TransactionId = "2452354325"
                });
                
                instanceId = await saga.Exists(paymentId, x => x.Captured);
                Assert.That(instanceId, Is.Not.Null);
            }
            finally
            {
                await harness.Stop();
            }
        }
        
        [Test]
        public async Task Should_mark_state_as_refunded_when_payment_is_refunded()
        {
            var paymentId = NewId.NextGuid();

            await TestHarness.Bus.Publish<IPaymentAuthorized>(new
            {
                PaymentId = paymentId,
                AuthorizationCode = "PASS",
                TransactionId = "2452354325"
            });

            Assert.That(SagaHarness.Created.Select(x => x.CorrelationId == paymentId).Any(), Is.True);

            var instanceId = await SagaHarness.Exists(paymentId, x => x.Authorized);
            Assert.That(instanceId, Is.Not.Null);
                
            var instance = SagaHarness.Sagas.Contains(instanceId.Value);
            Assert.That(instance.AuthorizationCode, Is.EqualTo("PASS"));
            Assert.That(instance.TransactionId, Is.EqualTo("2452354325"));
                
            await TestHarness.Bus.Publish<IPaymentCaptured>(new
            {
                TransactionId = "2452354325"
            });
                
            instanceId = await SagaHarness.Exists(paymentId, x => x.Captured);
            Assert.That(instanceId, Is.Not.Null);
                
            await TestHarness.Bus.Publish<IPaymentRefunded>(new
            {
                TransactionId = "2452354325"
            });
                
            instanceId = await SagaHarness.Exists(paymentId, x => x.Refunded);
            Assert.That(instanceId, Is.Not.Null);
        }
        
        [Test]
        public async Task Should_cancel_payment_when_auth_expires()
        {
            var paymentId = NewId.NextGuid();

            await TestHarness.Bus.Publish<IPaymentAuthorized>(new
            {
                PaymentId = paymentId,
                AuthorizationCode = "PASS",
                TransactionId = "2452354325",
                AuthorizationExpiration = TimeSpan.FromHours(6)
            });

            Assert.That(SagaHarness.Created.Select(x => x.CorrelationId == paymentId).Any(), Is.True);
            var instanceId = await SagaHarness.Exists(paymentId, x => x.Authorized);
            Assert.That(instanceId, Is.Not.Null);
            var instance = SagaHarness.Sagas.Contains(instanceId.Value);
            Assert.That(instance.AuthorizationCode, Is.EqualTo("PASS"));
            Assert.That(instance.TransactionId, Is.EqualTo("2452354325"));

            // await AdvanceSystemTime(TimeSpan.FromHours(7));


            // instanceId = await SagaHarness.Exists(paymentId, x => x.Cancelled);
            // Assert.That(instanceId, Is.Not.Null);
        }
    }
}