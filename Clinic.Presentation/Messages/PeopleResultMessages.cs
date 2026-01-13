namespace Clinic.BLL.Results
{
    public static class PeopleResultMessages
    {
        public static string GetMessage(enPeopleResult result)
        {
            switch (result)
            {
                case enPeopleResult.Success:
                    return "The operation completed successfully.";

                case enPeopleResult.OperationFailed:
                    return "The operation failed to complete.";

                case enPeopleResult.ValidationError:
                    return "One or more validation errors occurred.";

                case enPeopleResult.DatabaseError:
                    return "A database error occurred while processing the request.";

                case enPeopleResult.UnhandledException:
                    return "An unexpected error occurred.";

                case enPeopleResult.AccessDenied:
                    return "Access denied. You do not have permission to perform this action.";

                case enPeopleResult.PersonNotFound:
                    return "The specified person was not found.";

                case enPeopleResult.DuplicatePerson:
                    return "A person with the same information already exists.";

                case enPeopleResult.PersonAddedSuccessfully:
                    return "The person was added successfully.";

                case enPeopleResult.PersonCreationFailed:
                    return "Failed to add the person.";

                case enPeopleResult.PersonUpdatedSuccessfully:
                    return "The person information was updated successfully.";

                case enPeopleResult.PersonUpdateFailed:
                    return "Failed to update the person information.";

                case enPeopleResult.FirstNameUpdatedSuccessfully:
                    return "The first name was updated successfully.";

                case enPeopleResult.FirstNameUpdateFailed:
                    return "Failed to update the first name.";

                case enPeopleResult.LastNameUpdatedSuccessfully:
                    return "The last name was updated successfully.";

                case enPeopleResult.LastNameUpdateFailed:
                    return "Failed to update the last name.";

                case enPeopleResult.GenderUpdatedSuccessfully:
                    return "The gender was updated successfully.";

                case enPeopleResult.GenderUpdateFailed:
                    return "Failed to update the gender.";

                case enPeopleResult.EmailUpdatedSuccessfully:
                    return "The email address was updated successfully.";

                case enPeopleResult.EmailUpdateFailed:
                    return "Failed to update the email address.";

                case enPeopleResult.EmailAlreadyExists:
                    return "The email address is already in use.";

                case enPeopleResult.ContactNumberUpdatedSuccessfully:
                    return "The contact number was updated successfully.";

                case enPeopleResult.ContactNumberUpdateFailed:
                    return "Failed to update the contact number.";

                case enPeopleResult.ContactNumberAlreadyExists:
                    return "The contact number is already in use.";

                case enPeopleResult.PersonDeletedSuccessfully:
                    return "The person was deleted successfully.";

                case enPeopleResult.PersonDeletionFailed:
                    return "Failed to delete the person.";

                default:
                    return "Unknown result.";
            }
        }
    }
}
