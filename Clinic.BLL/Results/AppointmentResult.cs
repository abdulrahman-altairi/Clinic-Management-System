namespace Clinic.BLL.Enums
{
    public enum enAppointmentResult
    {
        // General Success
        Success,

        // Entity Verification
        PatientNotFound,
        DoctorNotFound,
        AppointmentNotFound,

        // Scheduling & Conflict Rules
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