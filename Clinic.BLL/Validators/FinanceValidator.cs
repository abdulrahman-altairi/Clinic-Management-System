using Clinic.BLL.Helper;
using Clinic.BLL.Results;
using Clinic.Contracts;
using Clinic.Contracts.DTOs;
using System;
using System.Collections.Generic;
using Clinic.Entities.Enums;

namespace Clinic.BLL.Validators
{

    // Provides financial validation logic to ensure accounting integrity for invoices, payments, and billing items.

    public class clsFinanceValidator
    {
        public static List<enValidationResult> ValidateInvoice(InvoiceDto invoiceDto)
        {
            var errors = new List<enValidationResult>();

            new clsValidationRole(invoiceDto.TotalAmount.ToString(), errors)
                .NotEmpty(enValidationResult.TotalAmountRequired)
                .IsMoney(enValidationResult.InvalidMoneyFormat)
                .GreaterThanZero(enValidationResult.AmountMustBePositive);

            new clsValidationRole(invoiceDto.TaxAmount.ToString(), errors)
                .IsMoney(enValidationResult.InvalidMoneyFormat)
                .NotNegative(enValidationResult.TaxCannotBeNegative);

            new clsValidationRole(invoiceDto.DiscountAmount.ToString(), errors)
                .IsMoney(enValidationResult.InvalidMoneyFormat)
                .NotNegative(enValidationResult.DiscountCannotBeNegative);

            if (invoiceDto.DueDate.HasValue)
            {
                new clsValidationRole(invoiceDto.DueDate.Value.ToString(), errors)
                    .Must(date => DateTime.Parse(date).Date >= DateTime.Today, enValidationResult.InvalidDueDate);
            }

            return errors;
        }

        public static List<enValidationResult> ValidateTotalAmount(decimal amount)
        {
            var errors = new List<enValidationResult>();

            new clsValidationRole(amount.ToString(), errors)
                .IsMoney(enValidationResult.InvalidMoneyFormat)
                .GreaterThanZero(enValidationResult.AmountMustBePositive);

            return errors;
        }

        public static List<enValidationResult> ValidateTaxAmount(decimal tax)
        {
            var errors = new List<enValidationResult>();

            new clsValidationRole(tax.ToString(), errors)
                .IsMoney(enValidationResult.InvalidMoneyFormat)
                .NotNegative(enValidationResult.TaxCannotBeNegative);

            return errors;
        }

        public static List<enValidationResult> ValidateDiscountAmount(decimal discount, decimal totalAmount)
        {
            var errors = new List<enValidationResult>();

            new clsValidationRole(discount.ToString(), errors)
                .IsMoney(enValidationResult.InvalidMoneyFormat)
                .NotNegative(enValidationResult.DiscountCannotBeNegative)
                .MaxValue(totalAmount, enValidationResult.DiscountExceedsTotal);

            return errors;
        }

        public static List<enValidationResult> ValidateDueDate(DateTime? dueDate)
        {
            var errors = new List<enValidationResult>();

            if (dueDate.HasValue)
            {
                new clsValidationRole(dueDate.Value.ToString(), errors)
                    .Must(date => DateTime.Parse(date).Date >= DateTime.Today, enValidationResult.InvalidDueDate);
            }

            return errors;
        }

        public static List<enValidationResult> ValidateInvoiceItem(InvoiceItemDto itemDto)
        {
            var errors = new List<enValidationResult>();

            new clsValidationRole(itemDto.ItemDescription, errors)
                .NotEmpty(enValidationResult.DescriptionRequired);

            new clsValidationRole(itemDto.UnitPrice.ToString(), errors)
                .IsMoney(enValidationResult.InvalidMoneyFormat)
                .GreaterThanZero(enValidationResult.AmountMustBePositive);

            new clsValidationRole(itemDto.Quantity.ToString(), errors)
                .Must(qty => int.TryParse(qty, out int result) && result > 0, enValidationResult.InvalidQuantity);

            return errors;
        }

        public static List<enValidationResult> ValidateItemQuantity(int quantity)
        {
            var errors = new List<enValidationResult>();

            new clsValidationRole(quantity.ToString(), errors)
                .Must(qty => int.TryParse(qty, out int result) && result > 0, enValidationResult.InvalidQuantity);

            return errors;
        }

        public static List<enValidationResult> ValidateItemDescription(string description)
        {
            var errors = new List<enValidationResult>();

            new clsValidationRole(description, errors)
                .NotEmpty(enValidationResult.DescriptionRequired)
                .Length(3, 500, enValidationResult.DescriptionTooLongOrShort); 

            return errors;
        }

        public static List<enValidationResult> ValidatePayment(PaymentDto paymentDto, decimal remainingBalance)
        {
            var errors = new List<enValidationResult>();

            new clsValidationRole(paymentDto.PaymentAmount.ToString(), errors)
                .IsMoney(enValidationResult.InvalidMoneyFormat)
                .GreaterThanZero(enValidationResult.AmountMustBePositive)
                .MaxValue(remainingBalance, enValidationResult.AmountExceedsRemainingBalance);

            new clsValidationRole(((int)paymentDto.PaymentMethod).ToString(), errors)
                .IsValidEnum<enPaymentMethod>(enValidationResult.InvalidPaymentMethod);

            if (paymentDto.PaymentMethod != Contracts.Enums.enPaymentMethod.Cash)
            {
                new clsValidationRole(paymentDto.TransactionRef, errors)
                    .NotEmpty(enValidationResult.TransactionRefRequired)
                    .Length(3, 50, enValidationResult.InvalidTransactionRef);
            }

            return errors;
        }
    }
}