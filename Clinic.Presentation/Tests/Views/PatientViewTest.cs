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
    /// PatientViewTest Class: Designed to validate the data retrieval layer for patient-related information.
    /// This test focuses on Read-Only operations and reporting:
    /// 1. Retrieval: Ensures lists of patients and specific detailed profiles are fetched correctly.
    /// 2. Filtering: Tests search logic by keywords and insurance provider filtering.
    /// 3. Statistics: Validates the aggregation logic (DataTable) used for clinic reports and insurance distribution.
    /// 4. Edge Cases: Tests scenarios where no data is found to ensure the Result Enums handle empty states gracefully.
    /// </summary>
    public class PatientViewTest
    {
        private static clsPatientViewService _patientService = new clsPatientViewService();
        private static int _targetPatientId = 1;
        private static string _targetInsuranceProvider = "Cigna";

        public static void RunTests()
        {
            Console.Title = "Clinic System - Patient View Service Test";
            PrintHeader("STARTING PATIENT VIEW SERVICE TEST (READ-ONLY)");

            // Phase 1: Basic Retrieval tests
            Test_PatientsRetrieval();

            // Phase 2: Search engine and filtering logic
            Test_SearchAndFilters();

            // Phase 3: Analytical data and reporting (Statistics)
            Test_InsuranceStats();

            PrintHeader("PATIENT VIEW TESTING COMPLETED SUCCESSFULLY");
            Console.WriteLine("Press any key to return to main menu...");
            Console.ReadKey();
        }

        #region Test Methods

        static void Test_PatientsRetrieval()
        {
            PrintSection("1. Patients List Retrieval");

            ProcessResult("Get All Patients List", _patientService.GetAllPatients());

            ProcessResult($"Get Patient Details (ID: {_targetPatientId})",
                _patientService.GetPatientById(_targetPatientId));

            ProcessResult("Get Patients With Emergency Contacts",
                _patientService.GetPatientsWithEmergencyContacts());
        }

        static void Test_SearchAndFilters()
        {
            PrintSection("2. Search & Insurance Filtering");

            // Scenario: Keyword search (Name or Phone)
            ProcessResult("Search: 'Smith' in Patients", _patientService.SearchPatients("Smith"));

            // Scenario: Filtering by specific insurance company
            ProcessResult($"Filter by Insurance: '{_targetInsuranceProvider}'",
                _patientService.GetPatientsByInsurance(_targetInsuranceProvider));

            // Scenario: Edge Case - Search for non-existent criteria
            ProcessResult("Search: '999999' (Expected: NoPatientsFound)", _patientService.SearchPatients("999999"));
        }

        static void Test_InsuranceStats()
        {
            PrintSection("3. Insurance Coverage Statistics");

            // Validates that the service correctly returns a DataTable for UI Grids
            var statsResult = _patientService.GetInsuranceStats();
            ProcessResult("Get Patients Count By Insurance (DataTable)", statsResult);

            if (statsResult.IsSuccess && statsResult.Data != null && statsResult.Data.Rows.Count > 0)
            {
                PrintResult("> Patient Distribution by Insurance Provider:", false);
                foreach (DataRow row in statsResult.Data.Rows)
                {
                    // Formatting the output for professional scannability
                    Console.WriteLine($"  - {row["Provider"],-25} : {row["PatientCount"]} Patients");
                }
            }
        }

        #endregion

        #region Output Helpers

        static void ProcessResult<T>(string actionName, ServiceResult<T, enPatientViewResult> result)
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