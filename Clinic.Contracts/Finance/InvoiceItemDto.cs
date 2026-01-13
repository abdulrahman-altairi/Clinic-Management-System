namespace Clinic.Contracts
{
    public class InvoiceItemDto
    {
        public int ItemId { get; set; }

        public int InvoiceId { get; set; }

        public string ItemDescription { get; set; }

        public decimal UnitPrice { get; set; }

        public int Quantity { get; set; }

        public decimal LineTotal => UnitPrice * Quantity;

        public string InvoiceNumber { get; set; }
    }
}