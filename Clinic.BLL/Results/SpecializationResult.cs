namespace Clinic.BLL.Enums
{
    public enum enSpecializationResult
    {
        // General Success
        Success,

        // Lookup & Identification
        NotFound,

        // Integrity & Constraints
        DuplicateName,
        HasRelatedDoctors,

        // System & Process Errors
        ValidationError,
        DatabaseError,
        OperationFailed
    }
}