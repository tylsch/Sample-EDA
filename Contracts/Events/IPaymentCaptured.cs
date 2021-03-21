using System;

namespace Contracts.Events
{
    public interface IPaymentCaptured
    {
        public string TransactionId { get; set; }
        public DateTime CapturedDateTime { get; set; }
    }
}