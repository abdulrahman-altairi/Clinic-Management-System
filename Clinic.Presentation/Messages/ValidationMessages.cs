using System;

namespace Clinic.BLL.Results
{
    public static class clsValidationMessages
    {
        public static string GetMessage(enValidationResult result)
        {
            switch (result)
            {
                case enValidationResult.UsernameRequired:
                    return "Username is required.";

                case enValidationResult.UsernameLengthError:
                    return "Username length is invalid.";

                case enValidationResult.PasswordRequired:
                    return "Password is required.";

                case enValidationResult.PasswordLengthError:
                    return "Password length is invalid.";

                case enValidationResult.RoleRequired:
                    return "User role is required.";

                case enValidationResult.InvalidRole:
                    return "The selected role is invalid.";

                case enValidationResult.FirstNameRequired:
                    return "First name is required.";

                case enValidationResult.FirstNameTooLong:
                    return "First name is too long.";

                case enValidationResult.FirstNameTooSmall:
                    return "First name is too short.";

                case enValidationResult.LastNameRequired:
                    return "Last name is required.";

                case enValidationResult.LastNameTooLong:
                    return "Last name is too long.";

                case enValidationResult.LastNameTooSmall:
                    return "Last name is too short.";

                case enValidationResult.EmailRequired:
                    return "Email is required.";

                case enValidationResult.InvalidEmail:
                    return "Email format is invalid.";

                case enValidationResult.PhoneNumberRequired:
                    return "Phone number is required.";

                case enValidationResult.InvalidPhoneNumber:
                    return "Phone number format is invalid.";

                case enValidationResult.PhoneNumberTooLong:
                    return "Phone number is too long.";

                case enValidationResult.PhoneNumberTooSmall:
                    return "Phone number is too short.";

                case enValidationResult.GenderRequired:
                    return "Gender is required.";

                case enValidationResult.InvalidGender:
                    return "Gender value is invalid.";

                case enValidationResult.InvalidDate:
                    return "The date format is invalid.";

                case enValidationResult.DateInFuture:
                    return "Date cannot be in the future.";

                case enValidationResult.InsuranceProviderRequired:
                    return "Insurance provider name is required.";

                case enValidationResult.InsurancePolicyNumberRequired:
                    return "Insurance policy number is required.";

                case enValidationResult.EmergencyContactNameRequired:
                    return "Emergency contact name is required.";

                case enValidationResult.EmergencyContactPhoneRequired:
                    return "Emergency contact phone number is required.";

                default:
                    return "Unknown validation error.";
            }
        }
    }
}