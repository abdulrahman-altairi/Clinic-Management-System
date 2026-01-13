namespace Clinic.BLL.Enums
{
    public enum enInvoiceItemResult
    {
        // Success Operations
        Success,
        AddedSuccessfully,
        UpdatedSuccessfully,
        DeletedSuccessfully,

        // Lookup & Identification
        NotFound,
        ItemNotFound,
        ParentInvoiceNotFound,

        // Validation & Data Integrity
        ValidationError,
        DescriptionRequired,
        InvalidQuantity,
        InvalidPrice,

        // Parent Invoice State Rules
        InvoiceAlreadyClosed,
        InvoiceCancelled,

        // Technical Errors
        DatabaseError,
        UnexpectedError
    }
}