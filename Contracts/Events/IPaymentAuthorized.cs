using System;

namespace Contracts.Events
{
    public interface IPaymentAuthorized
    {
        public Guid PaymentId { get; set; }
        public string AuthorizationCode { get; set; }
        public string TransactionId { get; set; }
    }
}