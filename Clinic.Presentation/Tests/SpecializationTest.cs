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
    /// SpecializationTest Class: Executes a complete lifecycle test for medical specializations.
    /// The test flow ensures the integrity of the clinic's structural data:
    /// 1. Creation & Validation: Tests successful creation, duplicate name prevention, and length validation.
    /// 2. Data Retrieval: Verifies fetching single records, full lists, and specialized statistics (Doctor counts).
    /// 3. Updates: Validates the ability to modify department names and descriptions.
    /// 4. Business Logic: Tests search functionality, existence checks, and referential integrity (CanDelete check).
    /// 5. Deletion & Cleanup: Finalizes by removing test data to maintain database cleanliness.
    /// </summary>
    public class SpecializationTest
    {
        private static clsSpecializationService _specService = new clsSpecializationService();
        private static int _testSpecId = 0;

        public static void RunTests()
        {
            Console.Title = "Clinic System - Specialization Service Comprehensive Test";
            PrintHeader("STARTING COMPREHENSIVE SPECIALIZATION SERVICE TEST");

            // Phase 1: Test Create operations and input validation
            Test_AddSpecialization();

            if (_testSpecId > 0)
            {
                // Phase 2: Test Read operations and complex stats retrieval
                Test_DataRetrieval();

                // Phase 3: Test Update operations
                Test_UpdateSpecialization();

                // Phase 4: Test Search engine and business rules (e.g., Doctors count)
                Test_SearchAndLogic();

                // Phase 5: Test Delete operations and cleanup
                Test_DeleteSpecialization();
            }
            else
            {
                PrintResult("!! Aborting further tests because Specialization Creation failed.", true);
            }

            PrintHeader("SPECIALIZATION TESTING COMPLETED");
            Console.WriteLine("Press any key to return to main menu...");
            Console.ReadKey();
        }

        #region Test Methods

        static void Test_AddSpecialization()
        {
            PrintSection("1. Specialization Creation & Validation");

            // Scenario A: Valid Successful Creation
            string uniqueName = "Cardiology_" + DateTime.Now.Ticks.ToString().Substring(10);
            var specDto = new SpecializationDto
            {
                SpecializationName = uniqueName,
                SpecializationDescription = "Study and treatment of heart disorders and diseases."
            };

            var result = _specService.AddSpecialization(specDto);
            if (result.IsSuccess) _testSpecId = result.Data;
            ProcessResult("Add New Specialization (Valid)", result);

            // Scenario B: Validation Failure (Name too short)
            var invalidDto = new SpecializationDto { SpecializationName = "He", SpecializationDescription = "Test" };
            ProcessResult("Add Specialization (Invalid Name)", _specService.AddSpecialization(invalidDto));

            // Scenario C: Constraint Failure (Duplicate Name)
            ProcessResult("Add Specialization (Duplicate Name)", _specService.AddSpecialization(specDto));
        }

        static void Test_DataRetrieval()
        {
            PrintSection("2. Data Retrieval Methods");

            ProcessResult("Get Specialization By ID", _specService.GetById(_testSpecId));
            ProcessResult("Get All Specializations", _specService.GetAllSpecializations());
            ProcessResult("Get Specializations With Stats (Doctor Counts)", _specService.GetAllSpecializationsWithStats());
        }

        static void Test_UpdateSpecialization()
        {
            PrintSection("3. Update Specialization Information");

            var updateDto = new SpecializationDto
            {
                SpecializationId = _testSpecId,
                SpecializationName = "Updated Heart Dept_" + DateTime.Now.Second,
                SpecializationDescription = "Updated description for cardiology department."
            };

            ProcessResult("Update Full Specialization Info", _specService.UpdateSpecialization(updateDto));
        }

        static void Test_SearchAndLogic()
        {
            PrintSection("4. Search, Exists & Statistics");

            ProcessResult("Search Specialization (Keyword: Heart)", _specService.Search("Heart"));

            int exists = _specService.SpecializationExists("NonExistentSpec");
            PrintResult($"Check If 'NonExistentSpec' Exists (ID returned): {exists}", false);

            int doctorCount = _specService.GetDoctorsCount(_testSpecId);
            PrintResult($"Doctors currently in this specialization: {doctorCount}", false);

            bool canDelete = _specService.CanDelete(_testSpecId);
            PrintResult($"Can Delete This Specialization? {canDelete}", false);
        }

        static void Test_DeleteSpecialization()
        {
            PrintSection("5. Deletion & Integrity Logic");

            // Clean up the created specialization. Should succeed if no doctors are linked.
            ProcessResult("Delete Created Specialization", _specService.DeleteSpecialization(_testSpecId));

            // Verify that the record no longer exists
            var result = _specService.GetById(_testSpecId);
            ProcessResult("Verify Deletion (Get By ID - Should Fail)", result);
        }

        #endregion

        #region Professional Output Helpers

        static void ProcessResult<T>(string actionName, ServiceResult<T, enSpecializationResult> result)
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
                    {
                        Console.WriteLine($"    - {error}");
                    }
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