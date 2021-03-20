using System;

namespace Contracts.Commands
{
    public interface ISubmitNewAcquisition
    {
        public string ExternalOrderId { get; set; }
    }
}