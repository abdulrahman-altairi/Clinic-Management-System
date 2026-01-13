using Clinic.BLL.Helper;
using Clinic.BLL.Results;
using Clinic.Contracts;
using Clinic.Contracts.Dtos;
using System.Collections.Generic;

namespace Clinic.BLL.Validators
{

    // Handles validation for medical-related entities including Specializations, Medical Records, and Appointments.

    public class clsMidecalValidator
    {

        // Validation rules for clinic specializations and departments.
        public static List<enValidationResult> ValidateSpecialization(SpecializationDto specDto)
        {
            var errors = new List<enValidationResult>();

            new clsValidationRole(specDto.SpecializationName, errors)
                .NotEmpty(enValidationResult.SpecializationNameRequired)
                .TooLong(100, enValidationResult.SpecializationNameTooLong)
                .TooSmall(3, enValidationResult.SpecializationNameTooSmall);

            if (!string.IsNullOrWhiteSpace(specDto.SpecializationDescription))
            {
                new clsValidationRole(specDto.SpecializationDescription, errors)
                    .TooLong(500, enValidationResult.DescriptionTooLong);
            }

            return errors;
        }

        public static List<enValidationResult> ValidateSpecializationName(string name)
        {
            var errors = new List<enValidationResult>();

            new clsValidationRole(name, errors)
                .NotEmpty(enValidationResult.SpecializationNameRequired)
                .TooLong(100, enValidationResult.SpecializationNameTooLong)
                .TooSmall(3, enValidationResult.SpecializationNameTooSmall);

            return errors;
        }

        public static List<enValidationResult> ValidateSpecializationDescription(string description)
        {
            var errors = new List<enValidationResult>();

            if (!string.IsNullOrWhiteSpace(description))
            {
                new clsValidationRole(description, errors)
                    .TooLong(500, enValidationResult.DescriptionTooLong);
            }

            return errors;
        }


        // Validates patient medical history, including clinical diagnoses and prescriptions.


        public static List<enValidationResult> ValidateRecord(MedicalRecordDto recordDto)
        {
            var errors = new List<enValidationResult>();

            new clsValidationRole(recordDto.Diagnosis, errors)
                .NotEmpty(enValidationResult.DiagnosisRequired)
                .TooLong(1000, enValidationResult.DiagnosisTooLong);

            if (!string.IsNullOrWhiteSpace(recordDto.Prescription))
            {
                new clsValidationRole(recordDto.Prescription, errors)
                    .TooLong(2000, enValidationResult.PrescriptionTooLong);
            }

            if (!string.IsNullOrWhiteSpace(recordDto.Notes))
            {
                new clsValidationRole(recordDto.Notes, errors)
                    .TooLong(1500, enValidationResult.NotesTooLong);
            }

            return errors;
        }

        public static List<enValidationResult> ValidateDiagnosis(MedicalRecordDto recordDto)
        {
            var errors = new List<enValidationResult>();

            new clsValidationRole(recordDto.Diagnosis, errors)
                .NotEmpty(enValidationResult.DiagnosisRequired)
                .TooLong(1000, enValidationResult.DiagnosisTooLong);

            return errors;
        }

        public static List<enValidationResult> ValidateNotes(MedicalRecordDto recordDto)
        {
            var errors = new List<enValidationResult>();

            if (!string.IsNullOrWhiteSpace(recordDto.Notes))
            {
                new clsValidationRole(recordDto.Notes, errors)
                    .TooLong(1500, enValidationResult.NotesTooLong);
            }

            return errors;
        }

        public static List<enValidationResult> ValidatePrescription(MedicalRecordDto recordDto)
        {
            var errors = new List<enValidationResult>();

            if (!string.IsNullOrWhiteSpace(recordDto.Prescription))
            {
                new clsValidationRole(recordDto.Prescription, errors)
                    .TooLong(2000, enValidationResult.PrescriptionTooLong);
            }

            return errors;
        }

        // Validates appointment scheduling logic, ensuring dates are future-based and within clinical business hours.

        public static List<enValidationResult> ValidateAppointment(AppointmentCreateDto appDto)
        {
            var errors = new List<enValidationResult>();

            new clsValidationRole(appDto.AppointmentDate.ToString(), errors)
                .IsFuture(enValidationResult.PastDateNotAllowed)
                .WithinWorkingHours(8, 20, enValidationResult.OutsideWorkingHours);

            new clsValidationRole(appDto.ReasonForVisit, errors)
                .NotEmpty(enValidationResult.ReasonRequired)
                .TooLong(500, enValidationResult.ReasonTooLong);

            return errors;
        }
    }
}
