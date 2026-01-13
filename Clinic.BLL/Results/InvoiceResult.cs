namespace Clinic.BLL.Enums
{
    public enum enInvoiceResult
    {
        // Success Operations
        Success,
        CreatedSuccessfully,
        UpdatedSuccessfully,
        StatusChangedSuccessfully,
        DeletedSuccessfully,

        // Lookup & Identification Errors
        NotFound,
        PatientNotFound,
        AppointmentNotFound,

        // Business Rule Violations
        DuplicateInvoiceNumber,
        AppointmentAlreadyHasInvoice,

        // Financial & Calculation Errors
        InvalidAmount,
        InvalidTaxAmount,
        InvalidDiscountAmount,
        DiscountExceedsTotal,
        InvalidDueDate,
        NegativeBalance,

        // Status Logic & Workflow
        InvalidStatusTransition,
        InvoiceAlreadyPaid,
        InvoiceCancelled,

        // Technical & Validation Errors
        ValidationError,
        DatabaseError,
        UnexpectedError
    }
}