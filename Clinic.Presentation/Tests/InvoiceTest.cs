using Clinic.BLL.Common.Result;
using Clinic.BLL.Enums;
using Clinic.BLL.Services;
using Clinic.Contracts;
using Clinic.Contracts.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Clinic.ConsoleUI
{
    /// <summary>
    /// InvoiceServiceTest Class: Validates the financial and billing layer of the Clinic System.
    /// This test suite focuses on strict financial business rules:
    /// 1. Creation & Integrity: Validates that invoices are linked correctly to appointments and patients.
    /// 2. Validation: Ensures negative amounts are rejected and one-invoice-per-appointment rules are enforced.
    /// 3. Status Transitions: Tests the lifecycle of an invoice (Issued -> Paid) and checks for immutable states.
    /// 4. Business Rules: Verifies that "Paid" invoices cannot be modified or reverted to "Issued".
    /// 5. Financial Metrics: Tests logic for outstanding balances and revenue calculation.
    /// </summary>
    public class InvoiceTest
    {
        private static clsInvoiceService _invoiceService = new clsInvoiceService();
        private static int _testInvoiceId = 0;
        private static int _targetAppointmentId = 1;
        private static int _targetPatientId = 3;

        public static void RunTests()
        {
            Console.Title = "Clinic System - Smart Invoice Service Test";
            PrintHeader("STARTING SMART INVOICE SERVICE TEST");

            // Phase 0: Smart Environment Preparation
            if (!PrepareTestData())
            {
                PrintResult("!! Aborting: Environment could not be prepared.", true);
                return;
            }

            // Phase 1: Test Invoice Creation and Financial constraints
            Test_CreateInvoice();

            if (_testInvoiceId > 0)
            {
                // Phase 2: Test Retrieval methods
                Test_DataRetrieval();

                // Phase 3: Test Update logic and business rule enforcement
                Test_UpdateInvoice();

                // Phase 4: Test Invoice State Management (Transitions)
                Test_StatusTransitions();

                // Phase 5: Test Financial reporting and metrics
                Test_FinancialMetrics();
            }
            else
            {
                PrintResult("!! Aborting further tests because Invoice Creation failed.", true);
            }

            PrintHeader("INVOICE TESTING COMPLETED");
            Console.WriteLine("Press any key to return to main menu...");
            Console.ReadKey();
        }

        private static bool PrepareTestData()
        {
            PrintSection("0. Smart Environment Preparation");

            Console.WriteLine($"Targeting Appointment ID: {_targetAppointmentId}");
            Console.WriteLine($"Targeting Patient ID: {_targetPatientId}");

            // Business Rule Validation: Ensuring environment is clean for a fresh test.
            // If an invoice already exists for this appointment, creation tests might fail due to system constraints.
            var check = _invoiceService.CheckInvoiceExists(_targetAppointmentId);
            if (check.IsSuccess && check.Data > 0)
            {
                PrintResult($"> Warning: Appointment {_targetAppointmentId} already has an invoice. System rules will prevent duplicates.", true);
            }

            return true;
        }

        #region Test Methods

        static void Test_CreateInvoice()
        {
            PrintSection("1. Invoice Creation & Financial Validation");

            // Scenario A: Valid Invoice
            var invoiceDto = new InvoiceDto
            {
                AppointmentId = _targetAppointmentId,
                PatientId = _targetPatientId,
                TotalAmount = 500.00m,
                TaxAmount = 75.00m,
                DiscountAmount = 25.00m,
                DueDate = DateTime.Now.AddDays(7),
                InvoiceStatus = (Contracts.Enums.enInvoiceStatus)enInvoiceStatus.Issued
            };

            var result = _invoiceService.CreateInvoice(invoiceDto);
            if (result.IsSuccess) _testInvoiceId = result.Data;
            ProcessResult("Create New Invoice (Valid)", result);

            // Scenario B: Validation Failure (Negative Amount)
            var invalidDto = new InvoiceDto { AppointmentId = _targetAppointmentId, TotalAmount = -100 };
            ProcessResult("Create Invoice (Invalid - Negative Amount)", _invoiceService.CreateInvoice(invalidDto));

            // Scenario C: Business Rule Failure (Duplicate Invoice for same Appointment)
            ProcessResult("Create Invoice (Duplicate Appointment ID)", _invoiceService.CreateInvoice(invoiceDto));
        }

        static void Test_DataRetrieval()
        {
            PrintSection("2. Data Retrieval Methods");

            ProcessResult("Get Invoice By ID", _invoiceService.GetById(_testInvoiceId));
            ProcessResult($"Get Invoices for Patient ID: {_targetPatientId}", _invoiceService.GetPatientInvoices(_targetPatientId));

            ProcessResult("Get Invoices By Date Range (Last 30 Days)",
                _invoiceService.GetInvoicesByDateRange(DateTime.Now.AddDays(-30), DateTime.Now));
        }

        static void Test_UpdateInvoice()
        {
            PrintSection("3. Update Invoice & Business Rules");

            var updateDto = new InvoiceDto
            {
                InvoiceId = _testInvoiceId,
                TotalAmount = 600.00m, // Increasing the amount
                TaxAmount = 90.00m,
                DiscountAmount = 0.00m
            };

            ProcessResult("Update Invoice Amounts", _invoiceService.UpdateInvoice(updateDto));
        }

        static void Test_StatusTransitions()
        {
            PrintSection("4. Invoice Status Transitions");

            // Scenario D: Moving to a terminal state (Paid)
            ProcessResult("Change Status to [Paid]", _invoiceService.UpdateStatus(_testInvoiceId, enInvoiceStatus.Paid));

            // Scenario E: Immutable State Check (Updating a Paid invoice should fail)
            var updatePaidInvoice = new InvoiceDto { InvoiceId = _testInvoiceId, TotalAmount = 1000 };
            ProcessResult("Update Paid Invoice (Should Fail)", _invoiceService.UpdateInvoice(updatePaidInvoice));

            // Scenario F: Illegal State Reversion (Paid back to Issued should fail)
            ProcessResult("Change Paid Status back to [Issued] (Should Fail)",
                _invoiceService.UpdateStatus(_testInvoiceId, enInvoiceStatus.Issued));
        }

        static void Test_FinancialMetrics()
        {
            PrintSection("5. Financial Metrics & Logic Checks");

            // Checking total debt for a patient
            ProcessResult($"Check Patient Outstanding Balance (ID: {_targetPatientId})",
                _invoiceService.GetPatientOutstandingBalance(_targetPatientId));

            // Revenue calculation based on paid status
            ProcessResult("Calculate Total Revenue (Today)",
                _invoiceService.GetTotalRevenue(DateTime.Today, DateTime.Now));

            // Technical logic check
            var exists = _invoiceService.CheckInvoiceExists(_targetAppointmentId);
            PrintResult($"Check Invoice Existence Logic for Appoint. {_targetAppointmentId}: {(exists.Data > 0 ? "Exists" : "None")}", false);
        }

        #endregion

        #region Output Helpers

        static void ProcessResult<T>(string actionName, ServiceResult<T, enInvoiceResult> result)
        {
            Console.Write($"{actionName,-45} : ");
            if (result.IsSuccess)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"[SUCCESS] -> Result: {result.Result}");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[FAILED]  -> Result: {result.Result}");

                if (result.ValidationErrors != null && result.ValidationErrors.Any())
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("    Validation Errors:");
                    foreach (var error in result.ValidationErrors)
                        Console.WriteLine($"    - {error}");
                }
            }
            Console.ResetColor();
        }

        static void PrintHeader(string title)
        {
            Console.WriteLine("\n" + new string('=', 75));
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($" {title} ");
            Console.ResetColor();
            Console.WriteLine(new string('=', 75) + "\n");
        }

        static void PrintSection(string sectionName)
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine($"\n>>> {sectionName}");
            Console.ResetColor();
            Console.WriteLine(new string('-', 60));
        }

        static void PrintResult(string message, bool isWarning)
        {
            Console.ForegroundColor = isWarning ? ConsoleColor.Yellow : ConsoleColor.White;
            Console.WriteLine(message);
            Console.ResetColor();
        }

        #endregion
    }
}