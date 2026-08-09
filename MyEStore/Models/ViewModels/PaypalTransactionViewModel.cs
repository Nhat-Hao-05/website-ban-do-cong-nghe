namespace MyEStore.Models.ViewModels
{
    public class PaypalTransactionViewModel
    {
        public string  OrderId { get; set; }
        public string TransactionId { get; set; }
        public decimal  Amount  { get; set; }

        public string Currency {  get; set; }
        
        public string Status { get; set; }
    }
}
