using System;

namespace Contracts.Events
{
    public interface IPaymentAuthorized
    {
        public Guid PaymentId { get; }
        public string AuthorizationCode { get; }
        public string TransactionId { get; }
        public TimeSpan AuthorizationExpiration { get; set; }
    }
}