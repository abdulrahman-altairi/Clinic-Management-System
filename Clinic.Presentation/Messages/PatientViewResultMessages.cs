using Clinic.BLL.Enums;

namespace Clinic.BLL.Results
{
    public static class clsPatientViewResultMessages
    {
        public static string GetMessage(enPatientViewResult result)
        {
            switch (result)
            {
                case enPatientViewResult.Success:
                    return "Patient information retrieved successfully.";

                case enPatientViewResult.PatientNotFound:
                    return "The specified patient record could not be found.";

                case enPatientViewResult.NoPatientsFound:
                    return "No patient records match your search criteria.";

                case enPatientViewResult.InsuranceProviderNotFound:
                    return "The specified insurance provider records were not found.";

                case enPatientViewResult.DatabaseError:
                    return "A database error occurred while fetching patient data.";

                case enPatientViewResult.OperationFailed:
                    return "The patient view operation failed to execute.";

                default:
                    return "An unknown error occurred in the patient view service.";
            }
        }
    }
}