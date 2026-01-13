using System;
using System.Linq;
using Clinic.BLL.Common.Result;
using Clinic.BLL.Results;
using Clinic.BLL.Services;
using Clinic.Contracts.Identity;
using Clinic.Contracts.Enums;

namespace Clinic.ConsoleUI
{
    /// <summary>
    /// UserTest Class: Performs a comprehensive automated test for the User Management System.
    /// The test flow follows a logical lifecycle:
    /// 1. Registration: Creates a unique user in the database.
    /// 2. Authentication: Validates login logic with correct and incorrect credentials.
    /// 3. Data Retrieval: Tests fetching users by ID, status, and pagination.
    /// 4. Identity Management: Tests sensitive updates like usernames, passwords, and roles.
    /// 5. Personal Information: Validates updates to profile data (Name, Email, Contact).
    /// 6. Lifecycle Control: Tests account deactivation, activation, and login restrictions.
    /// 7. Persistence: Tests soft-delete and restoration functionality.
    /// </summary>
    class UserTest
    {
        private static clsUserService _userService = new clsUserService();
        private static int _testUserId = 0;

        public static void RunTests()
        {
            Console.Title = "Clinic System - Professional User Service Test";
            PrintHeader("STARTING COMPREHENSIVE USER SERVICE TEST");

            // Phase 1: Create a user to perform subsequent tests on
            Test_RegisterUser();

            if (_testUserId > 0)
            {
                // Phase 2: Security & Authentication
                Test_Login();

                // Phase 3: Accessing Data
                Test_GetUsers();

                // Phase 4: Account Credentials
                Test_UpdateIdentityInfo();

                // Phase 5: Profile & Contact Data
                Test_UpdatePersonalInfo();

                // Phase 6: Operational Status
                Test_AccountStatus();

                // Phase 7: Data Integrity & Recovery
                Test_DeleteAndRestore();
            }
            else
            {
                PrintResult("Aborting further tests because Registration failed.", true);
            }

            PrintHeader("TESTING COMPLETED");
            Console.WriteLine("Press any key to return to main menu...");
            Console.ReadKey();
        }

        #region Test Methods

        static void Test_RegisterUser()
        {
            PrintSection("1. User Registration");

            var registerDto = new RegisterRequestDto
            {
                // Generate unique username and email using ticks to avoid duplication errors
                Username = "User" + DateTime.Now.Ticks.ToString().Substring(10),
                Password = "Password@123",
                FirstName = "Omar",
                LastName = "Ali",
                Email = "omar" + DateTime.Now.Ticks + "@test.com",
                ContactNumber = "777123456", // Matches standard regex validation
                Address = "Sana'a, Yemen",
                DateOfBirth = new DateTime(1995, 5, 15),
                Gender = enGender.Male,
                Role = enRole.Admin
            };

            var result = _userService.RegisterUser(registerDto);

            if (result.IsSuccess)
            {
                _testUserId = result.Data;
            }

            ProcessResult("Register New User", result);
        }

        static void Test_Login()
        {
            PrintSection("2. Authentication (Login)");

            Console.WriteLine($"Testing User ID: {_testUserId}");
            var registrationResult = _userService.GetUserById(_testUserId);

            if (!registrationResult.IsSuccess || registrationResult.Data == null)
            {
                PrintResult("!! Cannot proceed with Login test: User not found in database.", true);
                return;
            }

            var userData = registrationResult.Data;

            // Scenario A: Incorrect password
            var wrongPassResult = _userService.Login(userData.Username, "WrongPass");
            ProcessResult("Login with Wrong Password", wrongPassResult);

            // Scenario B: Correct credentials
            var successResult = _userService.Login(userData.Username, "Password@123");
            ProcessResult("Login with Correct Credentials", successResult);
        }

        static void Test_GetUsers()
        {
            PrintSection("3. Data Retrieval");
            ProcessResult("Get User By ID", _userService.GetUserById(_testUserId));
            ProcessResult("Get Active Users List", _userService.GetActiveUsers());
            ProcessResult("Get Paged Users (Page 1, Size 10)", _userService.GetUsersPaged(1, 10));
        }

        static void Test_UpdateIdentityInfo()
        {
            PrintSection("4. Update Identity Info");

            ProcessResult("Update Username", _userService.UpdateUsername(_testUserId, "NewUser_" + _testUserId));
            ProcessResult("Change Password", _userService.ChangePassword(_testUserId, "NewSecurePass123"));
            ProcessResult("Update User Role", _userService.UpdateUserRole(_testUserId, enRole.Doctor));
        }

        static void Test_UpdatePersonalInfo()
        {
            PrintSection("5. Update Personal Info");

            ProcessResult("Update First Name", _userService.UpdateFirstName(_testUserId, "Ahmed"));
            ProcessResult("Update Last Name", _userService.UpdateLastName(_testUserId, "Al-Mansoori"));
            ProcessResult("Update Gender", _userService.UpdateGender(_testUserId, enGender.Male));
            ProcessResult("Update Email", _userService.UpdateEmail(_testUserId, "ahmed" + DateTime.Now.Second + ".new@clinic.com"));
            ProcessResult("Update Contact Number", _userService.UpdateContactNumber(_testUserId, "777" + DateTime.Now.Second + "3456"));
            ProcessResult("Update Address", _userService.UpdateAddress(_testUserId, "Taiz Street, Sana'a"));
            ProcessResult("Update Date of Birth", _userService.UpdateDateOfBirth(_testUserId, new DateTime(1990, 1, 1)));
            ProcessResult("Update Full Contact Info", _userService.UpdateContactInfo(_testUserId, "777" + DateTime.Now.Second + "3456", "ahmed" + DateTime.Now.Second + ".new@clinic.com"));
        }

        static void Test_AccountStatus()
        {
            PrintSection("6. Account Status Control");

            // Deactivate account and check if login is still possible
            var deactivateResult = _userService.DeactivateUser(_testUserId);
            ProcessResult("Deactivate User", deactivateResult);

            var userResult = _userService.GetUserById(_testUserId);

            if (userResult.IsSuccess && userResult.Data != null)
            {
                var user = userResult.Data;
                var loginResult = _userService.Login(user.Username, "NewSecurePass123");
                PrintResult($"Login while Deactivated (Should Fail): {loginResult.Result}", !loginResult.IsSuccess);
            }
            else
            {
                PrintResult("Skipping Login Test: User data could not be retrieved.", true);
            }

            // Reactivate and restore functionality
            ProcessResult("Activate User", _userService.ActivateUser(_testUserId));
        }

        static void Test_DeleteAndRestore()
        {
            PrintSection("7. Soft Delete & Restore");

            ProcessResult("Soft Delete Person", _userService.SoftDeletePerson(_testUserId));
            ProcessResult("Restore Person/User", _userService.RestoreUser(_testUserId));
        }

        #endregion

        #region Professional Output Helpers

        /// <summary>
        /// Standardizes the visual output of each test case result.
        /// </summary>
        static void ProcessResult<T>(string actionName, ServiceResult<T, enUserResult> result)
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
                    Console.WriteLine("   Validation Errors:");
                    foreach (var error in result.ValidationErrors)
                    {
                        Console.WriteLine($"   - {error}");
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