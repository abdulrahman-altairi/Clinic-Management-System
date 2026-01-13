using Clinic.BLL.Enums;

namespace Clinic.BLL.Results
{
    public static class clsAppointmentViewResultMessages
    {
        public static string GetMessage(enAppointmentViewResult result)
        {
            switch (result)
            {
                case enAppointmentViewResult.Success:
                    return "Appointment operation completed successfully.";

                case enAppointmentViewResult.PatientNotFound:
                    return "The specified patient record could not be found.";

                case enAppointmentViewResult.DoctorNotFound:
                    return "The specified doctor record could not be found.";

                case enAppointmentViewResult.DoctorBusy:
                    return "The doctor has another appointment during the selected time slot.";

                case enAppointmentViewResult.InvalidDateTime:
                    return "The selected date or time format is invalid.";

                case enAppointmentViewResult.PastDateNotAllowed:
                    return "Cannot book or reschedule appointments for a past date.";

                case enAppointmentViewResult.AppointmentNotFound:
                    return "The specified appointment record could not be found.";

                case enAppointmentViewResult.StatusAlreadySet:
                    return "The appointment is already set to this status.";

                case enAppointmentViewResult.OperationFailed:
                    return "The appointment operation failed to execute.";

                case enAppointmentViewResult.DatabaseError:
                    return "A database error occurred while processing the appointment.";

                case enAppointmentViewResult.AccessDenied:
                    return "You do not have the required permissions for this action.";

                case enAppointmentViewResult.ValidationError:
                    return "Validation failed for the appointment data provided.";

                case enAppointmentViewResult.NoAppointmentsFound:
                    return "No appointment records were found matching the criteria.";

                case enAppointmentViewResult.NoDataForPeriod:
                    return "There are no appointments or revenue data for the selected period.";

                default:
                    return "An unknown error occurred in the appointment service.";
            }
        }
    }
}