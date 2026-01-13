using Clinic.BLL.Common.Result;
using Clinic.BLL.Enums;
using Clinic.BLL.Results;
using Clinic.BLL.Services;
using Clinic.Contracts.DTOs;
using Clinic.Contracts.Enums;
using System;
using System.Linq;
using System.Collections.Generic;

namespace Clinic.ConsoleUI
{
    /// <summary>
    /// PaymentServiceTest Class: Executes a comprehensive financial workflow test for clinic invoices.
    /// The test flow validates the integrity of the billing system:
    /// 1. Pre-Check: Verifies that the target invoice exists and is in a valid state for payment.
    /// 2. Partial Payment: Validates the ability to pay a portion of the balance and updates invoice status.
    /// 3. Overpayment Protection: Ensures the system blocks payments exceeding the remaining balance.
    /// 4. Transaction History: Verifies accurate logging of multiple payment transactions.
    /// 5. Debt Clearance: Completes the full payment to ensure the invoice transitions to 'Paid' status.
    /// 6. Immutability Rules: Tests protection logic that prevents further payments on fully settled invoices.
    /// </summary>
    public static class PaymentTest
    {
        private static readonly clsPaymentService _paymentService = new clsPaymentService();
        private static readonly clsInvoiceService _invoiceService = new clsInvoiceService();

        // Target Invoice ID for testing purposes
        private static int _testInvoiceId = 2;

        public static void RunTests()
        {
            Console.Clear();
            Console.Title = "Clinic System - Comprehensive Payment Test";
            PrintHeader($"STARTING COMPREHENSIVE PAYMENT SERVICE TEST (Invoice ID: {_testInvoiceId})");

            // Step 0: Ensure the invoice is ready in the database
            if (!CheckTargetInvoice()) return;

            // Step 1: Test valid partial payment
            Test_PartialPayment();

            // Step 2: Test business rule violation (Paying more than owed)
            Test_Overpayment();

            // Step 3: Test retrieval of all transaction records for the invoice
            Test_GetPaymentHistory();

            // Step 4: Test clearing the debt completely
            Test_FullPayment();

            // Step 5: Test protection against paying an already settled invoice
            Test_PaidInvoiceProtection();

            PrintHeader("ALL PAYMENT TESTS COMPLETED SUCCESSFULLY");
            Console.WriteLine("Press any key to return...");
            Console.ReadKey();
        }

        private static bool CheckTargetInvoice()
        {
            PrintSection("0. Checking Target Invoice Status");
            var check = _invoiceService.GetById(_testInvoiceId);

            if (check.IsSuccess && check.Data != null)
            {
                PrintResult($"> Found Invoice ID {_testInvoiceId}. Current Total: {check.Data.TotalAmount:C}", false);
                PrintResult($"> Current Status: {check.Data.InvoiceStatus}", false);

                if (check.Data.InvoiceStatus == enInvoiceStatus.Cancelled)
                {
                    PrintResult("!! Error: Invoice is Cancelled. Cannot test payments on it.", true);
                    return false;
                }
                return true;
            }

            PrintResult($"!! Error: Invoice ID {_testInvoiceId} not found.", true);
            return false;
        }

        static void Test_PartialPayment()
        {
            PrintSection("1. Executing Partial Payment");

            var payment = new PaymentDto
            {
                InvoiceId = _testInvoiceId,
                PaymentAmount = 100.00m,
                PaymentMethod = enPaymentMethod.Cash,
                TransactionRef = "CASH-TEST-001"
            };

            var result = _paymentService.ProcessPayment(payment);
            ProcessResult("Pay $100.00 (Cash)", result);

            // Verify status after partial payment
            var invoice = _invoiceService.GetById(_testInvoiceId);
            if (invoice.IsSuccess && invoice.Data != null)
                PrintResult($"> Post-Payment Status: {invoice.Data.InvoiceStatus}", false);
        }

        static void Test_Overpayment()
        {
            PrintSection("2. Testing Overpayment (Expected Failure)");

            var payment = new PaymentDto
            {
                InvoiceId = _testInvoiceId,
                PaymentAmount = 999999.99m,
                PaymentMethod = enPaymentMethod.Card,
                TransactionRef = "CC-ERR-999"
            };

            var result = _paymentService.ProcessPayment(payment);
            ProcessResult("Attempt Overpayment (Expected failure)", result);
        }

        static void Test_GetPaymentHistory()
        {
            PrintSection("3. Payment History Retrieval");

            var result = _paymentService.GetInvoicePayments(_testInvoiceId);

            if (result.IsSuccess && result.Data != null)
            {
                PrintResult($"> Total payments recorded for this invoice: {result.Data.Count}", false);

                foreach (var p in result.Data)
                {
                    Console.WriteLine($"  - [{p.PaymentDate:yyyy-MM-dd HH:mm}] Method: {p.PaymentMethod} | Amount: {p.PaymentAmount:C} | Ref: {p.TransactionRef}");
                }
            }
            else
            {
                PrintResult("!! Failed to retrieve payments history.", true);
            }
        }

        static void Test_FullPayment()
        {
            PrintSection("4. Clearing Remaining Balance");

            var invoiceResult = _invoiceService.GetById(_testInvoiceId);
            var paymentsResult = _paymentService.GetInvoicePayments(_testInvoiceId);

            if (invoiceResult.IsSuccess && paymentsResult.IsSuccess)
            {
                decimal totalPaid = paymentsResult.Data.Sum(p => p.PaymentAmount);
                decimal remaining = invoiceResult.Data.TotalAmount - totalPaid;

                PrintResult($"> Remaining balance to clear: {remaining:C}", false);

                if (remaining > 0)
                {
                    var finalPayment = new PaymentDto
                    {
                        InvoiceId = _testInvoiceId,
                        PaymentAmount = remaining,
                        PaymentMethod = enPaymentMethod.BankTransfer,
                        TransactionRef = "FINAL-BANK-TR"
                    };

                    var result = _paymentService.ProcessPayment(finalPayment);
                    ProcessResult("Pay Remaining Balance", result);

                    var finalCheck = _invoiceService.GetById(_testInvoiceId);
                    PrintResult($"> Final Invoice Status: {finalCheck.Data.InvoiceStatus}", false);
                }
            }
        }

        static void Test_PaidInvoiceProtection()
        {
            PrintSection("5. Business Rules (Paid Invoice Protection)");

            var payment = new PaymentDto
            {
                InvoiceId = _testInvoiceId,
                PaymentAmount = 1.00m,
                PaymentMethod = enPaymentMethod.Cash
            };

            var result = _paymentService.ProcessPayment(payment);
            ProcessResult("Attempt Payment to Already Paid Invoice (Expected Failure)", result);
        }

        #region UI Helpers

        static void ProcessResult<T>(string actionName, ServiceResult<T, enPaymentResult> result)
        {
            Console.Write($"{actionName,-55} : ");
            if (result.IsSuccess)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"[SUCCESS] -> Result: {result.Data}");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[FAILED]  -> {result.Result}");
            }
            Console.ResetColor();
        }

        static void PrintHeader(string title)
        {
            Console.WriteLine("\n" + new string('=', 85));
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($" {title.ToUpper()} ");
            Console.ResetColor();
            Console.WriteLine(new string('=', 85) + "\n");
        }

        static void PrintSection(string name)
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine($"\n>>> {name}");
            Console.ResetColor();
            Console.WriteLine(new string('-', 65));
        }

        static void PrintResult(string msg, bool warn)
        {
            Console.ForegroundColor = warn ? ConsoleColor.Yellow : ConsoleColor.White;
            Console.WriteLine(msg);
            Console.ResetColor();
        }

        #endregion
    }
}