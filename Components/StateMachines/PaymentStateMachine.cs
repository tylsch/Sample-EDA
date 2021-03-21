using Automatonymous;
using Contracts.Events;

namespace Components.StateMachines
{
    public class PaymentStateMachine : MassTransitStateMachine<PaymentState>
    {
        public PaymentStateMachine()
        {
            Event(() => OnAuthorizedPayment, x => x.CorrelateById(x => x.Message.PaymentId));
            
            InstanceState(x => x.CurrentState);
            
            Initially(
                When(OnAuthorizedPayment)
                    .Then(ctx =>
                    {
                        ctx.Instance.AuthorizationCode = ctx.Data.AuthorizationCode;
                        ctx.Instance.TransactionId = ctx.Data.TransactionId;
                    })
                    .TransitionTo(Authorized));
        }

        public State Authorized { get; set; }

        public Event<IPaymentAuthorized> OnAuthorizedPayment { get; private set; }
    }
}