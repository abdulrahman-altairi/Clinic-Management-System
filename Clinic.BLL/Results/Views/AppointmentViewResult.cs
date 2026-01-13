namespace Clinic.BLL.Enums
{
    public enum enAppointmentViewResult
    {
        // General Success
        Success,

        // Data Retrieval & Reports
        NoAppointmentsFound,
        NoDataForPeriod,
        AppointmentNotFound,

        // Related Entities Verification
        PatientNotFound,
        DoctorNotFound,

        // Scheduling & Logic Conflicts
        DoctorBusy,
        InvalidDateTime,
        PastDateNotAllowed,
        StatusAlreadySet,

        // System, Security & Validation
        ValidationError,
        AccessDenied,
        DatabaseError,
        OperationFailed
    }
}