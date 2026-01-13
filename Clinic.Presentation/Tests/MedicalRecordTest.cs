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
    /// MedicalRecordTest Class: Implements a "Smart" testing lifecycle for patient clinical records.
    /// This test sequence ensures the integrity of medical data linked to appointments:
    /// 1. Smart Preparation: Automatically checks for existing records to prevent unique constraint conflicts.
    /// 2. Creation & Validation: Tests record insertion, empty-field validation, and duplicate prevention.
    /// 3. Data Retrieval: Verifies record lookups by ID, Appointment ID, and full Patient clinical history.
    /// 4. Updates: Tests the ability to modify diagnoses and prescriptions.
    /// 5. Search & Logic: Validates keyword searching and date-range filtering for clinical reports.
    /// 6. Deletion: Ensures records can be safely removed and verifies their absence post-deletion.
    /// </summary>
    public class MedicalRecordTest
    {
        private static clsMedicalRecordService _recordService = new clsMedicalRecordService();
        private static int _testRecordId = 0;
        private static int _targetAppointmentId = 0;
        private static int _targetPatientId = 0;

        public static void RunTests()
        {
            Console.Title = "Clinic System - Smart Medical Record Service Test";
            PrintHeader("STARTING SMART MEDICAL RECORD SERVICE TEST");

            // Phase 0: Smart Environment Preparation
            if (!PrepareTestData())
            {
                PrintResult("!! Aborting: No Appointments found in database to perform tests.", true);
                return;
            }

            // Phase 1: Test Create operations and business rule validation
            Test_AddMedicalRecord();

            if (_testRecordId > 0)
            {
                // Phase 2: Test Read operations and History tracking
                Test_DataRetrieval();

                // Phase 3: Test Update operations
                Test_UpdateMedicalRecord();

                // Phase 4: Test Search engine and Date filtering
                Test_SearchAndLogic();

                // Phase 5: Test Delete operations and cleanup
                Test_DeleteMedicalRecord();
            }
            else
            {
                PrintResult("!! Aborting further tests because Medical Record Creation failed.", true);
            }

            PrintHeader("MEDICAL RECORD TESTING COMPLETED");
            Console.WriteLine("Press any key to return to main menu...");
            Console.ReadKey();
        }

        private static bool PrepareTestData()
        {
            PrintSection("0. Smart Environment Preparation");

            // Targeting specific IDs for consistent testing environment
            _targetAppointmentId = 1;
            _targetPatientId = 3;

            Console.WriteLine($"Targeting Appointment ID: {_targetAppointmentId}");
            Console.WriteLine($"Targeting Patient ID: {_targetPatientId}");

            // Environment Intelligence: If an old record exists for this appointment, 
            // delete it to ensure the fresh creation test doesn't fail on duplicates.
            if (_recordService.RecordExistsForAppointment(_targetAppointmentId) > 0)
            {
                var existingRecord = _recordService.GetByAppointmentId(_targetAppointmentId);
                if (existingRecord.IsSuccess)
                {
                    _recordService.DeleteMedicalRecord(existingRecord.Data.RecordId);
                    PrintResult($"> Existing record for Appointment {_targetAppointmentId} deleted to prepare fresh test.", false);
                }
            }

            return _targetAppointmentId > 0;
        }

        #region Test Methods

        static void Test_AddMedicalRecord()
        {
            PrintSection("1. Medical Record Creation & Validation");

            // Scenario A: Valid Success
            var recordDto = new MedicalRecordDto
            {
                AppointmentId = _targetAppointmentId,
                Diagnosis = "Initial diagnosis: Chronic Migraine.",
                Prescription = "Panadol 500mg, Rest.",
                Notes = "Automated test record."
            };

            var result = _recordService.AddMedicalRecord(recordDto);
            if (result.IsSuccess) _testRecordId = result.Data;
            ProcessResult("Add New Medical Record (Valid)", result);

            // Scenario B: Validation Failure (Empty Diagnosis)
            var invalidDto = new MedicalRecordDto { AppointmentId = _targetAppointmentId, Diagnosis = "" };
            ProcessResult("Add Record (Invalid - Empty Diagnosis)", _recordService.AddMedicalRecord(invalidDto));

            // Scenario C: Constraint Failure (One Record per Appointment)
            ProcessResult("Add Record (Duplicate Appointment ID)", _recordService.AddMedicalRecord(recordDto));
        }

        static void Test_DataRetrieval()
        {
            PrintSection("2. Data Retrieval Methods");

            ProcessResult("Get Record By ID", _recordService.GetById(_testRecordId));
            ProcessResult("Get Record By Appointment ID", _recordService.GetByAppointmentId(_targetAppointmentId));

            // Fetches the entire history of the patient linked to the target appointment
            ProcessResult($"Get Patient History (Patient ID: {_targetPatientId})", _recordService.GetPatientHistory(_targetPatientId));

            ProcessResult("Get All Records (Paged: Page 1, Size 10)", _recordService.GetAllRecordsPaged(1, 10));
        }

        static void Test_UpdateMedicalRecord()
        {
            PrintSection("3. Update Medical Record Information");

            var updateDto = new MedicalRecordDto
            {
                RecordId = _testRecordId,
                AppointmentId = _targetAppointmentId,
                Diagnosis = "Updated Diagnosis: Migraine with Aura.",
                Prescription = "Sumatriptan 50mg",
                Notes = "Updated during automated test."
            };

            ProcessResult("Update Full Medical Record Info", _recordService.UpdateMedicalRecord(updateDto));
        }

        static void Test_SearchAndLogic()
        {
            PrintSection("4. Search, Filtering & Logic");

            ProcessResult("Search Records (Keyword: Migraine)", _recordService.Search("Migraine"));

            ProcessResult("Get Records By Date Range",
                _recordService.GetRecordsByDateRange(DateTime.Now.AddDays(-30), DateTime.Now.AddDays(1)));

            int exists = _recordService.RecordExistsForAppointment(_targetAppointmentId);
            PrintResult($"Check If Record Exists for Appointment {_targetAppointmentId}: {(exists > 0 ? "Yes" : "No")}", false);
        }

        static void Test_DeleteMedicalRecord()
        {
            PrintSection("5. Deletion & Verification");

            ProcessResult("Delete Created Medical Record", _recordService.DeleteMedicalRecord(_testRecordId));
            ProcessResult("Verify Deletion (Get By ID - Should Fail)", _recordService.GetById(_testRecordId));
        }

        #endregion

        #region Output Helpers

        static void ProcessResult<T>(string actionName, ServiceResult<T, enMedicalRecordResult> result)
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