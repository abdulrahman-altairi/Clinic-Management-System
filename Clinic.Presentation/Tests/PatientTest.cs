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
    /// PatientTest Class: Conducts a comprehensive lifecycle test for the Patient Management System.
    /// The test flow validates the multi-table architecture (Person and Patient tables):
    /// 1. Registration: Creates a new patient with unique insurance and contact details.
    /// 2. Data Retrieval: Tests various fetch methods including ID lookup, full lists, search, and pagination.
    /// 3. Personal Info Updates: Validates modifications to the base 'Person' entity (Name, Contact, Address).
    /// 4. Medical Info Updates: Validates modifications specific to the 'Patient' entity (Insurance, Emergency Contacts).
    /// 5. Lifecycle Management: Tests the Soft Delete mechanism and verifies record state after deletion.
    /// </summary>
    class PatientTest
    {
        private static clsPatientService _patientService = new clsPatientService();
        private static int _testPatientId = 0;

        public static void RunTests()
        {
            Console.Title = "Clinic System - Patient Service Comprehensive Test";
            PrintHeader("STARTING COMPREHENSIVE PATIENT SERVICE TEST");

            // Phase 1: Registration and identity creation
            Test_RegisterPatient();

            if (_testPatientId > 0)
            {
                // Phase 2: Data visibility and searchability
                Test_GetPatients();

                // Phase 3: Base entity (Person) data integrity
                Test_UpdatePersonalInformation();

                // Phase 4: Extended entity (Patient) specific data
                Test_UpdateMedicalInformation();

                // Phase 5: Logical deletion and cleanup verification
                Test_SoftDelete();
            }
            else
            {
                PrintResult("!! Aborting further tests because Patient Registration failed.", true);
            }

            PrintHeader("PATIENT TESTING COMPLETED");
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }

        #region Test Methods

        static void Test_RegisterPatient()
        {
            PrintSection("1. Patient Registration");

            // Using timestamps and unique suffixes to ensure no primary key or unique constraint violations
            string timeStamp = DateTime.Now.ToString("yyyyMMddHHmmss");
            string uniqueSuffix = Guid.NewGuid().ToString().Substring(0, 4);

            var patientDto = new PatientDto
            {
                FirstName = "Saleh",
                LastName = "Mahmoud",
                Email = $"saleh.{timeStamp}@clinic.com",
                ContactNumber = "77" + timeStamp.Substring(timeStamp.Length - 7),
                DateOfBirth = new DateTime(1988, 10, 20),
                Gender = enGender.Male,
                Address = "Al-Siteen Street, Sana'a",
                InsuranceProvider = "Yemen Insurance Co",
                InsurancePolicyNumber = "POL-" + timeStamp + "-" + uniqueSuffix,
                EmergencyContactName = "Ali Mahmoud",
                EmergencyContactPhone = "771234567"
            };

            var result = _patientService.RegisterPatient(patientDto);

            if (result.IsSuccess)
                _testPatientId = result.Data;

            ProcessResult("Register New Patient", result);
        }

        static void Test_GetPatients()
        {
            PrintSection("2. Data Retrieval");

            ProcessResult("Get Patient By ID", _patientService.GetPatientById(_testPatientId));
            ProcessResult("Get All Patients List", _patientService.GetAllPatients());
            ProcessResult("Search Patients (Keyword: Saleh)", _patientService.SearchPatients("Saleh"));
            ProcessResult("Get Paged Patients (Page 1, Size 5)", _patientService.GetPatientsPaged(1, 5));
        }

        static void Test_UpdatePersonalInformation()
        {
            PrintSection("3. Update Personal Information (Person Table)");

            ProcessResult("Update First Name", _patientService.UpdateFirstName(_testPatientId, "Saleh Updated"));
            ProcessResult("Update Last Name", _patientService.UpdateLastName(_testPatientId, "Mahmoud Updated"));
            ProcessResult("Update Gender", _patientService.UpdateGender(_testPatientId, enGender.Male));
            ProcessResult("Update Email", _patientService.UpdateEmail(_testPatientId, "updated." + DateTime.Now.Ticks + "@mail.com"));
            ProcessResult("Update Contact Number", _patientService.UpdateContactNumber(_testPatientId, "775544332"));
            ProcessResult("Update Address", _patientService.UpdateAddress(_testPatientId, "Hadda Street, Sana'a"));
            ProcessResult("Update Date of Birth", _patientService.UpdateDateOfBirth(_testPatientId, new DateTime(1985, 1, 1)));
            ProcessResult("Update Full Contact Info", _patientService.UpdateContactInfo(_testPatientId, "772" + DateTime.Now.Second + "234", "fullcon" + DateTime.Now.Ticks + "tact@test.com"));
        }

        static void Test_UpdateMedicalInformation()
        {
            PrintSection("4. Update Medical/Insurance Information (Patient Table)");

            ProcessResult("Update Insurance Info", _patientService.UpdateInsuranceInfo(_testPatientId, "Global " + DateTime.Now.Second + " Health", "G-99" + DateTime.Now.Second + "77"));
            ProcessResult("Update Emergency Contact", _patientService.UpdateEmergencyContact(_testPatientId, "Brother Ali", "770000111"));
        }

        static void Test_SoftDelete()
        {
            PrintSection("5. Soft Delete Test");

            // Deleting the patient logically (IsDeleted = true)
            ProcessResult("Soft Delete Patient (Person Table)", _patientService.DeletePatient(_testPatientId));

            // Verify if the patient is still accessible via standard retrieval methods
            var checkResult = _patientService.GetPatientById(_testPatientId);
            PrintResult($"Verification - Get Deleted Patient Status: {checkResult.Result}", false);
        }

        #endregion

        #region Output Helpers

        static void ProcessResult<T>(string actionName, ServiceResult<T, enPatientResult> result)
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
            Console.WriteLine("\n" + new string('=', 70));
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($" {title} ");
            Console.ResetColor();
            Console.WriteLine(new string('=', 70) + "\n");
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