using System;
using Clinic.Entities.Enums;
namespace Clinic.Entities
{
    public class Invoice
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

    }
}
