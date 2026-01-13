using Clinic.BLL.Helper;
using Clinic.BLL.Results;
using Clinic.Contracts;
using Clinic.Contracts.Enums;
using System.Collections.Generic;

namespace Clinic.BLL.Validators
{

    // Provides centralized validation logic fro Person, Patient and Doctor dada transfer object.

    public class clsPeopleValidator
    {
        public static List<enValidationResult> ValidatePerson(PersonDto personDto)
        {
            var errors = new List<enValidationResult>();

            new clsValidationRole(personDto.FirstName, errors)
                .NotEmpty(enValidationResult.FirstNameRequired)
                .TooLong(30, enValidationResult.FirstNameTooLong)
                .TooSmall(3, enValidationResult.FirstNameTooSmall);

            new clsValidationRole(personDto.LastName, errors)
                .NotEmpty(enValidationResult.LastNameRequired)
                .TooLong(30, enValidationResult.LastNameTooLong)
                .TooSmall(3, enValidationResult.LastNameTooSmall);

            new clsValidationRole(personDto.Gender.ToString(), errors)
                .NotEmpty(enValidationResult.GenderRequired)
                .IsValidGender(enValidationResult.InvalidGender);

            new clsValidationRole(personDto.DateOfBirth.ToString(), errors)
                .Matches(clsRegexPatterns.Date, enValidationResult.InvalidDate)
                .NotFuture(enValidationResult.DateInFuture);

            new clsValidationRole(personDto.Email, errors)
                .NotEmpty(enValidationResult.EmailRequired)
                .Matches(clsRegexPatterns.Email, enValidationResult.InvalidEmail);

            new clsValidationRole(personDto.ContactNumber, errors)
                .NotEmpty(enValidationResult.PhoneNumberRequired)
                .Matches(clsRegexPatterns.Phone, enValidationResult.InvalidPhoneNumber)
                .TooLong(15, enValidationResult.PhoneNumberTooLong)
                .TooSmall(7, enValidationResult.PhoneNumberTooSmall);

            return errors;
        }

        public static List<enValidationResult> ValidateFirstName(string firstName)
        {
            var errors = new List<enValidationResult>();
            new clsValidationRole(firstName, errors)
                .NotEmpty(enValidationResult.FirstNameRequired)
                .TooLong(30, enValidationResult.FirstNameTooLong)
                .TooSmall(3, enValidationResult.FirstNameTooSmall);
            return errors;
        }

        public static List<enValidationResult> ValidateLastName(string lastName)
        {
            var errors = new List<enValidationResult>();
            new clsValidationRole(lastName, errors)
                .NotEmpty(enValidationResult.LastNameRequired)
                .TooLong(30, enValidationResult.LastNameTooLong)
                .TooSmall(3, enValidationResult.LastNameTooSmall);
            return errors;
        }

        public static List<enValidationResult> ValidateEmail(string email)
        {
            var errors = new List<enValidationResult>();
            new clsValidationRole(email, errors)
                .NotEmpty(enValidationResult.EmailRequired)
                .Matches(clsRegexPatterns.Email, enValidationResult.InvalidEmail);
            return errors;
        }

        public static List<enValidationResult> ValidateContactNumber(string contactNumber)
        {
            var errors = new List<enValidationResult>();
            new clsValidationRole(contactNumber, errors)
                .NotEmpty(enValidationResult.PhoneNumberRequired)
                .Matches(clsRegexPatterns.Phone, enValidationResult.InvalidPhoneNumber)
                .TooLong(15, enValidationResult.PhoneNumberTooLong)
                .TooSmall(7, enValidationResult.PhoneNumberTooSmall);
            return errors;
        }

        public static List<enValidationResult> ValidateContactInfo(UpdateContactInfoDto dto)
        {
            var errors = new List<enValidationResult>();

            new clsValidationRole(dto.Email, errors)
                .NotEmpty(enValidationResult.EmailRequired)
                .Matches(clsRegexPatterns.Email, enValidationResult.InvalidEmail);

            new clsValidationRole(dto.ContactNumber, errors)
                .NotEmpty(enValidationResult.PhoneNumberRequired)
                .Matches(clsRegexPatterns.Phone, enValidationResult.InvalidPhoneNumber)
                .TooLong(15, enValidationResult.PhoneNumberTooLong)
                .TooSmall(7, enValidationResult.PhoneNumberTooSmall);

            return errors;
        }

        public static List<enValidationResult> ValidateGender(enGender gender)
        {
            var errors = new List<enValidationResult>();

            new clsValidationRole(((int)gender).ToString(), errors)
                .NotEmpty(enValidationResult.GenderRequired)
                .IsValidGender(enValidationResult.InvalidGender);

            return errors;
        }

        public static List<enValidationResult> ValidateDateOfBirth(string dateOfBirth)
        {
            var errors = new List<enValidationResult>();

            new clsValidationRole(dateOfBirth, errors)
                .Matches(clsRegexPatterns.Date, enValidationResult.InvalidDate)
                .NotFuture(enValidationResult.DateInFuture);

            return errors;
        }



        // ######### Patient

        public static List<enValidationResult> ValidatePatient(PatientDto patientDto)
        {
            var errors = new List<enValidationResult>();

            new clsValidationRole(patientDto.FirstName, errors)
                .NotEmpty(enValidationResult.FirstNameRequired)
                .TooLong(30, enValidationResult.FirstNameTooLong)
                .TooSmall(3, enValidationResult.FirstNameTooSmall);

            new clsValidationRole(patientDto.LastName, errors)
                .NotEmpty(enValidationResult.LastNameRequired)
                .TooLong(30, enValidationResult.LastNameTooLong)
                .TooSmall(3, enValidationResult.LastNameTooSmall);

            new clsValidationRole(((int)patientDto.Gender).ToString(), errors)
                .NotEmpty(enValidationResult.GenderRequired)
                .IsValidGender(enValidationResult.InvalidGender);

            new clsValidationRole(patientDto.DateOfBirth.ToString("yyyy-MM-dd"), errors)
                .Matches(clsRegexPatterns.Date, enValidationResult.InvalidDate)
                .NotFuture(enValidationResult.DateInFuture);

            new clsValidationRole(patientDto.Email, errors)
                .NotEmpty(enValidationResult.EmailRequired)
                .Matches(clsRegexPatterns.Email, enValidationResult.InvalidEmail);

            new clsValidationRole(patientDto.ContactNumber, errors)
                .NotEmpty(enValidationResult.PhoneNumberRequired)
                .Matches(clsRegexPatterns.Phone, enValidationResult.InvalidPhoneNumber)
                .TooLong(15, enValidationResult.PhoneNumberTooLong)
                .TooSmall(7, enValidationResult.PhoneNumberTooSmall);

            if (!string.IsNullOrWhiteSpace(patientDto.InsurancePolicyNumber))
            {
                new clsValidationRole(patientDto.InsuranceProvider, errors)
                    .NotEmpty(enValidationResult.InsuranceProviderRequired);
            }

            if (!string.IsNullOrWhiteSpace(patientDto.EmergencyContactName) || !string.IsNullOrWhiteSpace(patientDto.EmergencyContactPhone))
            {
                new clsValidationRole(patientDto.EmergencyContactName, errors)
                    .NotEmpty(enValidationResult.EmergencyNameRequired)
                    .TooLong(60, enValidationResult.EmergencyNameTooLong);

                new clsValidationRole(patientDto.EmergencyContactPhone, errors)
                    .NotEmpty(enValidationResult.EmergencyPhoneRequired)
                    .Matches(clsRegexPatterns.Phone, enValidationResult.InvalidEmergencyPhone);
            }

            return errors;
        }


        // ######### Doctor


        public static List<enValidationResult> ValidateDoctor(DoctorDto doctorDto)
        {
            var errors = new List<enValidationResult>();

            new clsValidationRole(doctorDto.FirstName, errors)
                .NotEmpty(enValidationResult.FirstNameRequired)
                .TooLong(30, enValidationResult.FirstNameTooLong)
                .TooSmall(3, enValidationResult.FirstNameTooSmall);

            new clsValidationRole(doctorDto.LastName, errors)
                .NotEmpty(enValidationResult.LastNameRequired)
                .TooLong(30, enValidationResult.LastNameTooLong)
                .TooSmall(3, enValidationResult.LastNameTooSmall);

            new clsValidationRole(((int)doctorDto.Gender).ToString(), errors)
                .NotEmpty(enValidationResult.GenderRequired)
                .IsValidGender(enValidationResult.InvalidGender);

            if (doctorDto.DateOfBirth.HasValue)
            {
                new clsValidationRole(doctorDto.DateOfBirth.Value.ToString("yyyy-MM-dd"), errors)
                    .Matches(clsRegexPatterns.Date, enValidationResult.InvalidDate)
                    .NotFuture(enValidationResult.DateInFuture);
            }

            new clsValidationRole(doctorDto.Email, errors)
                .NotEmpty(enValidationResult.EmailRequired)
                .Matches(clsRegexPatterns.Email, enValidationResult.InvalidEmail);

            new clsValidationRole(doctorDto.ContactNumber, errors)
                .NotEmpty(enValidationResult.PhoneNumberRequired)
                .Matches(clsRegexPatterns.Phone, enValidationResult.InvalidPhoneNumber);

            if (doctorDto.SpecializationId <= 0)
            {
                errors.Add(enValidationResult.SpecializationRequired);
            }

            new clsValidationRole(doctorDto.Bio, errors)
                .TooLong(1000, enValidationResult.BioTooLong);

            if (doctorDto.ConsultationFee < 0)
            {
                errors.Add(enValidationResult.InvalidConsultationFee);
            }

            return errors;
        }
    }
}
