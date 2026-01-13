namespace Clinic.BLL.Enums
{
    public enum enPatientViewResult
    {
        // General Success
        Success,

        // Data Retrieval & Search
        PatientNotFound,
        NoPatientsFound,
        InsuranceProviderNotFound,

        // System & Technical Errors
        DatabaseError,
        OperationFailed
    }
}