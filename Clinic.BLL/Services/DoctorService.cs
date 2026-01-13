using Clinic.BLL.Common.Result;
using Clinic.BLL.Enums;
using Clinic.BLL.Validators;
using Clinic.Contracts;
using Clinic.Contracts.DTOs;
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

    public class clsDoctorService
    {
        private readonly clsDoctorRepository _doctorRepository;
        private readonly clsPeopleRepository _peopleRepository;

        public clsDoctorService()
        {
            _doctorRepository = new clsDoctorRepository();
            _peopleRepository = new clsPeopleRepository();
        }

        public ServiceResult<int, enDoctorResult> RegisterDoctor(DoctorDto doctorDto)
        {

            var validatetor = clsPeopleValidator.ValidateDoctor(doctorDto);
            if (validatetor.Count > 0)
                return ServiceResult<int, enDoctorResult>.Failure(
                    enDoctorResult.ValidationError,
                    validationErrors: validatetor.Cast<Enum>().ToList()
                );

            using (var connection = DBHelper.GetOpenConnection())
            {
                using (var transaction = DBHelper.BeginTransaction(connection))
                {
                    try
                    {

                        if (_peopleRepository.EmailExists(doctorDto.Email, connection: connection, transaction: transaction) > 0)
                            return ServiceResult<int, enDoctorResult>.Failure(enDoctorResult.ValidationError);

                        Person person = new Person
                        {
                            FirstName = doctorDto.FirstName,
                            LastName = doctorDto.LastName,
                            Email = doctorDto.Email,
                            ContactNumber = doctorDto.ContactNumber,
                            DateOfBirth = doctorDto.DateOfBirth,
                            Gender = Entities.Enums.enGender.Male,
                            Address = doctorDto.Address
                        };

                        int personId = _peopleRepository.AddPerson(person, connection, transaction);
                        if (personId <= 0) throw new Exception("Person insertion failed");

                        Doctor doctor = new Doctor
                        {
                            DoctorId = personId,
                            SpecializationId = doctorDto.SpecializationId,
                            Bio = doctorDto.Bio,
                            ConsultationFee = doctorDto.ConsultationFee,
                            IsAvilable = doctorDto.IsAvailable
                        };

                        int rowsAffected = _doctorRepository.AddDoctor(doctor, connection, transaction);

                        if (rowsAffected > 0)
                        {
                            transaction.Commit();
                            return ServiceResult<int, enDoctorResult>.Success(personId, enDoctorResult.Success);
                        }

                        transaction.Rollback();
                        return ServiceResult<int, enDoctorResult>.Failure(enDoctorResult.DatabaseError);
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        return ServiceResult<int, enDoctorResult>.Failure(enDoctorResult.OperationFailed);
                    }
                }
            }
        }

        public ServiceResult<int, enDoctorResult> UpdateFullProfile(DoctorDto doctorDto)
        {

            var validatetor = clsPeopleValidator.ValidateDoctor(doctorDto);
            if (validatetor.Count > 0)
                return ServiceResult<int, enDoctorResult>.Failure(
                    enDoctorResult.ValidationError,
                    validationErrors: validatetor.Cast<Enum>().ToList()
                );

            try
            {
                if (_doctorRepository.DoctorExists(doctorDto.DoctorId) == 0)
                    return ServiceResult<int, enDoctorResult>.Failure(enDoctorResult.DoctorNotFound);

                Doctor doctor = new Doctor
                {
                    DoctorId = doctorDto.DoctorId,
                    SpecializationId = doctorDto.SpecializationId,
                    Bio = doctorDto.Bio,
                    ConsultationFee = doctorDto.ConsultationFee
                };

                int rowsAffected = _doctorRepository.UpdateFullProfile(doctor);
                return rowsAffected > 0
                    ? ServiceResult<int, enDoctorResult>.Success(rowsAffected, enDoctorResult.Success)
                    : ServiceResult<int, enDoctorResult>.Failure(enDoctorResult.DatabaseError);
            }
            catch (Exception)
            {
                return ServiceResult<int, enDoctorResult>.Failure(enDoctorResult.OperationFailed);
            }
        }

        public ServiceResult<int, enDoctorResult> UpdateFirstName(int personId, string firstName)
        {
            var validator = clsPeopleValidator.ValidateFirstName(firstName);
            if (validator.Count > 0)
                return ServiceResult<int, enDoctorResult>.Failure(enDoctorResult.ValidationError, validationErrors: validator.Cast<Enum>().ToList());

            try
            {
                int rowsAffected = _peopleRepository.UpdateFirstName(personId, firstName);
                if (rowsAffected > 0)
                    return ServiceResult<int, enDoctorResult>.Success(rowsAffected, enDoctorResult.Success);
                return ServiceResult<int, enDoctorResult>.Failure(enDoctorResult.DatabaseError);
            }
            catch (Exception)
            {
                return ServiceResult<int, enDoctorResult>.Failure(enDoctorResult.OperationFailed);
            }
        }

        public ServiceResult<int, enDoctorResult> UpdateLastName(int personId, string lastName)
        {
            var validator = clsPeopleValidator.ValidateLastName(lastName);
            if (validator.Count > 0)
                return ServiceResult<int, enDoctorResult>.Failure(enDoctorResult.ValidationError, validationErrors: validator.Cast<Enum>().ToList());

            try
            {
                int rowsAffected = _peopleRepository.UpdateLastName(personId, lastName);
                if (rowsAffected > 0)
                    return ServiceResult<int, enDoctorResult>.Success(rowsAffected, enDoctorResult.Success);
                return ServiceResult<int, enDoctorResult>.Failure(enDoctorResult.DatabaseError);
            }
            catch (Exception)
            {
                return ServiceResult<int, enDoctorResult>.Failure(enDoctorResult.OperationFailed);
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

        public ServiceResult<int, enDoctorResult> UpdateEmail(int personId, string email)
        {
            var validator = clsPeopleValidator.ValidateEmail(email);
            if (validator.Count > 0)
                return ServiceResult<int, enDoctorResult>.Failure(enDoctorResult.ValidationError, validationErrors: validator.Cast<Enum>().ToList());

            try
            {
                if (_peopleRepository.EmailExists(email, personId) > 0)
                    return ServiceResult<int, enDoctorResult>.Failure(enDoctorResult.ValidationError);

                int rowsAffected = _peopleRepository.UpdateEmail(personId, email);
                if (rowsAffected > 0)
                    return ServiceResult<int, enDoctorResult>.Success(rowsAffected, enDoctorResult.Success);
                return ServiceResult<int, enDoctorResult>.Failure(enDoctorResult.DatabaseError);
            }
            catch (Exception)
            {
                return ServiceResult<int, enDoctorResult>.Failure(enDoctorResult.OperationFailed);
            }
        }

        public ServiceResult<int, enDoctorResult> UpdateContactNumber(int personId, string contactNumber)
        {
            var validator = clsPeopleValidator.ValidateContactNumber(contactNumber);
            if (validator.Count > 0)
                return ServiceResult<int, enDoctorResult>.Failure(enDoctorResult.ValidationError, validationErrors: validator.Cast<Enum>().ToList());

            try
            {
                if (_peopleRepository.ContactNumberExists(contactNumber, personId) > 0)
                    return ServiceResult<int, enDoctorResult>.Failure(enDoctorResult.ValidationError);

                int rowsAffected = _peopleRepository.UpdateContactNumber(personId, contactNumber);
                if (rowsAffected > 0)
                    return ServiceResult<int, enDoctorResult>.Success(rowsAffected, enDoctorResult.Success);
                return ServiceResult<int, enDoctorResult>.Failure(enDoctorResult.DatabaseError);
            }
            catch (Exception)
            {
                return ServiceResult<int, enDoctorResult>.Failure(enDoctorResult.OperationFailed);
            }
        }

        public ServiceResult<int, enDoctorResult> UpdateDateOfBirth(int personId, DateTime dateOfBirth)
        {
            string formattedDate = dateOfBirth.ToString("yyyy-MM-dd");
            var validator = clsPeopleValidator.ValidateDateOfBirth(formattedDate);
            if (validator.Count > 0)
                return ServiceResult<int, enDoctorResult>.Failure(enDoctorResult.ValidationError, validationErrors: validator.Cast<Enum>().ToList());

            try
            {
                int rowsAffected = _peopleRepository.UpdateDateOfBirth(personId, dateOfBirth);
                if (rowsAffected > 0)
                    return ServiceResult<int, enDoctorResult>.Success(rowsAffected, enDoctorResult.Success);
                return ServiceResult<int, enDoctorResult>.Failure(enDoctorResult.DatabaseError);
            }
            catch (Exception)
            {
                return ServiceResult<int, enDoctorResult>.Failure(enDoctorResult.OperationFailed);
            }
        }

        public ServiceResult<int, enDoctorResult> UpdateAddress(int personId, string address)
        {
            try
            {
                int rowsAffected = _peopleRepository.UpdateAddress(personId, address);
                if (rowsAffected > 0)
                    return ServiceResult<int, enDoctorResult>.Success(rowsAffected, enDoctorResult.Success);
                return ServiceResult<int, enDoctorResult>.Failure(enDoctorResult.DatabaseError);
            }
            catch (Exception)
            {
                return ServiceResult<int, enDoctorResult>.Failure(enDoctorResult.OperationFailed);
            }
        }

        public ServiceResult<int, enDoctorResult> UpdateContactInfo(int personId, string phone, string email)
        {
            try
            {
                if (_peopleRepository.EmailExists(email, personId) > 0)
                    return ServiceResult<int, enDoctorResult>.Failure(enDoctorResult.ValidationError);

                if (_peopleRepository.ContactNumberExists(phone, personId) > 0)
                    return ServiceResult<int, enDoctorResult>.Failure(enDoctorResult.ValidationError);

                int rowsAffected = _peopleRepository.UpdateContactInfo(personId, phone, email);
                if (rowsAffected > 0)
                    return ServiceResult<int, enDoctorResult>.Success(rowsAffected, enDoctorResult.Success);
                return ServiceResult<int, enDoctorResult>.Failure(enDoctorResult.DatabaseError);
            }
            catch (Exception)
            {
                return ServiceResult<int, enDoctorResult>.Failure(enDoctorResult.OperationFailed);
            }
        }

        public ServiceResult<int, enDoctorResult> ChangeSpecialization(int doctorId, int newSpecializationId)
        {
            try
            {
                int rowsAffected = _doctorRepository.ChangeSpecialization(doctorId, newSpecializationId);
                return rowsAffected > 0
                    ? ServiceResult<int, enDoctorResult>.Success(rowsAffected, enDoctorResult.Success)
                    : ServiceResult<int, enDoctorResult>.Failure(enDoctorResult.DoctorNotFound);
            }
            catch (Exception)
            {
                return ServiceResult<int, enDoctorResult>.Failure(enDoctorResult.OperationFailed);
            }
        }

        public ServiceResult<int, enDoctorResult> UpdateConsultationFee(int doctorId, decimal newFee)
        {
            if (newFee < 0) return ServiceResult<int, enDoctorResult>.Failure(enDoctorResult.InvalidConsultationFee);
            try
            {
                int rowsAffected = _doctorRepository.UpdateConsultationFee(doctorId, newFee);
                return rowsAffected > 0
                    ? ServiceResult<int, enDoctorResult>.Success(rowsAffected, enDoctorResult.Success)
                    : ServiceResult<int, enDoctorResult>.Failure(enDoctorResult.DoctorNotFound);
            }
            catch (Exception)
            {
                return ServiceResult<int, enDoctorResult>.Failure(enDoctorResult.OperationFailed);
            }
        }

        public ServiceResult<int, enDoctorResult> SetAvailability(int doctorId, bool isAvailable)
        {
            try
            {
                int rowsAffected = _doctorRepository.SetAvailability(doctorId, isAvailable);
                return rowsAffected > 0
                    ? ServiceResult<int, enDoctorResult>.Success(rowsAffected, enDoctorResult.Success)
                    : ServiceResult<int, enDoctorResult>.Failure(enDoctorResult.DoctorNotFound);
            }
            catch (Exception)
            {
                return ServiceResult<int, enDoctorResult>.Failure(enDoctorResult.OperationFailed);
            }
        }

        public ServiceResult<DoctorDto, enDoctorResult> GetDoctorById(int doctorId)
        {
            try
            {
                var doctor = _doctorRepository.GetDoctorById(doctorId);
                if (doctor == null) return ServiceResult<DoctorDto, enDoctorResult>.Failure(enDoctorResult.DoctorNotFound);
                return ServiceResult<DoctorDto, enDoctorResult>.Success(_MapDoctorToDto(doctor), enDoctorResult.Success);
            }
            catch (Exception)
            {
                return ServiceResult<DoctorDto, enDoctorResult>.Failure(enDoctorResult.OperationFailed);
            }
        }

        public ServiceResult<List<DoctorDto>, enDoctorResult> GetAllDoctors()
        {
            try
            {
                var doctors = _doctorRepository.GetAllDoctors();
                return ServiceResult<List<DoctorDto>, enDoctorResult>.Success(_MapToDtoList(doctors), enDoctorResult.Success);
            }
            catch (Exception)
            {
                return ServiceResult<List<DoctorDto>, enDoctorResult>.Failure(enDoctorResult.OperationFailed);
            }
        }

        public ServiceResult<List<DoctorDto>, enDoctorResult> GetAvailableDoctors()
        {
            try
            {
                var doctors = _doctorRepository.GetAvailableDoctors();
                return ServiceResult<List<DoctorDto>, enDoctorResult>.Success(_MapToDtoList(doctors), enDoctorResult.Success);
            }
            catch (Exception)
            {
                return ServiceResult<List<DoctorDto>, enDoctorResult>.Failure(enDoctorResult.OperationFailed);
            }
        }

        public ServiceResult<List<DoctorDto>, enDoctorResult> GetDoctorsPaged(int page, int pageSize)
        {
            try
            {
                var doctors = _doctorRepository.GetDoctorsPaged(page, pageSize);
                return ServiceResult<List<DoctorDto>, enDoctorResult>.Success(_MapToDtoList(doctors), enDoctorResult.Success);
            }
            catch (Exception)
            {
                return ServiceResult<List<DoctorDto>, enDoctorResult>.Failure(enDoctorResult.OperationFailed);
            }
        }

        public ServiceResult<int, enDoctorResult> GetTotalDoctorsCount()
        {
            try
            {
                int count = _doctorRepository.GetTotalDoctorsCount();
                return ServiceResult<int, enDoctorResult>.Success(count, enDoctorResult.Success);
            }
            catch (Exception)
            {
                return ServiceResult<int, enDoctorResult>.Failure(enDoctorResult.OperationFailed);
            }
        }

        public ServiceResult<Dictionary<string, int>, enDoctorResult> GetCountPerSpecialization()
        {
            try
            {
                var stats = _doctorRepository.GetCountPerSpecialization();
                return ServiceResult<Dictionary<string, int>, enDoctorResult>.Success(stats, enDoctorResult.Success);
            }
            catch (Exception)
            {
                return ServiceResult<Dictionary<string, int>, enDoctorResult>.Failure(enDoctorResult.OperationFailed);
            }
        }

        public ServiceResult<List<DoctorDto>, enDoctorResult> SearchDoctors(string keyword)
        {
            try
            {
                var doctors = _doctorRepository.SearchDoctors(keyword);
                return ServiceResult<List<DoctorDto>, enDoctorResult>.Success(_MapToDtoList(doctors), enDoctorResult.Success);
            }
            catch (Exception)
            {
                return ServiceResult<List<DoctorDto>, enDoctorResult>.Failure(enDoctorResult.OperationFailed);
            }
        }

        private DoctorDto _MapDoctorToDto(Doctor doctor)
        {
            return new DoctorDto
            {
                DoctorId = doctor.DoctorId,
                FirstName = doctor.FirstName,
                LastName = doctor.LastName,
                Email = doctor.Email,
                ContactNumber = doctor.ContactNumber,
                SpecializationId = doctor.SpecializationId,
                SpecializationName = doctor.SpecializationName,
                Bio = doctor.Bio,
                ConsultationFee = doctor.ConsultationFee,
                IsAvailable = doctor.IsAvilable
            };
        }

        private List<DoctorDto> _MapToDtoList(List<Doctor> doctors)
        {
            var list = new List<DoctorDto>();
            if (doctors != null)
            {
                foreach (var d in doctors) list.Add(_MapDoctorToDto(d));
            }
            return list;
        }
    }
}