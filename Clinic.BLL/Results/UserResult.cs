using System;

namespace Clinic.BLL.Results
{
    public enum enUserResult
    {
        // Success States
        Success,

        // Validation Errors
        ValidationError,
        FirstNameRequired,
        FirstNameTooLong,
        FirstNameTooShort,
        LastNameRequired,
        LastNameTooLong,
        LastNameTooShort,
        UsernameRequired,
        UsernameLengthError,
        PasswordRequired,
        PasswordLengthError,
        GenderRequired,
        InvalidGender,
        DateOfBirthRequired,
        InvalidDateOfBirth,
        DateOfBirthInFuture,
        EmailRequired,
        InvalidEmail,
        ContactNumberRequired,
        InvalidPhoneNumber,
        PhoneNumberTooLong,
        PhoneNumberTooShort,
        RoleRequired,
        InvalidRole,

        // Database & Constraints
        UsernameAlreadyExists,
        EmailAlreadyExists,
        ContactNumberAlreadyExists,
        DatabaseError,

        // Authentication & General Status
        OperationFailed,
        InvalidCredentials,
        UserNotActive,
        UserNotFount
    }
}