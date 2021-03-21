using Automatonymous;
using Contracts.Events;

namespace Components.StateMachines
{
    public class OrderStateMachine : MassTransitStateMachine<OrderState>
    {
        public OrderStateMachine()
        {
            Event(() => OnOrderSubmitted, x => x.CorrelateById(x => x.Message.OrderId));
            
            InstanceState(x => x.CurrentState);
            
            Initially(
                When(OnOrderSubmitted)
                    .Then(ctx =>
                    {
                        ctx.Instance.ExternalOrderId = ctx.Data.ExternalOrderId;
                    })
                    .TransitionTo(Submitted));
        }

        public State Submitted { get; set; }

        public Event<IOrderSubmitted> OnOrderSubmitted { get; set; }
    }
}