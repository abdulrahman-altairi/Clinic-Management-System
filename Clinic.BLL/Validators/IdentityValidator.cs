using Clinic.BLL.Helper;
using Clinic.BLL.Results;
using Clinic.Contracts;
using Clinic.Contracts.Enums;
using Clinic.Contracts.Identity;
using System;
using System.Collections.Generic;

namespace Clinic.BLL.Validators
{


    // Manages security-related validation for user authentication and account registration.

    public class clsIdentityValidator
    {
        // Validates the format and requirements of login credentials before processing authentication.

        public static List<enValidationResult> ValidateLogin(string userName, string passWord)
        {
            var errors = new List<enValidationResult>();

            new clsValidationRole(userName, errors)
                .NotEmpty(enValidationResult.UsernameRequired)
                .Length(3, 20, enValidationResult.UsernameLengthError);

            new clsValidationRole(passWord, errors)
                .NotEmpty(enValidationResult.PasswordRequired)
                .Length(5, 20, enValidationResult.PasswordLengthError);

            return errors;
        }



        // Comprehensive validation for new user accounts, including both identity and personal profile data.


        // Comprehensive validation for new user accounts, including both identity and personal profile data.
        public static List<enValidationResult> ValidateRegistration(RegisterRequestDto registerDto)
        {
            var errors = new List<enValidationResult>();

            // Validate Authentication Credentials
            new clsValidationRole(registerDto.Username, errors)
                .NotEmpty(enValidationResult.UsernameRequired)
                .Length(3, 20, enValidationResult.UsernameLengthError);

            // Ensures password meets minimum complexity requirements for system security
            new clsValidationRole(registerDto.Password, errors)
                .NotEmpty(enValidationResult.PasswordRequired)
                .Length(5, 20, enValidationResult.PasswordLengthError);

            // Validate Personal Name Info
            new clsValidationRole(registerDto.FirstName, errors)
                .NotEmpty(enValidationResult.FirstNameRequired)
                .TooLong(30, enValidationResult.FirstNameTooLong)
                .TooSmall(3, enValidationResult.FirstNameTooSmall);

            new clsValidationRole(registerDto.LastName, errors)
                .NotEmpty(enValidationResult.LastNameRequired)
                .TooLong(30, enValidationResult.LastNameTooLong)
                .TooSmall(3, enValidationResult.LastNameTooSmall);

            // Validate Gender: Convert Enum to numeric string to avoid format errors
            new clsValidationRole(((int)registerDto.Gender).ToString(), errors)
                .NotEmpty(enValidationResult.GenderRequired)
                .IsValidGender(enValidationResult.InvalidGender);

            // FIX: Using .Value to access DateTime from Nullable<DateTime> to allow formatting
            // Validate DateOfBirth: Use "yyyy-MM-dd" format to match Regex patterns
            string dobString = registerDto.DateOfBirth.HasValue
                ? registerDto.DateOfBirth.Value.ToString("yyyy-MM-dd")
                : string.Empty;

            new clsValidationRole(dobString, errors)
                .Matches(clsRegexPatterns.Date, enValidationResult.InvalidDate)
                .NotFuture(enValidationResult.DateInFuture);

            // Validate Contact Information
            new clsValidationRole(registerDto.Email, errors)
                .NotEmpty(enValidationResult.EmailRequired)
                .Matches(clsRegexPatterns.Email, enValidationResult.InvalidEmail);

            new clsValidationRole(registerDto.ContactNumber, errors)
                .NotEmpty(enValidationResult.PhoneNumberRequired)
                .Matches(clsRegexPatterns.Phone, enValidationResult.InvalidPhoneNumber)
                .TooLong(15, enValidationResult.PhoneNumberTooLong)
                .TooSmall(7, enValidationResult.PhoneNumberTooSmall);

            // Validates that the assigned user role falls within the predefined authorization levels (Enum as Int)
            new clsValidationRole(((int)registerDto.Role).ToString(), errors)
                .NotEmpty(enValidationResult.RoleRequired)
                .Between(1, 3, enValidationResult.InvalidRole);

            return errors;
        }

        

    }
}
