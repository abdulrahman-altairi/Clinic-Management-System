namespace Clinic.BLL.Enums
{
    public enum enPaymentResult
    {
        // General Success
        Success,

        // Invoice & Balance Status
        InvoiceNotFound,
        InvoiceAlreadyPaid,
        InvoiceCancelled,
        AmountExceedsRemainingBalance,

        // Validation & Data Integrity
        InvalidPaymentAmount,
        TransactionRefRequired,
        ValidationError,

        // System & Security
        CreationFailed,
        DatabaseError,
        AccessDenied,
        OperationFailed
    }
}