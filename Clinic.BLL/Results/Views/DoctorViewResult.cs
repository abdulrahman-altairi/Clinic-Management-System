namespace Clinic.BLL.Enums
{
    public enum enDoctorViewResult
    {
        // General Success
        Success,

        // Data Retrieval & Search
        DoctorNotFound,
        NoDoctorsFound,
        SpecializationNotFound,

        // Filter & Range Validation
        InvalidFeeRange,

        // System & Technical Errors
        DatabaseError,
        OperationFailed
    }
}