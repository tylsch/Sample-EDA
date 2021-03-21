using System;

namespace Contracts.Events
{
    public interface IPaymentCaptured
    {
        public string TransactionId { get; }
    }
}