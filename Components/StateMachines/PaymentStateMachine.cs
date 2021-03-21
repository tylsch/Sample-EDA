using Automatonymous;
using Contracts.Events;

namespace Components.StateMachines
{
    public class PaymentStateMachine : MassTransitStateMachine<PaymentState>
    {
        public PaymentStateMachine()
        {
            Event(() => OnAuthorized, x => x.CorrelateById(y => y.Message.PaymentId));
            Event(() => OnCaptured, x => x.CorrelateBy((saga, context) => saga.TransactionId == context.Message.TransactionId));
            
            InstanceState(x => x.CurrentState);
            
            Initially(
                When(OnAuthorized)
                    .Then(ctx =>
                    {
                        ctx.Instance.AuthorizationCode = ctx.Data.AuthorizationCode;
                        ctx.Instance.TransactionId = ctx.Data.TransactionId;
                    })
                    .TransitionTo(Authorized));
            
            During(Authorized,
                Ignore(OnAuthorized),
                When(OnCaptured)
                    .Then(ctx =>
                    {
                        ctx.Instance.CapturedDateTime = ctx.Data.CapturedDateTime;
                    })
                    .TransitionTo(Captured));
        }

        public State Authorized { get; set; }
        public State Captured { get; set; }

        public Event<IPaymentAuthorized> OnAuthorized { get; private set; }
        public Event<IPaymentCaptured> OnCaptured { get; private set; }
    }
}