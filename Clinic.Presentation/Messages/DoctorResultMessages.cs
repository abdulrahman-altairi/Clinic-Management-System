using Clinic.BLL.Enums;
using System;

namespace Clinic.BLL.Results
{
    public static class clsDoctorResultMessages
    {
        public static string GetMessage(enDoctorResult result)
        {
            switch (result)
            {
                case enDoctorResult.Success:
                    return "Doctor operation completed successfully.";

                case enDoctorResult.Failure:
                    return "The doctor operation failed.";

                case enDoctorResult.DoctorNotFound:
                    return "The specified doctor could not be found.";

                case enDoctorResult.ValidationError:
                    return "One or more validation errors occurred in doctor data.";

                case enDoctorResult.DatabaseError:
                    return "A database error occurred while processing the doctor record.";

                case enDoctorResult.SpecializationNotFound:
                    return "The assigned specialization does not exist.";

                case enDoctorResult.DuplicateDoctorRecord:
                    return "This person is already registered as a doctor in the system.";

                case enDoctorResult.OperationFailed:
                    return "The operation failed due to an unexpected error.";

                case enDoctorResult.PersonNotFound:
                    return "The base person record for this doctor was not found.";

                case enDoctorResult.Unauthorized:
                    return "You do not have the necessary permissions for this doctor operation.";

                case enDoctorResult.DependencyError:
                    return "Cannot delete doctor because they have associated appointments or records.";

                case enDoctorResult.DoctorNotAvailable:
                    return "The doctor is currently marked as unavailable.";

                case enDoctorResult.InvalidConsultationFee:
                    return "The consultation fee provided is not within the valid range.";

                case enDoctorResult.BioTooLong:
                    return "The biography text exceeds the maximum allowed length.";

                default:
                    return "An unknown error occurred.";
            }
        }
    }
}