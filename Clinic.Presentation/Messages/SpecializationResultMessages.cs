using Clinic.BLL.Enums;
using System;

namespace Clinic.BLL.Results
{
    public static class clsSpecializationResultMessages
    {
        public static string GetMessage(enSpecializationResult result)
        {
            switch (result)
            {
                case enSpecializationResult.Success:
                    return "Specialization operation completed successfully.";

                case enSpecializationResult.NotFound:
                    return "The specified specialization could not be found.";

                case enSpecializationResult.DuplicateName:
                    return "A specialization with this name already exists in the system.";

                case enSpecializationResult.HasRelatedDoctors:
                    return "Cannot delete specialization because it is currently assigned to one or more doctors.";

                case enSpecializationResult.ValidationError:
                    return "One or more validation errors occurred in specialization data.";

                case enSpecializationResult.DatabaseError:
                    return "A database error occurred while processing the specialization record.";

                case enSpecializationResult.OperationFailed:
                    return "The specialization operation failed due to an unexpected error.";

                default:
                    return "An unknown error occurred in specialization service.";
            }
        }
    }
}