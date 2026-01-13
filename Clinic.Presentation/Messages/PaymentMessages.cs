using Clinic.BLL.Enums;

namespace Clinic.BLL.Common
{
    public static class clsPaymentMessages
    {
        public static string GetMessage(enPaymentResult result)
        {
            switch (result)
            {
                case enPaymentResult.Success:
                    return "Payment has been processed successfully.";
                case enPaymentResult.CreationFailed:
                    return "Failed to create the payment record.";
                case enPaymentResult.InvoiceNotFound:
                    return "The specified invoice was not found in the system.";
                case enPaymentResult.InvoiceAlreadyPaid:
                    return "This invoice has already been fully paid.";
                case enPaymentResult.InvoiceCancelled:
                    return "Cannot process payment for a cancelled invoice.";
                case enPaymentResult.AmountExceedsRemainingBalance:
                    return "The payment amount exceeds the remaining balance of the invoice.";
                case enPaymentResult.InvalidPaymentAmount:
                    return "The provided payment amount is invalid.";
                case enPaymentResult.DatabaseError:
                    return "A database error occurred while processing the payment.";
                case enPaymentResult.ValidationError:
                    return "One or more validation errors occurred.";
                case enPaymentResult.TransactionRefRequired:
                    return "A transaction reference is required for this payment method.";
                case enPaymentResult.AccessDenied:
                    return "You do not have permission to perform this action.";
                default:
                    return "An unknown error occurred.";
            }
        }
    }
}