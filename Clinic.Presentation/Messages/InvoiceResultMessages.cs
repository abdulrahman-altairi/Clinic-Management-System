using Clinic.BLL.Enums;

namespace Clinic.BLL.Results
{
    public static class clsInvoiceResultMessages
    {
        public static string GetMessage(enInvoiceResult result)
        {
            switch (result)
            {
                case enInvoiceResult.Success:
                    return "Invoice operation completed successfully.";

                case enInvoiceResult.CreatedSuccessfully:
                    return "The invoice has been created successfully.";

                case enInvoiceResult.UpdatedSuccessfully:
                    return "The invoice has been updated successfully.";

                case enInvoiceResult.StatusChangedSuccessfully:
                    return "The invoice status has been changed successfully.";

                case enInvoiceResult.DeletedSuccessfully:
                    return "The invoice has been deleted successfully.";

                case enInvoiceResult.NotFound:
                    return "The specified invoice could not be found.";

                case enInvoiceResult.DuplicateInvoiceNumber:
                    return "The invoice number already exists in the system.";

                case enInvoiceResult.AppointmentAlreadyHasInvoice:
                    return "An invoice already exists for this appointment; duplicate billing is not allowed.";

                case enInvoiceResult.PatientNotFound:
                    return "The specified patient does not exist in the system.";

                case enInvoiceResult.AppointmentNotFound:
                    return "The specified appointment does not exist in the system.";

                // حالات الفشل المالي
                case enInvoiceResult.InvalidAmount:
                    return "The total amount provided is invalid.";

                case enInvoiceResult.InvalidTaxAmount:
                    return "The tax amount provided is invalid.";

                case enInvoiceResult.InvalidDiscountAmount:
                    return "The discount amount provided is invalid.";

                case enInvoiceResult.DiscountExceedsTotal:
                    return "The discount amount cannot be greater than the total amount of the invoice.";

                case enInvoiceResult.InvalidDueDate:
                    return "The provided due date is invalid; it cannot be in the past.";

                case enInvoiceResult.NegativeBalance:
                    return "The operation would result in a negative balance, which is not allowed.";

                case enInvoiceResult.InvalidStatusTransition:
                    return "The requested status change is not logically allowed.";

                case enInvoiceResult.InvoiceAlreadyPaid:
                    return "This invoice is already paid and cannot be modified.";

                case enInvoiceResult.InvoiceCancelled:
                    return "This invoice is cancelled and no further actions can be performed on it.";

                case enInvoiceResult.ValidationError:
                    return "One or more validation errors occurred in the invoice data.";

                case enInvoiceResult.DatabaseError:
                    return "A database error occurred while processing the invoice.";

                case enInvoiceResult.UnexpectedError:
                    return "An unexpected error occurred in the finance service.";

                default:
                    return "An unknown error occurred in the invoice service.";
            }
        }
    }
}