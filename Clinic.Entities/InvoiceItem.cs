using System;

namespace Clinic.Entities
{
    public class InvoiceItem
    {
        public int ItemId { get; set; }
        public int InvoiceId { get; set; }
        public string ItemDescription { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal LineTotal { get; set; }
    }
}
