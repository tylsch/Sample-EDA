using System;

namespace Contracts.Events
{
    public interface IOrderSubmitted
    {
        public Guid OrderId { get; }
        public string ExternalOrderId { get; }
    }
}