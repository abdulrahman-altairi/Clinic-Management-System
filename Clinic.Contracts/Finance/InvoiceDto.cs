using Clinic.Contracts.Enums;
using System;

namespace Clinic.Contracts
{
    public class InvoiceDto
    {
        public int InvoiceId { get; set; }
        public string InvoiceNumber { get; set; }
        public int AppointmentId { get; set; }
        public int PatientId { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal NetAmount { get; set; }
        public DateTime InvoiceDate { get; set; }
        public DateTime? DueDate { get; set; }
        public enInvoiceStatus? InvoiceStatus { get; set; }
        public string PatientName { get; set; }

        public bool IsOverdue => DueDate.HasValue && DueDate.Value < DateTime.Today && InvoiceStatus != enInvoiceStatus.Paid;
    }
}