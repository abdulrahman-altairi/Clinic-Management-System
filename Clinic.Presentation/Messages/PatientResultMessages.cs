using Clinic.BLL.Enums;
using System;

namespace Clinic.BLL.Results
{
    public static class clsPatientResultMessages
    {
        public static string GetMessage(enPatientResult result)
        {
            switch (result)
            {
                case enPatientResult.Success:
                    return "Patient operation completed successfully.";

                case enPatientResult.Failure:
                    return "The patient operation failed.";

                case enPatientResult.PatientNotFound:
                    return "The specified patient could not be found.";

                case enPatientResult.ValidationError:
                    return "One or more validation errors occurred.";

                case enPatientResult.DatabaseError:
                    return "A database error occurred while processing the patient record.";

                case enPatientResult.InsurancePolicyAlreadyExists:
                    return "The insurance policy number is already registered to another patient.";

                case enPatientResult.OperationFailed:
                    return "The operation failed due to an unexpected error.";

                case enPatientResult.PersonNotFound:
                    return "The base person record for this patient was not found.";

                case enPatientResult.Unauthorized:
                    return "You do not have the necessary permissions for this patient operation.";

                case enPatientResult.DependencyError:
                    return "The operation cannot be completed because the patient has related records.";

                default:
                    return "An unknown error occurred.";
            }
        }
    }
}