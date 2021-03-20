using System.Threading.Tasks;
using Contracts.Commands;
using MassTransit;

namespace Components.Consumers
{
    public class SubmitNewAcquisitionConsumer : IConsumer<ISubmitNewAcquisition>
    {
        public Task Consume(ConsumeContext<ISubmitNewAcquisition> context)
        {
            throw new System.NotImplementedException();
        }
    }
}