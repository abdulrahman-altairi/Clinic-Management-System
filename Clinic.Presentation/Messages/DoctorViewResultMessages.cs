using Clinic.BLL.Enums;

namespace Clinic.BLL.Results
{
    public static class clsDoctorViewResultMessages
    {
        public static string GetMessage(enDoctorViewResult result)
        {
            switch (result)
            {
                case enDoctorViewResult.Success:
                    return "Doctor information retrieved successfully.";

                case enDoctorViewResult.DoctorNotFound:
                    return "The specified doctor record could not be found.";

                case enDoctorViewResult.NoDoctorsFound:
                    return "No doctor records match your search criteria.";

                case enDoctorViewResult.SpecializationNotFound:
                    return "The specified specialization category does not exist.";

                case enDoctorViewResult.InvalidFeeRange:
                    return "The provided consultation fee range is invalid.";

                case enDoctorViewResult.DatabaseError:
                    return "A database error occurred while fetching doctor data.";

                case enDoctorViewResult.OperationFailed:
                    return "The doctor view operation failed to execute.";

                default:
                    return "An unknown error occurred in the doctor view service.";
            }
        }
    }
}