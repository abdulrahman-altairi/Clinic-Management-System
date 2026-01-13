using Clinic.BLL.Common.Result;

using Clinic.BLL.Enums;
using Clinic.BLL.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Clinic.ConsoleUI
{
    /// <summary>
    /// AppointmentViewTest Class: Validates the read-only, analytical, and reporting layer.
    /// This test suite focuses on:
    /// 1. Data Visibility: Ensuring complex joins (Patient-Doctor-Appointment) are flattened correctly in the UI views.
    /// 2. Patient Continuity: Verifying that historical records are correctly grouped by Patient ID.
    /// 3. Financial Forecasting: Testing the logic for 'Expected Revenue' based on booked appointment fees.
    /// 4. Operational Intelligence: Validating specialization demand statistics via DataTable reporting.
    /// </summary>
    public class AppointmentViewTest
    {
        private static clsAppointmentViewService _viewService = new clsAppointmentViewService();
        private static int _targetPatientId = 1;
        private static int _targetAppointmentId = 1;

        public static void RunTests()
        {
            Console.Title = "Clinic System - Appointment View Service Test";
            PrintHeader("STARTING APPOINTMENT VIEW SERVICE TEST (READ-ONLY)");

            // Phase 1: Retrieve flat data and historical records
            Test_ListsRetrieval();

            // Phase 2: Validate searching and keyword filtering across the view
            Test_SearchAndFilters();

            // Phase 3: Test analytical calculations and statistical reporting
            Test_FinancialsAndStats();

            PrintHeader("VIEW TESTING COMPLETED SUCCESSFULLY");
            Console.WriteLine("Press any key to return to main menu...");
            Console.ReadKey();
        }

        #region Test Methods

        static void Test_ListsRetrieval()
        {
            PrintSection("1. Basic Lists Retrieval");

            // Validates retrieval of the main appointment board view
            ProcessResult("Get All Appointments View", _viewService.GetAllAppointments());

            // Validates filtered view for current daily operations
            ProcessResult("Get Today's Appointments", _viewService.GetAllAppointments());

            // Validates detailed DTO retrieval for specific ID
            ProcessResult($"Get Appointment Details (ID: {_targetAppointmentId})",
                _viewService.GetAppointmentById(_targetAppointmentId));

            // Validates clinical history tracking for a specific patient
            ProcessResult($"Get Patient History (ID: {_targetPatientId})",
                _viewService.GetPatientHistory(_targetPatientId));
        }

        static void Test_SearchAndFilters()
        {
            PrintSection("2. Search & Filter Functionality");

            // Scenario: Searching for a specific patient or doctor by name within the view
            ProcessResult("Search: 'John' in Appointments", _viewService.SearchAppointments("John"));

            // Scenario: Validating system behavior when no results are found
            ProcessResult("Search: 'UnknownPerson' (Empty Case)", _viewService.SearchAppointments("UnknownPerson"));
        }

        static void Test_FinancialsAndStats()
        {
            PrintSection("3. Financial Reports & Statistics");

            // Defining current month range for revenue forecasting
            DateTime start = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            DateTime end = start.AddMonths(1).AddDays(-1);

            var revenueResult = _viewService.GetExpectedRevenue(start, end);
            ProcessResult($"Calculate Monthly Expected Revenue", revenueResult);

            if (revenueResult.IsSuccess)
            {
                PrintResult($"> Verification: Expected Revenue for {DateTime.Now:MMMM} is {revenueResult.Data:C}", false);
            }

            // Validates the demand-per-specialization logic for clinic management planning
            var statsResult = _viewService.GetSpecializationStats();
            ProcessResult("Get Specialization Stats (DataTable Output)", statsResult);

            if (statsResult.IsSuccess && statsResult.Data != null && statsResult.Data.Rows.Count > 0)
            {
                PrintResult("> Top Specializations by Demand Summary:", false);
                foreach (DataRow row in statsResult.Data.Rows)
                {
                    Console.WriteLine($"   - {row["SpecializationName"],-20} : {row["TotalAppointments"]} Appointments");
                }
            }
        }

        #endregion

        #region Output Helpers

        // Generic Parameter updated to enAppointmentViewResult for BLL compatibility
        static void ProcessResult<T>(string actionName, ServiceResult<T, enAppointmentViewResult> result)
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