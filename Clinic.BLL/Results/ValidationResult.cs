namespace Clinic.BLL.Results
{
    public enum enValidationResult
    {
        // Identity & Security
        UsernameRequired,
        UsernameLengthError,
        PasswordRequired,
        PasswordLengthError,
        RoleRequired,
        InvalidRole,

        // Personal Information
        FirstNameRequired,
        FirstNameTooLong,
        FirstNameTooSmall,
        LastNameRequired,
        LastNameTooLong,
        LastNameTooSmall,

        // Common Fields
        EmailRequired,
        InvalidEmail,
        PhoneNumberRequired,
        PhoneNumberTooSmall,
        PhoneNumberTooLong,
        InvalidPhoneNumber,
        GenderRequired,
        InvalidGender,

        // Date Checks
        InvalidDate,
        DateInFuture,

        // Medical & Insurance
        InsuranceProviderRequired,
        InsurancePolicyNumberRequired,
        EmergencyContactNameRequired,
        EmergencyContactPhoneRequired,

        // Emergency Contact & Professional Details
        EmergencyPhoneRequired,
        EmergencyNameRequired,
        EmergencyNameTooLong,
        InvalidEmergencyPhone,
        SpecializationRequired,
        BioTooLong,
        InvalidConsultationFee,

        // Specializations Management
        SpecializationNameRequired,
        SpecializationNameTooLong,
        SpecializationNameTooSmall,
        DescriptionTooLong,

        // Medical Records & Clinical Data
        DiagnosisRequired,
        DiagnosisTooLong,
        PrescriptionTooLong,
        NotesTooLong,

        // Financials & Invoicing
        InvalidDueDate,
        DiscountExceedsTotal,
        DiscountCannotBeNegative,
        InvalidMoneyFormat,
        TaxCannotBeNegative,
        AmountMustBePositive,
        TotalAmountRequired,

        // Services & Line Items
        DescriptionRequired,
        DescriptionTooLongOrShort,
        InvalidQuantity,

        // Payments & Transactions
        TransactionRefRequired,
        InvalidTransactionRef,
        InvalidPaymentMethod,
        AmountExceedsRemainingBalance,

        // Appointments Scheduling
        PastDateNotAllowed,
        OutsideWorkingHours,
        ReasonRequired,
        ReasonTooLong,
    }
}