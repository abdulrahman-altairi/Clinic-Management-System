using Clinic.BLL.Enums;

namespace Clinic.BLL.Results
{
    public static class clsAppointmentResultMessages
    {
        public static string GetMessage(enAppointmentResult result)
        {
            switch (result)
            {
                case enAppointmentResult.Success:
                    return "Appointment operation completed successfully.";

                case enAppointmentResult.PatientNotFound:
                    return "The specified patient record could not be found.";

                case enAppointmentResult.DoctorNotFound:
                    return "The specified doctor record could not be found.";

                case enAppointmentResult.DoctorBusy:
                    return "The doctor has another appointment during the selected time slot.";

                case enAppointmentResult.InvalidDateTime:
                    return "The selected date or time format is invalid.";

                case enAppointmentResult.PastDateNotAllowed:
                    return "Cannot book or reschedule appointments for a past date.";

                case enAppointmentResult.AppointmentNotFound:
                    return "The specified appointment record could not be found.";

                case enAppointmentResult.StatusAlreadySet:
                    return "The appointment is already set to this status.";

                case enAppointmentResult.OperationFailed:
                    return "The appointment operation failed to execute.";

                case enAppointmentResult.DatabaseError:
                    return "A database error occurred while processing the appointment.";

                case enAppointmentResult.AccessDenied:
                    return "You do not have the required permissions for this action.";

                case enAppointmentResult.ValidationError:
                    return "Validation failed for the appointment data provided.";

                default:
                    return "An unknown error occurred in the appointment service.";
            }
        }
    }
}