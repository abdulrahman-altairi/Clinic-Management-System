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
    /// DoctorTest Class: Executes a full lifecycle test for Doctor entities.
    /// This test is critical as it validates the 'Table-per-Type' inheritance logic:
    /// 1. Registration: Tests the transactional creation of both a Person and a Doctor record.
    /// 2. Data Retrieval: Verifies the joining logic for fetching full profiles.
    /// 3. Personal Updates: Specifically targets the shared base table (People).
    /// 4. Professional Updates: Specifically targets the specialized table (Doctors).
    /// 5. Analytics: Tests count-per-specialization and keyword searching.
    /// </summary>
    class DoctorTest
    {
        private static clsDoctorService _doctorService = new clsDoctorService();
        private static int _testDoctorId = 0;

        public static void RunTests()
        {
            Console.Title = "Clinic System - Doctor Service Comprehensive Test";
            PrintHeader("STARTING COMPREHENSIVE DOCTOR SERVICE TEST");

            // Phase 1: Registration (Base + Specialized record creation)
            Test_RegisterDoctor();

            if (_testDoctorId > 0)
            {
                // Phase 2: Retrieval and Paging
                Test_DataRetrieval();

                // Phase 3: Base Identity Updates
                Test_UpdatePersonalInformation();

                // Phase 4: Professional Metadata Updates
                Test_UpdateProfessionalInformation();

                // Phase 5: Statistics and Search Logic
                Test_StatisticsAndSearch();
            }
            else
            {
                PrintResult("!! Aborting further tests because Doctor Registration failed.", true);
            }

            PrintHeader("DOCTOR TESTING COMPLETED");
            Console.WriteLine("Press any key to return to main menu...");
            Console.ReadKey();
        }

        #region Test Methods

        static void Test_RegisterDoctor()
        {
            PrintSection("1. Doctor Registration & Profile Creation");

            // Use timestamp to ensure unique Email/Contact for every test run
            string timeStamp = DateTime.Now.ToString("yyyyMMddHHmmss");

            var doctorDto = new DoctorDto
            {
                FirstName = "Omar",
                LastName = "Khattab",
                Email = $"dr.omar.{timeStamp}@clinic.com",
                ContactNumber = "71" + timeStamp.Substring(timeStamp.Length - 7),
                Gender = enGender.Male,
                DateOfBirth = new DateTime(1980, 5, 15),
                Address = "Diplomatic Quarter, Sana'a",
                SpecializationId = 1,
                Bio = "Senior consultant with 15 years of experience in cardiology.",
                ConsultationFee = 50.00m,
                IsAvailable = true
            };

            var result = _doctorService.RegisterDoctor(doctorDto);

            if (result.IsSuccess)
                _testDoctorId = result.Data;

            ProcessResult("Register New Doctor (Transaction)", result);
        }

        static void Test_DataRetrieval()
        {
            PrintSection("2. Data Retrieval Methods");

            ProcessResult("Get Doctor By ID", _doctorService.GetDoctorById(_testDoctorId));
            ProcessResult("Get All Doctors List", _doctorService.GetAllDoctors());
            ProcessResult("Get Available Doctors", _doctorService.GetAvailableDoctors());
            ProcessResult("Get Paged Doctors (Page 1, Size 10)", _doctorService.GetDoctorsPaged(1, 10));
        }

        static void Test_UpdatePersonalInformation()
        {
            PrintSection("3. Update Personal Information (Shared Person Table)");

            ProcessResult("Update First Name", _doctorService.UpdateFirstName(_testDoctorId, "Omar Updated"));
            ProcessResult("Update Last Name", _doctorService.UpdateLastName(_testDoctorId, "Khattab Updated"));
            ProcessResult("Update Email", _doctorService.UpdateEmail(_testDoctorId, $"new.email.{DateTime.Now.Ticks}@test.com"));
            ProcessResult("Update Contact Number", _doctorService.UpdateContactNumber(_testDoctorId, $"7009{DateTime.Now.Second}98877"));
            ProcessResult("Update Address", _doctorService.UpdateAddress(_testDoctorId, "Hadda Street, Tower 5"));
            ProcessResult("Update Date of Birth", _doctorService.UpdateDateOfBirth(_testDoctorId, new DateTime(1982, 6, 20)));
            ProcessResult("Update Full Contact Info", _doctorService.UpdateContactInfo(_testDoctorId, $"7112{DateTime.Now.Second}344", $"contact.{DateTime.Now.Ticks}@test.com"));
        }

        static void Test_UpdateProfessionalInformation()
        {
            PrintSection("4. Update Professional/Doctor Specific Information");

            // Note: Required fields are included to pass DTO validation rules
            var updateDto = new DoctorDto
            {
                DoctorId = _testDoctorId,
                FirstName = "Omar",
                LastName = "Saleh",
                Gender = Clinic.Contracts.Enums.enGender.Male,
                Email = "omar@clinic.com",
                ContactNumber = "1234567890",
                SpecializationId = 2,
                Bio = "Updated Bio: Expert in Interventional Cardiology.",
                ConsultationFee = 75.50m
            };

            ProcessResult("Update Full Profile (Specialization/Bio/Fee)", _doctorService.UpdateFullProfile(updateDto));

            // Testing granular update methods
            ProcessResult("Change Specialization Only", _doctorService.ChangeSpecialization(_testDoctorId, 5));
            ProcessResult("Update Consultation Fee", _doctorService.UpdateConsultationFee(_testDoctorId, 100.00m));
            ProcessResult("Set Availability (Offline)", _doctorService.SetAvailability(_testDoctorId, false));
            ProcessResult("Set Availability (Online)", _doctorService.SetAvailability(_testDoctorId, true));
        }

        static void Test_StatisticsAndSearch()
        {
            PrintSection("5. Search & Statistical Methods");

            ProcessResult("Search Doctors (Keyword: Omar)", _doctorService.SearchDoctors("Omar"));
            ProcessResult("Get Total Doctors Count", _doctorService.GetTotalDoctorsCount());
            ProcessResult("Get Count Per Specialization", _doctorService.GetCountPerSpecialization());
        }

        #endregion

        #region Output Helpers

        static void ProcessResult<T>(string actionName, ServiceResult<T, enDoctorResult> result)
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