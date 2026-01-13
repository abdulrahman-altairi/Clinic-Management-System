using Clinic.BLL.Enums;

namespace Clinic.BLL.Results
{
    public static class clsInvoiceItemResultMessages
    {
        public static string GetMessage(enInvoiceItemResult result)
        {
            switch (result)
            {
                case enInvoiceItemResult.Success:
                    return "Invoice item operation completed successfully.";

                case enInvoiceItemResult.AddedSuccessfully:
                    return "The item has been added to the invoice successfully.";

                case enInvoiceItemResult.UpdatedSuccessfully:
                    return "The invoice item has been updated successfully.";

                case enInvoiceItemResult.DeletedSuccessfully:
                    return "The item has been removed from the invoice successfully.";

                case enInvoiceItemResult.ValidationError:
                    return "Validation failed for the invoice item data.";

                case enInvoiceItemResult.InvalidQuantity:
                    return "The quantity must be a positive integer greater than zero.";

                case enInvoiceItemResult.InvalidPrice:
                    return "The unit price cannot be a negative value.";

                case enInvoiceItemResult.DescriptionRequired:
                    return "The item description is required and cannot be empty.";

                case enInvoiceItemResult.NotFound:
                    return "The specified invoice item could not be found.";

                case enInvoiceItemResult.ParentInvoiceNotFound:
                    return "The associated invoice for this item does not exist.";

                case enInvoiceItemResult.InvoiceAlreadyClosed:
                    return "Cannot modify items because the invoice is already paid.";

                case enInvoiceItemResult.InvoiceCancelled:
                    return "Cannot modify items because the invoice has been cancelled.";

                case enInvoiceItemResult.DatabaseError:
                    return "A database error occurred while processing the invoice item.";

                case enInvoiceItemResult.UnexpectedError:
                    return "An unexpected error occurred while processing the invoice item.";

                default:
                    return "An unknown error occurred in the invoice item service.";
            }
        }
    }
}