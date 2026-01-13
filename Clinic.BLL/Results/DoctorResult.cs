namespace Clinic.BLL.Enums
{
    public enum enDoctorResult
    {
        // General Outcomes
        Success,
        Failure,

        // Lookup & Identity
        DoctorNotFound,
        PersonNotFound,
        SpecializationNotFound,

        // Validation & Professional Rules
        ValidationError,
        DuplicateDoctorRecord,
        InvalidConsultationFee,
        BioTooLong,

        // Availability & Scheduling
        DoctorNotAvailable,

        // System & Security
        DatabaseError,
        Unauthorized,
        DependencyError,
        OperationFailed
    }
}