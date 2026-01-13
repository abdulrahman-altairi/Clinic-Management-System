namespace Clinic.BLL.Results
{
    public enum enPeopleResult
    {
        // General
        Success,
        OperationFailed,
        ValidationError,
        DatabaseError,
        UnhandledException,
        AccessDenied,

        // Person existence
        PersonNotFound,
        DuplicatePerson,

        // Creation
        PersonAddedSuccessfully,
        PersonCreationFailed,

        // Update (General)
        PersonUpdatedSuccessfully,
        PersonUpdateFailed,

        // Update (Specific Fields)
        FirstNameUpdatedSuccessfully,
        FirstNameUpdateFailed,

        LastNameUpdatedSuccessfully,
        LastNameUpdateFailed,

        GenderUpdatedSuccessfully,
        GenderUpdateFailed,

        EmailUpdatedSuccessfully,
        EmailUpdateFailed,
        EmailAlreadyExists,

        ContactNumberUpdatedSuccessfully,
        ContactNumberUpdateFailed,
        ContactNumberAlreadyExists,

        // Deletion
        PersonDeletedSuccessfully,
        PersonDeletionFailed
    }
}
