using Clinic.BLL.Enums;

namespace Clinic.BLL.Results
{
    public static class clsMedicalRecordResultMessages
    {
        public static string GetMessage(enMedicalRecordResult result)
        {
            switch (result)
            {
                case enMedicalRecordResult.Success:
                    return "Medical record operation completed successfully.";

                case enMedicalRecordResult.OperationFailed:
                    return "The medical record operation failed due to an unexpected error.";

                case enMedicalRecordResult.NotFound:
                    return "The specified medical record could not be found.";

                case enMedicalRecordResult.DatabaseError:
                    return "A database error occurred while processing the medical record.";

                case enMedicalRecordResult.ValidationError:
                    return "One or more validation errors occurred in the medical record data.";

                case enMedicalRecordResult.DiagnosisRequired:
                    return "Diagnosis is mandatory and cannot be left empty.";

                case enMedicalRecordResult.DiagnosisTooLong:
                    return "The diagnosis text exceeds the maximum allowed length.";

                case enMedicalRecordResult.PrescriptionTooLong:
                    return "The prescription text exceeds the maximum allowed length.";

                case enMedicalRecordResult.NotesTooLong:
                    return "The notes text exceeds the maximum allowed length.";

                case enMedicalRecordResult.AppointmentNotFound:
                    return "The specified appointment does not exist in the system.";

                case enMedicalRecordResult.RecordAlreadyExistsForAppointment:
                    return "A medical record already exists for this appointment; duplicate entries are not allowed.";

                case enMedicalRecordResult.UnauthorizedAction:
                    return "You do not have the necessary permissions to perform this action on the medical record.";

                case enMedicalRecordResult.PatientHasNoHistory:
                    return "No previous medical history or records found for this patient.";

                case enMedicalRecordResult.InvalidDateRange:
                    return "The provided date range is invalid; please ensure the start date is before the end date.";

                default:
                    return "An unknown error occurred in the medical record service.";
            }
        }
    }
}