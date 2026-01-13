using System;
using Clinic.ConsoleUI;

namespace Clinic.ConsoleUI
{
    class Program
    {
        static void Main(string[] args)
        {
            bool exit = false;

            while (!exit)
            {
                Console.Clear();
                PrintHeader();

                // Column 1: Core Services
                Console.WriteLine(" [1]  Doctor Service         [7]  Patient Service");
                Console.WriteLine(" [2]  Appointment Service    [8]  Medical Record Service");
                Console.WriteLine(" [3]  Invoice Service        [9]  Payment Service");
                Console.WriteLine(" [4]  Invoice Item Service   [10] Specialization Service");
                Console.WriteLine(" [5]  User Service           [11] Doctor View (Read-Only)");
                Console.WriteLine(" [6]  Appointment View       [12] Patient View (Read-Only)");

                Console.WriteLine("\n-----------------------------------------------------------");
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine(" [A]  RUN ALL 12 TESTS (Full System Integrity Check)");
                Console.ResetColor();
                Console.WriteLine(" [0]  Exit");
                Console.WriteLine("-----------------------------------------------------------");
                Console.Write("\nSelect Option: ");

                string choice = Console.ReadLine()?.ToUpper();

                switch (choice)
                {
                    case "1": DoctorTest.RunTests(); break;
                    case "2": AppointmentTest.RunTests(); break;
                    case "3": InvoiceTest.RunTests(); break;
                    case "4": InvoiceItemTest.RunTests(); break;
                    case "5": UserTest.RunTests(); break;
                    case "6": AppointmentViewTest.RunTests(); break;
                    case "7": PatientTest.RunTests(); break;
                    case "8": MedicalRecordTest.RunTests(); break;
                    case "9": PaymentTest.RunTests(); break;
                    case "10": SpecializationTest.RunTests(); break;
                    case "11": DoctorViewTest.RunTests(); break;
                    case "12": PatientViewTest.RunTests(); break;
                    case "A": RunAllTests(); break;
                    case "0": exit = true; break;
                    default:
                        Console.WriteLine("\nInvalid Selection. Press any key to try again...");
                        Console.ReadKey();
                        break;
                }
            }
        }

        static void RunAllTests()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(">>> STARTING GLOBAL SYSTEM TEST (12 MODULES) <<<");
            Console.ResetColor();

            // Executing all suites sequentially
            DoctorTest.RunTests();
            AppointmentTest.RunTests();
            InvoiceTest.RunTests();
            InvoiceItemTest.RunTests();
            UserTest.RunTests();
            AppointmentViewTest.RunTests();
            PatientTest.RunTests();
            MedicalRecordTest.RunTests();
            PaymentTest.RunTests();
            SpecializationTest.RunTests();
            DoctorViewTest.RunTests();
            PatientViewTest.RunTests();

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\n===========================================================");
            Console.WriteLine("GLOBAL TEST COMPLETE: ALL MODULES EXECUTED");
            Console.WriteLine("===========================================================");
            Console.ResetColor();
            Console.WriteLine("Press any key to return to menu...");
            Console.ReadKey();
        }

        static void PrintHeader()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("===========================================================");
            Console.WriteLine("              CLINIC SYSTEM MANAGEMENT - TEST SUITE        ");
            Console.WriteLine("===========================================================");
            Console.ResetColor();
        }
    }
}