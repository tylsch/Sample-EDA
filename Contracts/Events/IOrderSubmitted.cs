using System;

namespace Contracts.Events
{
    public interface IOrderSubmitted
    {
        public Guid OrderId { get; set; }
        public string ExternalOrderId { get; set; }
    }
}