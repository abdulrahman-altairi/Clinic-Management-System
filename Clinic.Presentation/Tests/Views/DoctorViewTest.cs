using Clinic.BLL.Common.Result;
using Clinic.BLL.Enums;
using Clinic.BLL.Services;
using SmartClinic.Contracts;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Clinic.ConsoleUI
{
    /// <summary>
    /// DoctorViewTest Class: Focuses on the "Read-Only" and "Discovery" layer of doctor management.
    /// This test sequence ensures the following capabilities are functioning correctly:
    /// 1. Bulk Retrieval: Testing the fetching of all doctors and specific specialization lists.
    /// 2. Deep Search: Validating keyword searches across names, emails, and professional bios.
    /// 3. Fee Filtering: Ensuring the BLL correctly handles range-based price filtering for patient budgets.
    /// 4. Business Intelligence: Testing the generation of distribution statistics (Doctors per Specialization) 
    ///    using DataTable results for reporting.
    /// </summary>
    public class DoctorViewTest
    {
        private static clsDoctorViewService _doctorService = new clsDoctorViewService();
        private static int _targetDoctorId = 1;
        private static int _targetSpecializationId = 1;

        public static void RunTests()
        {
            Console.Title = "Clinic System - Doctor View Service Test";
            PrintHeader("STARTING DOCTOR VIEW SERVICE TEST (READ-ONLY)");

            // Phase 1: Test basic and specific retrieval operations
            Test_DoctorsRetrieval();

            // Phase 2: Test advanced lookup logic and financial filters
            Test_SearchAndFilters();

            // Phase 3: Test analytical distribution and statistical data
            Test_DistributionStats();

            PrintHeader("DOCTOR VIEW TESTING COMPLETED SUCCESSFULLY");
            Console.WriteLine("Press any key to return to main menu...");
            Console.ReadKey();
        }

        #region Test Methods

        static void Test_DoctorsRetrieval()
        {
            PrintSection("1. Doctors List Retrieval");

            // Validates that the full list is reachable
            ProcessResult("Get All Doctors Info", _doctorService.GetAllDoctors());

            // Validates fetching specific metadata for a single doctor
            ProcessResult($"Get Doctor Full Details (ID: {_targetDoctorId})",
                _doctorService.GetDoctorById(_targetDoctorId));

            // Validates the relationship between Specializations and Doctors
            ProcessResult($"Get Available Doctors by Specialization (ID: {_targetSpecializationId})",
                _doctorService.GetDoctorsBySpecialization(_targetSpecializationId));
        }

        static void Test_SearchAndFilters()
        {
            PrintSection("2. Search & Fee Filtering");

            // Scenario A: Keyword Search (checks logic for Partial Matches in Bio/Name)
            ProcessResult("Search Doctors: 'Cardiology'", _doctorService.SearchDoctors("Cardiology"));

            // Scenario B: Financial Range Filter
            decimal minFee = 50;
            decimal maxFee = 200;
            var feeResult = _doctorService.GetDoctorsByFeeRange(minFee, maxFee);
            ProcessResult($"Filter Doctors by Fee ({minFee:C} - {maxFee:C})", feeResult);

            if (feeResult.IsSuccess && feeResult.Data.Any())
            {
                PrintResult($"> Verification: Found {feeResult.Data.Count} doctors within the specified price range.", false);
            }
        }

        static void Test_DistributionStats()
        {
            PrintSection("3. Doctor Distribution Statistics");

            // Tests the reporting capability (useful for Dashboard metrics)
            var statsResult = _doctorService.GetDoctorDistributionStats();
            ProcessResult("Get Doctors per Specialization (DataTable Output)", statsResult);

            if (statsResult.IsSuccess && statsResult.Data != null && statsResult.Data.Rows.Count > 0)
            {
                PrintResult("> Doctors Distribution Summary:", false);
                foreach (DataRow row in statsResult.Data.Rows)
                {
                    // Formats the DataTable output for clean console viewing
                    Console.WriteLine($"  - {row["SpecializationName"],-20} : {row["DoctorCount"]} Doctors");
                }
            }
        }

        #endregion

        #region Output Helpers

        static void ProcessResult<T>(string actionName, ServiceResult<T, enDoctorViewResult> result)
        {
            Console.Write($"{actionName,-45} : ");
            if (result.IsSuccess)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"[SUCCESS] -> Status: {result.Result}");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[FAILED]  -> Status: {result.Result}");
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