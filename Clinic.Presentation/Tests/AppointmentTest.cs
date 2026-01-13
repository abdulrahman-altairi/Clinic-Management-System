using Clinic.BLL.Common.Result;
using Clinic.BLL.Enums;
using Clinic.BLL.Services;
using Clinic.Contracts.Dtos;
using Clinic.Contracts.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Clinic.ConsoleUI
{
    /// <summary>
    /// AppointmentTest Class: Validates the core scheduling engine.
    /// This suite performs rigorous testing on:
    /// 1. Validation Logic: Ensuring appointments aren't booked in the past.
    /// 2. Conflict Management: Verifying that the system blocks double-booking for the same doctor.
    /// 3. Availability Check: Testing the logic that queries time-slot gaps.
    /// 4. Lifecycle Management: Testing the transition from Pending to Confirmed to Cancelled.
    /// </summary>
    public class AppointmentTest
    {
        private static clsAppointmentService _appointmentService = new clsAppointmentService();
        private static int _testAppointmentId = 0;
        private static int _targetPatientId = 1; // Pre-existing Patient
        private static int _targetDoctorId = 1;  // Pre-existing Doctor

        public static void RunTests()
        {
            Console.Title = "Clinic System - Appointment Service Professional Test";
            PrintHeader("STARTING SMART APPOINTMENT SERVICE TEST");

            // Phase 1: Test Booking, Validation, and the 'Doctor Busy' logic
            Test_BookAppointment();

            if (_testAppointmentId > 0)
            {
                // Phase 2: Test Data Retrieval & Slot Checks
                Test_DataRetrieval();

                // Phase 3: Test Status Transitions
                Test_UpdateStatus();

                // Phase 4: Test Cancellation Logic & Error Handling
                Test_CancelAppointment();
            }
            else
            {
                PrintResult("!! Aborting further tests because Appointment Creation failed.", true);
            }

            PrintHeader("APPOINTMENT TESTING COMPLETED");
            Console.WriteLine("Press any key to return to main menu...");
            Console.ReadKey();
        }

        #region Test Methods

        static void Test_BookAppointment()
        {
            PrintSection("1. Appointment Creation & Conflict Validation");

            // SCENARIO A: Valid Booking (Future Date)
            var appDto = new AppointmentCreateDto
            {
                PatientId = _targetPatientId,
                DoctorId = _targetDoctorId,
                AppointmentDate = DateTime.Now.AddDays(2),
                DurationMinutes = 30,
                ReasonForVisit = "Regular checkup for blood pressure.",
                CreatedBy = 1,
                Status = enStatus.Pending
            };

            var result = _appointmentService.BookAppointment(appDto);
            if (result.IsSuccess) _testAppointmentId = result.Data;
            ProcessResult("Book New Appointment (Valid Future Date)", result);

            // SCENARIO B: Invalid Booking (Past Date - Should fail BLL Validation)
            var pastDto = new AppointmentCreateDto
            {
                AppointmentDate = DateTime.Now.AddDays(-1),
                DoctorId = _targetDoctorId
            };
            ProcessResult("Book Appointment (Invalid - Past Date)", _appointmentService.BookAppointment(pastDto));

            // SCENARIO C: Conflict Validation (Double-booking the same Doctor)
            var conflictDto = new AppointmentCreateDto
            {
                PatientId = 2,
                DoctorId = _targetDoctorId,
                AppointmentDate = appDto.AppointmentDate, // Exact same time as Scenario A
                DurationMinutes = 30,
                ReasonForVisit = "Conflict Test",
                CreatedBy = 1
            };
            ProcessResult("Book Appointment (Conflict - Doctor Busy)", _appointmentService.BookAppointment(conflictDto));
        }

        static void Test_DataRetrieval()
        {
            PrintSection("2. Data Retrieval & Availability Checking");

            ProcessResult("Get All Appointments List", _appointmentService.GetAllAppointments());

            // Directly query the availability engine for a far-future date
            bool isAvailable = _appointmentService.IsSlotAvailable(_targetDoctorId, DateTime.Now.AddDays(10), 30);
            PrintResult($"> Doctor Availability Check (Next Week): {(isAvailable ? "Available" : "Busy")}", false);
        }

        static void Test_UpdateStatus()
        {
            PrintSection("3. Update Appointment Status");

            // Transitions the record to 'Confirmed'
            ProcessResult("Update Status to Confirmed",
                _appointmentService.UpdateStatus(_testAppointmentId, enStatus.Confirmed, 1));
        }

        static void Test_CancelAppointment()
        {
            PrintSection("4. Cancellation Logic & Security");

            // Valid Cancellation
            ProcessResult("Cancel Created Appointment", _appointmentService.CancelAppointment(_testAppointmentId, 1));

            // Failure Case: Handling non-existent records
            ProcessResult("Cancel Non-Existent Appointment", _appointmentService.CancelAppointment(9999, 1));
        }

        #endregion

        #region Output Helpers

        static void ProcessResult<T>(string actionName, ServiceResult<T, enAppointmentResult> result)
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