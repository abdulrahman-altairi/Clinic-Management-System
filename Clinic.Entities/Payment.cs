using System;
using Clinic.Entities.Enums;

namespace Clinic.Entities
{
    public class Payment
    {
        public int PaymentId { get; set; }
        public int InvoiceId { get; set; }
        public decimal PaymentAmount { get; set; }
        public DateTime PaymentDate { get; set; }
        public enPaymentMethod PaymentMethod { get; set; }
        public string TransactionRef { get; set; }
    }
}
