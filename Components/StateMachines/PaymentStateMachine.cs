using System;
using Automatonymous;
using Contracts.Events;
using MassTransit;

namespace Components.StateMachines
{
    public class PaymentStateMachine : MassTransitStateMachine<PaymentState>
    {
        public PaymentStateMachine()
        {
            Event(() => OnAuthorized, x => x.CorrelateById(y => y.Message.PaymentId));
            Event(() => OnCancelled, x => x.CorrelateById(y => y.Message.PaymentId));
            Event(() => OnCaptured, x => x.CorrelateBy((saga, context) => saga.TransactionId == context.Message.TransactionId));
            Event(() => OnRefunded, x => x.CorrelateBy((saga, context) => saga.TransactionId == context.Message.TransactionId));
            
            // Schedule(() => OnAuthorizationExpired, x => x.AuthorizationExpiredTokenId, s =>
            // {
            //     s.Delay = TimeSpan.FromHours(6);
            //     s.Received = x => x.CorrelateById(m => m.Message.PaymentId);
            // });
            
            InstanceState(x => x.CurrentState);
            
            Initially(
                When(OnAuthorized)
                    .Then(ctx =>
                    {
                        ctx.Instance.AuthorizationCode = ctx.Data.AuthorizationCode;
                        ctx.Instance.TransactionId = ctx.Data.TransactionId;
                    })
                    .TransitionTo(Authorized)
                    // .Schedule(OnAuthorizationExpired, context => context.Init<IAuthorizationExpired>(new
                    // {
                    //     PaymentId = context.Data.PaymentId
                    // }), ctx => ctx.Data.AuthorizationExpiration)
                );
            
            During(Authorized,
                Ignore(OnAuthorized),
                Ignore(OnRefunded),
                // When(OnAuthorizationExpired.Received)
                //     .TransitionTo(Cancelled),
                When(OnCancelled)
                    // .Unschedule(OnAuthorizationExpired)
                    .TransitionTo(Cancelled),
                When(OnCaptured)
                    // .Unschedule(OnAuthorizationExpired)
                    .TransitionTo(Captured));
            
            During(Captured,
                Ignore(OnAuthorized),
                Ignore(OnCaptured), 
                Ignore(OnCancelled),
                When(OnRefunded)
                    .TransitionTo(Refunded));
        }

        public State Authorized { get; set; }
        public State Captured { get; set; }
        public State Cancelled { get; set; }
        public State Refunded { get; set; }

        // public Schedule<PaymentState, IAuthorizationExpired> OnAuthorizationExpired { get; set; }

        public Event<IPaymentAuthorized> OnAuthorized { get; private set; }
        public Event<IPaymentCaptured> OnCaptured { get; private set; }
        public Event<IPaymentCancelled> OnCancelled { get; private set; }
        public Event<IPaymentRefunded> OnRefunded { get; private set; }
    }
}