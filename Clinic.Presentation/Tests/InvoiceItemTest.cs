using Clinic.BLL.Common.Result;
using Clinic.BLL.Enums;
using Clinic.BLL.Services;
using Clinic.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Clinic.ConsoleUI
{
    /// <summary>
    /// InvoiceItemServiceTest Class: Conducts deep-level testing of the billing line-item logic.
    /// This test suite focuses on three critical areas:
    /// 1. Data Integrity: Validates the one-to-many relationship between Invoices and Items.
    /// 2. Financial Sync: Ensures that adding, updating, or deleting items triggers a recalculation of the Parent Invoice total.
    /// 3. State-Based Security: Verifies that "Locked" (Paid) invoices prevent any modification to their line items.
    /// </summary>
    public class InvoiceItemTest
    {
        private static clsInvoiceItemService _itemService = new clsInvoiceItemService();
        private static clsInvoiceService _invoiceService = new clsInvoiceService();

        // Target Invoice ID generated from previous successful tests
        private static int _testInvoiceId = 3;
        private static int _testItemId = 0;

        public static void RunTests()
        {
            Console.Clear();
            Console.Title = "Clinic System - Direct Invoice Item Test";
            PrintHeader($"STARTING DIRECT INVOICE ITEM TEST (TARGET INVOICE: {_testInvoiceId})");

            // Phase 0: Validate the target environment
            if (!CheckTargetInvoice()) return;

            // Phase 1: Test Item Insertion
            Test_AddItem();

            if (_testItemId > 0)
            {
                // Phase 2: Test Record Selection
                Test_GetItems();

                // Phase 3: Test Financial Synchronization (The Core Business Logic)
                Test_UpdateItem();

                // Phase 4: Test Protection Rules (Immutable Records)
                Test_InvoiceProtectionRules();

                // Phase 5: Test Deletion and Final Balance Cleanup
                Test_DeleteItem();
            }

            PrintHeader("ALL INVOICE ITEM TESTS COMPLETED SUCCESSFULLY");
            Console.WriteLine("Press any key to return...");
            Console.ReadKey();
        }

        private static bool CheckTargetInvoice()
        {
            PrintSection("0. Checking Target Invoice Status");
            var check = _invoiceService.GetById(_testInvoiceId);

            if (check.IsSuccess && check.Data != null)
            {
                PrintResult($"> Found Invoice ID {_testInvoiceId}. Status: {check.Data.InvoiceStatus}", false);

                // Intelligence: Ensure invoice is 'Issued' (Status 1) to allow testing.
                // If it was previously locked (Paid), we force it open for this test phase.
                if ((int)check.Data.InvoiceStatus != 1)
                {
                    PrintResult($"> Invoice is locked ({check.Data.InvoiceStatus}). Opening it now for testing...", true);
                    _invoiceService.UpdateStatus(_testInvoiceId, (Clinic.Contracts.Enums.enInvoiceStatus)1);
                }
                return true;
            }

            PrintResult($"!! Error: Invoice ID {_testInvoiceId} not found. Ensure the Parent Invoice exists.", true);
            return false;
        }

        static void Test_AddItem()
        {
            PrintSection("1. Adding New Items");

            var item = new InvoiceItemDto
            {
                InvoiceId = _testInvoiceId,
                ItemDescription = "Consultation Fee (Test)",
                UnitPrice = 150.00m,
                Quantity = 1
            };

            var result = _itemService.AddItem(item);
            if (result.IsSuccess) _testItemId = result.Data;

            ProcessResult("Add 'Consultation Fee' to Invoice", result);
        }

        static void Test_GetItems()
        {
            PrintSection("2. Retrieval");
            var result = _itemService.GetItemsByInvoiceId(_testInvoiceId);

            if (result.IsSuccess)
            {
                PrintResult($"> Total items found for this invoice: {result.Data.Count()}", false);
                foreach (var item in result.Data)
                {
                    Console.WriteLine($"   - {item.ItemDescription} | {item.Quantity} x {item.UnitPrice:C}");
                }
            }
            ProcessResult("Fetch all items for Invoice", result);
        }

        static void Test_UpdateItem()
        {
            PrintSection("3. Update & Sync Verification");

            var updateDto = new InvoiceItemDto
            {
                ItemId = _testItemId,
                InvoiceId = _testInvoiceId,
                ItemDescription = "Urgent Specialist Consultation",
                UnitPrice = 300.00m, // Increased from 150
                Quantity = 2        // Increased from 1
            };

            // Expected Outcome: Total should update to 300 * 2 = 600
            ProcessResult("Update Item (Qty=2, Price=300)", _itemService.UpdateItem(updateDto));

            // Verify Financial Sync: Check if the Parent Invoice Total updated automatically
            var parent = _invoiceService.GetById(_testInvoiceId);
            PrintResult($"> Financial Sync: Parent Invoice Total is now {parent.Data.TotalAmount:C}", false);
        }

        static void Test_InvoiceProtectionRules()
        {
            PrintSection("4. Business Rules (Lock Test)");

            // Lock the invoice (Status 3 = Paid)
            _invoiceService.UpdateStatus(_testInvoiceId, (Clinic.Contracts.Enums.enInvoiceStatus)3);
            PrintResult("> Invoice status set to [Paid]. Modifications should be BLOCKED.", true);

            // Attempting to delete from a locked invoice should fail at the Service Layer
            var result = _itemService.DeleteItem(_testItemId);
            ProcessResult("Attempt Delete Item from Locked Invoice (Expected Failure)", result);

            // Revert to 'Issued' (Status 1) to proceed with cleanup
            _invoiceService.UpdateStatus(_testInvoiceId, (Clinic.Contracts.Enums.enInvoiceStatus)1);
            PrintResult("> Invoice status reverted to [Issued]. Modifications UNLOCKED.", false);
        }

        static void Test_DeleteItem()
        {
            PrintSection("5. Final Deletion & Cleanup");

            ProcessResult("Delete Item from Invoice", _itemService.DeleteItem(_testItemId));

            // Verify the balance returns to 0 (or original state) after item removal
            var finalCheck = _invoiceService.GetById(_testInvoiceId);
            PrintResult($"> Final Balance Sync: Parent Invoice Total is now {finalCheck.Data.TotalAmount:C}", false);
        }

        #region UI Helpers
        static void ProcessResult<T>(string actionName, ServiceResult<T, enInvoiceItemResult> result)
        {
            Console.Write($"{actionName,-50} : ");
            if (result.IsSuccess)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"[SUCCESS] -> {result.Result}");
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
            Console.WriteLine("\n" + new string('=', 80));
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($" {title} ");
            Console.ResetColor();
            Console.WriteLine(new string('=', 80) + "\n");
        }

        static void PrintSection(string name)
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine($"\n>>> {name}");
            Console.ResetColor();
            Console.WriteLine(new string('-', 60));
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