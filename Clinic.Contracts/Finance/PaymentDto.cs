using Clinic.Contracts.Enums;
using System;

namespace Clinic.Contracts.DTOs
{
    public class PaymentDto
    {
        public int PaymentId { get; set; }

        public int InvoiceId { get; set; }

        public decimal PaymentAmount { get; set; }

        public DateTime PaymentDate { get; set; }

        public enPaymentMethod PaymentMethod { get; set; }

        public string TransactionRef { get; set; }

        public string InvoiceNumber { get; set; }
    }
}