using System;

namespace Contracts.Events
{
    public interface IPaymentCaptured
    {
        public string TransactionId { get; }
        public DateTime CapturedDateTime { get; }
    }
}