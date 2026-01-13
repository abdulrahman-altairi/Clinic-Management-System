namespace Clinic.BLL.Enums
{
    public enum enPatientResult
    {
        // General Outcomes
        Success,
        Failure,

        // Lookup & Identity
        PatientNotFound,
        PersonNotFound,

        // Validation & Constraints
        ValidationError,
        InsurancePolicyAlreadyExists,

        // System & Security
        DatabaseError,
        Unauthorized,
        DependencyError,
        OperationFailed
    }
}