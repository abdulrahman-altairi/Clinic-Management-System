using Clinic.BLL.Common.Result;
using Clinic.BLL.Enums;
using Clinic.BLL.Validators;
using Clinic.Contracts;
using Clinic.Contracts.Enums;
using Clinic.DAL;
using Clinic.DAL.Repositories;
using Clinic.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Clinic.BLL.Services
{


    /// <summary>
    /// This service manages [User/Doctor/...] domain logic. 
    /// Note on Design: Some methods (like UpdateEmail, UpdateFirstName) are implemented here 
    /// despite being present in PeopleService to maintain 'Domain Decoupling' and ensure 
    /// each service returns its specific 'ServiceResult' type (e.g., enUserResult).
    /// This prevents 'Tight Coupling', simplifies Unit Testing, and allows for 
    /// future domain-specific business rules without affecting the generic PeopleService.
    /// Data consistency is maintained by reusing the underlying PeopleRepository.
    /// </summary>

    public class clsPatientService
    {
        private clsPatientRepositroy _patientRepository;
        private clsPeopleRepository _peopleRepository;

        public clsPatientService()
        {
            _patientRepository = new clsPatientRepositroy();
            _peopleRepository = new clsPeopleRepository();
        }

        public ServiceResult<int, enPatientResult> RegisterPatient(PatientDto patientDto)
        {

            var validatetor = clsPeopleValidator.ValidatePatient(patientDto);
            if (validatetor.Count > 0)
                return ServiceResult<int, enPatientResult>.Failure(
                    enPatientResult.ValidationError,
                    validationErrors: validatetor.Cast<Enum>().ToList()
                );

            using (var connection = DBHelper.GetOpenConnection())
            {
                using (var transaction = DBHelper.BeginTransaction(connection))
                {
                    try
                    {

                        if (_peopleRepository.EmailExists(patientDto.Email, connection: connection, transaction: transaction) > 0)
                            return ServiceResult<int, enPatientResult>.Failure(enPatientResult.ValidationError);

                        if (_patientRepository.InsurancePolicyExists(patientDto.InsurancePolicyNumber) > 0)
                        {
                            return ServiceResult<int, enPatientResult>.Failure(enPatientResult.InsurancePolicyAlreadyExists);
                        }

                        Person person = new Person
                        {
                            FirstName = patientDto.FirstName,
                            LastName = patientDto.LastName,
                            Address = patientDto.Address,
                            ContactNumber = patientDto.ContactNumber,
                            DateOfBirth = patientDto.DateOfBirth,
                            Email = patientDto.Email,
                            Gender = (Entities.Enums.enGender)patientDto.Gender
                        };


                        int personId = _peopleRepository.AddPerson(person, connection, transaction);
                        if (personId <= 0) throw new Exception("Person insertion failed");

                        Patient patient = new Patient
                        {
                            PatintId = personId,
                            InsuranceProvider = patientDto.InsuranceProvider,
                            InsurancePolicyNumber = patientDto.InsurancePolicyNumber,
                            EmergencyContactName = patientDto.EmergencyContactName,
                            EmergencyContactPhone = patientDto.EmergencyContactPhone
                        };


                        int rowsAffected = _patientRepository.AddPatient(patient, connection, transaction);

                        if (rowsAffected > 0)
                        {
                            transaction.Commit();
                            return ServiceResult<int, enPatientResult>.Success(personId, enPatientResult.Success);
                        }

                        transaction.Rollback();
                        return ServiceResult<int, enPatientResult>.Failure(enPatientResult.DatabaseError);
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        return ServiceResult<int, enPatientResult>.Failure(enPatientResult.OperationFailed);
                    }
                }
            }
        }

        public ServiceResult<int, enPatientResult> UpdateFirstName(int personId, string firstName)
        {
            var validatetor = clsPeopleValidator.ValidateFirstName(firstName);
            if (validatetor.Count > 0)
                return ServiceResult<int, enPatientResult>.Failure(
                    enPatientResult.ValidationError,
                    validationErrors: validatetor.Cast<Enum>().ToList()
                );

            try
            {
                int rowsAffected = _peopleRepository.UpdateFirstName(personId, firstName);
                if (rowsAffected > 0)
                    return ServiceResult<int, enPatientResult>.Success(rowsAffected, enPatientResult.Success);
                return ServiceResult<int, enPatientResult>.Failure(enPatientResult.DatabaseError);
            }
            catch (Exception)
            {
                return ServiceResult<int, enPatientResult>.Failure(enPatientResult.OperationFailed);
            }
        }

        public ServiceResult<int, enPatientResult> UpdateGender(int personId, enGender gender)
        {
            var validator = clsPeopleValidator.ValidateGender(gender);
            if (validator.Count > 0)
                return ServiceResult<int, enPatientResult>.Failure(
                    enPatientResult.ValidationError,
                    validationErrors: validator.Cast<Enum>().ToList()
                );

            try
            {
                int rowsAffected = _peopleRepository.UpdateGender(personId, (Entities.Enums.enGender)gender);

                if (rowsAffected > 0)
                    return ServiceResult<int, enPatientResult>.Success(rowsAffected, enPatientResult.Success);

                return ServiceResult<int, enPatientResult>.Failure(enPatientResult.DatabaseError);
            }
            catch (Exception)
            {
                return ServiceResult<int, enPatientResult>.Failure(enPatientResult.OperationFailed);
            }
        }

        public ServiceResult<int, enPatientResult> UpdateLastName(int personId, string lastName)
        {
            var validator = clsPeopleValidator.ValidateLastName(lastName);
            if (validator.Count > 0)
                return ServiceResult<int, enPatientResult>.Failure(
                    enPatientResult.ValidationError,
                    validationErrors: validator.Cast<Enum>().ToList()
                );

            try
            {
                int rowsAffected = _peopleRepository.UpdateLastName(personId, lastName);

                if (rowsAffected > 0)
                    return ServiceResult<int, enPatientResult>.Success(rowsAffected, enPatientResult.Success);

                return ServiceResult<int, enPatientResult>.Failure(enPatientResult.DatabaseError);
            }
            catch (Exception)
            {
                return ServiceResult<int, enPatientResult>.Failure(enPatientResult.OperationFailed);
            }
        }

        public ServiceResult<int, enPatientResult> UpdateEmail(int personId, string email)
        {
            var validatetor = clsPeopleValidator.ValidateEmail(email);
            if (validatetor.Count > 0)
                return ServiceResult<int, enPatientResult>.Failure(
                    enPatientResult.ValidationError,
                    validationErrors: validatetor.Cast<Enum>().ToList()
                );

            try
            {
                if (_peopleRepository.EmailExists(email, personId) > 0)
                    return ServiceResult<int, enPatientResult>.Failure(enPatientResult.ValidationError);

                int rowsAffected = _peopleRepository.UpdateEmail(personId, email);

                if (rowsAffected > 0)
                    return ServiceResult<int, enPatientResult>.Success(rowsAffected, enPatientResult.Success);

                return ServiceResult<int, enPatientResult>.Failure(enPatientResult.DatabaseError);
            }
            catch (Exception)
            {
                return ServiceResult<int, enPatientResult>.Failure(enPatientResult.OperationFailed);
            }
        }

        public ServiceResult<int, enPatientResult> UpdateContactNumber(int personId, string contactNumber)
        {
            var validator = clsPeopleValidator.ValidateContactNumber(contactNumber);
            if (validator.Count > 0)
                return ServiceResult<int, enPatientResult>.Failure(
                    enPatientResult.ValidationError,
                    validationErrors: validator.Cast<Enum>().ToList()
                );

            try
            {
                if (_peopleRepository.ContactNumberExists(contactNumber, personId) > 0)
                    return ServiceResult<int, enPatientResult>.Failure(enPatientResult.ValidationError);

                int rowsAffected = _peopleRepository.UpdateContactNumber(personId, contactNumber);

                if (rowsAffected > 0)
                    return ServiceResult<int, enPatientResult>.Success(rowsAffected, enPatientResult.Success);

                return ServiceResult<int, enPatientResult>.Failure(enPatientResult.DatabaseError);
            }
            catch (Exception)
            {
                return ServiceResult<int, enPatientResult>.Failure(enPatientResult.OperationFailed);
            }
        }

        public ServiceResult<int, enPatientResult> UpdateDateOfBirth(int personId, DateTime dateOfBirth)
        {
            string formattedDate = dateOfBirth.ToString("yyyy-MM-dd");

            var validator = clsPeopleValidator.ValidateDateOfBirth(formattedDate);
            if (validator.Count > 0)
                return ServiceResult<int, enPatientResult>.Failure(
                    enPatientResult.ValidationError,
                    validationErrors: validator.Cast<Enum>().ToList()
                );

            try
            {
                int rowsAffected = _peopleRepository.UpdateDateOfBirth(personId, dateOfBirth);

                if (rowsAffected > 0)
                    return ServiceResult<int, enPatientResult>.Success(rowsAffected, enPatientResult.Success);

                return ServiceResult<int, enPatientResult>.Failure(enPatientResult.DatabaseError);
            }
            catch (Exception)
            {
                return ServiceResult<int, enPatientResult>.Failure(enPatientResult.OperationFailed);
            }
        }

        public ServiceResult<int, enPatientResult> UpdateAddress(int personId, string address)
        {
            try
            {
                int rowsAffected = _peopleRepository.UpdateAddress(personId, address);

                if (rowsAffected > 0)
                    return ServiceResult<int, enPatientResult>.Success(rowsAffected, enPatientResult.Success);

                return ServiceResult<int, enPatientResult>.Failure(enPatientResult.DatabaseError);
            }
            catch (Exception)
            {
                return ServiceResult<int, enPatientResult>.Failure(enPatientResult.OperationFailed);
            }
        }

        public ServiceResult<int, enPatientResult> UpdateContactInfo(int personId, string phone, string email)
        {
            try
            {

                if (_peopleRepository.EmailExists(email, personId) > 0)
                    return ServiceResult<int, enPatientResult>.Failure(enPatientResult.ValidationError);

                if (_peopleRepository.ContactNumberExists(phone, personId) > 0)
                    return ServiceResult<int, enPatientResult>.Failure(enPatientResult.ValidationError);

                int rowsAffected = _peopleRepository.UpdateContactInfo(personId, phone, email);

                if (rowsAffected > 0)
                    return ServiceResult<int, enPatientResult>.Success(rowsAffected, enPatientResult.Success);

                return ServiceResult<int, enPatientResult>.Failure(enPatientResult.DatabaseError);
            }
            catch (Exception)
            {
                return ServiceResult<int, enPatientResult>.Failure(enPatientResult.OperationFailed);
            }
        }

        public ServiceResult<PatientDto, enPatientResult> GetPatientById(int patientId)
        {
            try
            {

                var patient = _patientRepository.GetPatientById(patientId);
                if (patient == null)
                    return ServiceResult<PatientDto, enPatientResult>.Failure(enPatientResult.PatientNotFound);

                return ServiceResult<PatientDto, enPatientResult>.Success(_MapPatientToPatientDto(patient), enPatientResult.Success);
            }
            catch (Exception)
            {
                return ServiceResult<PatientDto, enPatientResult>.Failure(enPatientResult.OperationFailed);
            }
        }

        public ServiceResult<List<PatientDto>, enPatientResult> GetAllPatients()
        {
            try
            {
                var patients = _patientRepository.GetAllPatients();
                if (patients == null)
                    return ServiceResult<List<PatientDto>, enPatientResult>.Failure(enPatientResult.Failure);
                return ServiceResult<List<PatientDto>, enPatientResult>.Success(_MapToDtoList(patients), enPatientResult.Success);
            }
            catch (Exception)
            {
                return ServiceResult<List<PatientDto>, enPatientResult>.Failure(enPatientResult.OperationFailed);
            }
        }

        public ServiceResult<List<PatientDto>, enPatientResult> GetPatientsPaged(int pageNumber, int pageSize)
        {
            try
            {
                var patients = _patientRepository.GetPatientsPaged(pageNumber, pageSize);
                if (patients == null)
                    return ServiceResult<List<PatientDto>, enPatientResult>.Failure(enPatientResult.Failure);
                return ServiceResult<List<PatientDto>, enPatientResult>.Success(_MapToDtoList(patients), enPatientResult.Success);
            }
            catch (Exception)
            {
                return ServiceResult<List<PatientDto>, enPatientResult>.Failure(enPatientResult.OperationFailed);
            }
        }

        public ServiceResult<List<PatientDto>, enPatientResult> SearchPatients(string keyword)
        {
            try
            {
                var patients = _patientRepository.SearchPatients(keyword);
                if (patients == null)
                    return ServiceResult<List<PatientDto>, enPatientResult>.Failure(enPatientResult.Failure);
                return ServiceResult<List<PatientDto>, enPatientResult>.Success(_MapToDtoList(patients), enPatientResult.Success);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"DEBUG ERROR: {ex.Message}");
                return ServiceResult<List<PatientDto>, enPatientResult>.Failure(enPatientResult.OperationFailed);
            }
        }

        public ServiceResult<int, enPatientResult> DeletePatient(int patientId)
        {
            try
            {
                if (_patientRepository.PatientExists(patientId) == 0)
                    return ServiceResult<int, enPatientResult>.Failure(enPatientResult.PatientNotFound);

                int rowsAffected = _peopleRepository.SoftDeletePerson(patientId);

                if (rowsAffected > 0)
                {
                    return ServiceResult<int, enPatientResult>.Success(rowsAffected, enPatientResult.Success);
                }

                return ServiceResult<int, enPatientResult>.Failure(enPatientResult.DatabaseError);
            }
            catch (Exception)
            {
                return ServiceResult<int, enPatientResult>.Failure(enPatientResult.OperationFailed);
            }
        }

        public ServiceResult<int, enPatientResult> UpdateInsuranceInfo(int patientId, string provider, string policy)
        {
            try
            {
                if (_patientRepository.PatientExists(patientId) == 0)
                    return ServiceResult<int, enPatientResult>.Failure(enPatientResult.PatientNotFound);

                if (_patientRepository.InsurancePolicyExists(policy, patientId) > 0)
                {
                    return ServiceResult<int, enPatientResult>.Failure(enPatientResult.InsurancePolicyAlreadyExists);
                }

                _patientRepository.UpdateInsuranceProvider(patientId, provider);
                int rowsAffected = _patientRepository.UpdateInsurancePolicyNumber(patientId, policy);

                return rowsAffected > 0
                    ? ServiceResult<int, enPatientResult>.Success(rowsAffected, enPatientResult.Success)
                    : ServiceResult<int, enPatientResult>.Failure(enPatientResult.DatabaseError);
            }
            catch (Exception)
            {
                return ServiceResult<int, enPatientResult>.Failure(enPatientResult.OperationFailed);
            }
        }

        public ServiceResult<int, enPatientResult> UpdateEmergencyContact(int patientId, string name, string phone)
        {
            try
            {
                if (_patientRepository.PatientExists(patientId) == 0)
                    return ServiceResult<int, enPatientResult>.Failure(enPatientResult.PatientNotFound);

                _patientRepository.UpdateEmergencyContactName(patientId, name);
                int rowsAffected = _patientRepository.UpdateEmergencyContactPhone(patientId, phone);

                return rowsAffected > 0
                    ? ServiceResult<int, enPatientResult>.Success(rowsAffected, enPatientResult.Success)
                    : ServiceResult<int, enPatientResult>.Failure(enPatientResult.DatabaseError);
            }
            catch (Exception)
            {
                return ServiceResult<int, enPatientResult>.Failure(enPatientResult.OperationFailed);
            }
        }

        private PatientDto _MapPatientToPatientDto(Patient patient)
        {
            return new PatientDto
            {
                PatintId = patient.PatintId,
                FirstName = patient.FirstName,
                LastName = patient.LastName,
                Email = patient.Email,
                ContactNumber = patient.ContactNumber,
                DateOfBirth = (DateTime)patient.DateOfBirth,
                Gender = (enGender)patient.Gender,
                Address = patient.Address,
                InsuranceProvider = patient.InsuranceProvider,
                InsurancePolicyNumber = patient.InsurancePolicyNumber,
                EmergencyContactName = patient.EmergencyContactName,
                EmergencyContactPhone = patient.EmergencyContactPhone
            };
        }

        private List<PatientDto> _MapToDtoList(List<Patient> patients)
        {
            var list = new List<PatientDto>();
            if (patients != null)
            {
                foreach (var p in patients) list.Add(_MapPatientToPatientDto(p));
            }
            return list;
        }
    }
}
