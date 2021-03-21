using System;

namespace Contracts.Events
{
    public interface IPaymentCancelled
    {
        public Guid PaymentId { get; set; }
    }
}