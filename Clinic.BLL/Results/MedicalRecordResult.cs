namespace Clinic.BLL.Enums
{
    public enum enMedicalRecordResult
    {
        // General Success
        Success,

        // Data Retrieval & Status
        NotFound,
        PatientHasNoHistory,
        InvalidDateRange,

        // Validation & Content Errors
        ValidationError,
        DiagnosisRequired,
        DiagnosisTooLong,
        PrescriptionTooLong,
        NotesTooLong,

        // Appointment & Integrity Constraints
        AppointmentNotFound,
        RecordAlreadyExistsForAppointment,

        // System, Security & Technical Errors
        UnauthorizedAction,
        DatabaseError,
        OperationFailed
    }
}