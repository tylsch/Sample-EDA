namespace Contracts.Events
{
    public interface IPaymentRefunded
    {
        public string TransactionId { get; set; }
    }
}