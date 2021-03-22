using System;

namespace Contracts.Events
{
    public interface IAuthorizationExpired
    {
        public Guid PaymentId { get; set; }
    }
}