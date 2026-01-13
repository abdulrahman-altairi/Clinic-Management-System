using Clinic.BLL.Common.Result;
using Clinic.BLL.Results;
using Clinic.BLL.Validators;
using Clinic.Contracts;
using Clinic.Contracts.Enums;
using Clinic.DAL.Repositories;
using Clinic.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Clinic.BLL.Services
{
    public class clsPeopleService
    {
        private clsPeopleRepository _peopleRepository;

        public clsPeopleService()
        {
            _peopleRepository = new clsPeopleRepository();
        }

        public ServiceResult<int, enPeopleResult> CreatePerson(PersonDto personDto)
        {
            var validatetor = clsPeopleValidator.ValidatePerson(personDto);

            if (validatetor.Count > 0)
                return ServiceResult<int, enPeopleResult>.Failure(
                    enPeopleResult.ValidationError,
                    validationErrors: validatetor.Cast<Enum>().ToList()
                );
            try
            {


                if (_peopleRepository.EmailExists(personDto.Email) > 0)
                    return ServiceResult<int, enPeopleResult>.Failure(enPeopleResult.EmailAlreadyExists);

                if(_peopleRepository.ContactNumberExists(personDto.ContactNumber) > 0)
                    return ServiceResult<int, enPeopleResult>.Failure(enPeopleResult.ContactNumberAlreadyExists);

                Person person = _MapToEntity(personDto);
                int personId = 0;
                personId = _peopleRepository.AddPerson(person);

                if (personId > 0)
                    return ServiceResult<int, enPeopleResult>.Success(personId, enPeopleResult.Success);
                return ServiceResult<int, enPeopleResult>.Failure(enPeopleResult.DatabaseError);

            }
            catch (Exception)
            {
                return ServiceResult<int, enPeopleResult>.Failure(enPeopleResult.OperationFailed);
            }
        }

        public ServiceResult<int, enPeopleResult> UpdatePerson(PersonDto personDto)
        {
           var validatetor = clsPeopleValidator.ValidatePerson(personDto);

           if (validatetor.Count > 0)
               return ServiceResult<int, enPeopleResult>.Failure(
                   enPeopleResult.ValidationError,
                   validationErrors: validatetor.Cast<Enum>().ToList()
               );

            try
            {

                if (_peopleRepository.EmailExists(personDto.Email, personDto.PersonId) > 0)
                    return ServiceResult<int, enPeopleResult>.Failure(enPeopleResult.EmailAlreadyExists);

                if (_peopleRepository.ContactNumberExists(personDto.ContactNumber, personDto.PersonId) > 0)
                    return ServiceResult<int, enPeopleResult>.Failure(enPeopleResult.ContactNumberAlreadyExists);

                Person person = _MapToEntity(personDto);
                int rowsAffected = 0;
                rowsAffected = _peopleRepository.UpdatePerson(person);

                if (rowsAffected > 0)
                    return ServiceResult<int, enPeopleResult>.Success(rowsAffected, enPeopleResult.Success);
                return ServiceResult<int, enPeopleResult>.Failure(enPeopleResult.DatabaseError);
            }
            catch (Exception)
            {
                return ServiceResult<int, enPeopleResult>.Failure(enPeopleResult.OperationFailed);
            }
        }

        public ServiceResult<int, enPeopleResult> UpdateFirstName(int personId, string firstName)
        {
           var validatetor = clsPeopleValidator.ValidateFirstName(firstName);
           if (validatetor.Count > 0)
               return ServiceResult<int, enPeopleResult>.Failure(
                   enPeopleResult.ValidationError,
                   validationErrors: validatetor.Cast<Enum>().ToList()
               );

            try
            {

                int rowsAffected = 0;
                rowsAffected = _peopleRepository.UpdateFirstName(personId, firstName);

                if(rowsAffected > 0)
                    return ServiceResult<int, enPeopleResult>.Success(rowsAffected, enPeopleResult.Success);
                return ServiceResult<int, enPeopleResult>.Failure(enPeopleResult.DatabaseError);
            }
            catch (Exception)
            {
                return ServiceResult<int, enPeopleResult>.Failure(enPeopleResult.OperationFailed);
            }
        }

        public ServiceResult<int, enPeopleResult> UpdateGender(int personId, enGender gender)
        {
            var validatetor = clsPeopleValidator.ValidateGender(gender);
            if (validatetor.Count > 0)
                return ServiceResult<int, enPeopleResult>.Failure(
                    enPeopleResult.ValidationError,
                    validationErrors: validatetor.Cast<Enum>().ToList()
                );
            try
            {
                int rowsAffected = 0;
                rowsAffected = _peopleRepository.UpdateGender(personId, (Entities.Enums.enGender)gender);

                if (rowsAffected > 0)
                    return ServiceResult<int, enPeopleResult>.Success(rowsAffected, enPeopleResult.Success);
                return ServiceResult<int, enPeopleResult>.Failure(enPeopleResult.DatabaseError);
            }
            catch (Exception)
            {
                return ServiceResult<int, enPeopleResult>.Failure(enPeopleResult.OperationFailed);
            }
        }

        public ServiceResult<int, enPeopleResult> UpdateLastName(int personId, string lastName)
        {
           var validatetor = clsPeopleValidator.ValidateLastName(lastName);
           if (validatetor.Count > 0)
               return ServiceResult<int, enPeopleResult>.Failure(
                   enPeopleResult.ValidationError,
                   validationErrors: validatetor.Cast<Enum>().ToList()
               );

            try
            {
                int rowsAffected = 0;
                rowsAffected = _peopleRepository.UpdateLastName(personId, lastName);

                if (rowsAffected > 0)
                    return ServiceResult<int, enPeopleResult>.Success(rowsAffected, enPeopleResult.Success);
                return ServiceResult<int, enPeopleResult>.Failure(enPeopleResult.DatabaseError);
            }
            catch (Exception)
            {
                return ServiceResult<int, enPeopleResult>.Failure(enPeopleResult.OperationFailed);
            }
        }

        public ServiceResult<int, enPeopleResult> UpdateEmail(int personId, string email)
        {
           var validatetor = clsPeopleValidator.ValidateEmail(email);
           if (validatetor.Count > 0)
               return ServiceResult<int, enPeopleResult>.Failure(
                   enPeopleResult.ValidationError,
                   validationErrors: validatetor.Cast<Enum>().ToList()
               );
            try
            {
                if (_peopleRepository.EmailExists(email, personId) > 0)
                    return ServiceResult<int, enPeopleResult>.Failure(enPeopleResult.EmailAlreadyExists);
                int rowsAffected = 0;
                rowsAffected = _peopleRepository.UpdateEmail(personId, email);

                if (rowsAffected > 0)
                    return ServiceResult<int, enPeopleResult>.Success(rowsAffected, enPeopleResult.Success);
                return ServiceResult<int, enPeopleResult>.Failure(enPeopleResult.DatabaseError);
            }
            catch (Exception)
            {
                return ServiceResult<int, enPeopleResult>.Failure(enPeopleResult.OperationFailed);
            }
        }

        public ServiceResult<int, enPeopleResult> UpdateContactNumber(int personId, string contactNumber)
        {
            var validatetor = clsPeopleValidator.ValidateContactNumber(contactNumber);
            if (validatetor.Count > 0)
                return ServiceResult<int, enPeopleResult>.Failure(
                    enPeopleResult.ValidationError,
                    validationErrors: validatetor.Cast<Enum>().ToList()
                );
            try
            {
                if (_peopleRepository.ContactNumberExists(contactNumber, personId) > 0)
                    return ServiceResult<int, enPeopleResult>.Failure(enPeopleResult.ContactNumberAlreadyExists);
                int rowsAffected = 0;
                rowsAffected = _peopleRepository.UpdateContactNumber(personId, contactNumber);

                if (rowsAffected > 0)
                    return ServiceResult<int, enPeopleResult>.Success(rowsAffected, enPeopleResult.Success);
                return ServiceResult<int, enPeopleResult>.Failure(enPeopleResult.DatabaseError);
            }
            catch (Exception)
            {
                return ServiceResult<int, enPeopleResult>.Failure(enPeopleResult.OperationFailed);
            }
        }

        public ServiceResult<int, enPeopleResult> UpdateDateOfBirth(int personId, DateTime dateOfBirth)
        {
            string formattedDate = dateOfBirth.ToString("yyyy-MM-dd");

            var validator = clsPeopleValidator.ValidateDateOfBirth(formattedDate);
            if (validator.Count > 0)
               return ServiceResult<int, enPeopleResult>.Failure(
                   enPeopleResult.ValidationError,
                   validationErrors: validator.Cast<Enum>().ToList()
               );
            try
            { 

                int rowsAffected = 0;
                rowsAffected = _peopleRepository.UpdateDateOfBirth(personId, dateOfBirth);

                if (rowsAffected > 0)
                    return ServiceResult<int, enPeopleResult>.Success(rowsAffected, enPeopleResult.Success);
                return ServiceResult<int, enPeopleResult>.Failure(enPeopleResult.DatabaseError);
            }
            catch (Exception)
            {
                return ServiceResult<int, enPeopleResult>.Failure(enPeopleResult.OperationFailed);
            }
        }

        public ServiceResult<int, enPeopleResult> UpdateAddress(int personId, string adderss)
        {
            try
            {
                int rowsAffected = 0;
                rowsAffected = _peopleRepository.UpdateAddress(personId, adderss);

                if (rowsAffected > 0)
                    return ServiceResult<int, enPeopleResult>.Success(rowsAffected, enPeopleResult.Success);
                return ServiceResult<int, enPeopleResult>.Failure(enPeopleResult.DatabaseError);
            }
            catch (Exception)
            {
                return ServiceResult<int, enPeopleResult>.Failure(enPeopleResult.OperationFailed);
            }
        }

        public ServiceResult<int, enPeopleResult> UpdateContactInfo(int personId, string phone, string email)
        {
            try
            {
                if (_peopleRepository.EmailExists(email, personId) > 0)
                    return ServiceResult<int, enPeopleResult>.Failure(enPeopleResult.EmailAlreadyExists);

                if (_peopleRepository.ContactNumberExists(phone, personId) > 0)
                    return ServiceResult<int, enPeopleResult>.Failure(enPeopleResult.ContactNumberAlreadyExists);

                int rowsAffected = 0;
                rowsAffected = _peopleRepository.UpdateContactInfo(personId, phone, email);

                if (rowsAffected > 0)
                    return ServiceResult<int, enPeopleResult>.Success(rowsAffected, enPeopleResult.Success);
                return ServiceResult<int, enPeopleResult>.Failure(enPeopleResult.DatabaseError);
            }
            catch (Exception)
            {
                return ServiceResult<int, enPeopleResult>.Failure(enPeopleResult.OperationFailed);
            }
        }

        public ServiceResult<int, enPeopleResult> SoftDeletePerson(int personId)
        {
            try
            {
                if (_peopleRepository.PersonExists(personId) == 0)
                    return ServiceResult<int, enPeopleResult>.Failure(enPeopleResult.PersonNotFound);
                int rowsAffected = 0;
                rowsAffected = _peopleRepository.SoftDeletePerson(personId);

                if (rowsAffected > 0)
                    return ServiceResult<int, enPeopleResult>.Success(rowsAffected, enPeopleResult.Success);
                return ServiceResult<int, enPeopleResult>.Failure(enPeopleResult.DatabaseError);
            }
            catch (Exception)
            {
                return ServiceResult<int, enPeopleResult>.Failure(enPeopleResult.OperationFailed);
            }
        }

        public ServiceResult<int, enPeopleResult> RestorePerson(int personId)
        {
            try
            {
                if (_peopleRepository.PersonExists(personId) == 0)
                    return ServiceResult<int, enPeopleResult>.Failure(enPeopleResult.PersonNotFound);
                int rowsAffected = 0;
                rowsAffected = _peopleRepository.RestorePerson(personId);

                if (rowsAffected > 0)
                    return ServiceResult<int, enPeopleResult>.Success(rowsAffected, enPeopleResult.Success);
                return ServiceResult<int, enPeopleResult>.Failure(enPeopleResult.DatabaseError);
            }
            catch (Exception)
            {
                return ServiceResult<int, enPeopleResult>.Failure(enPeopleResult.OperationFailed);
            }
        }

        public ServiceResult<PersonDto, enPeopleResult> GetPersonById(int personId) 
        {
            try
            {
                if (_peopleRepository.PersonExists(personId) == 0)
                    return ServiceResult<PersonDto, enPeopleResult>.Failure(enPeopleResult.PersonNotFound);
                Person person = _peopleRepository.GetPersonById(personId);
                if (person != null)
                {
                    PersonDto personDto = _MapToDto(person);
                    return ServiceResult<PersonDto, enPeopleResult>.Success(personDto, enPeopleResult.Success);
                }
                return ServiceResult<PersonDto, enPeopleResult>.Failure(enPeopleResult.PersonNotFound);
            }
            catch (Exception)
            {
                return ServiceResult<PersonDto, enPeopleResult>.Failure(enPeopleResult.OperationFailed);
            }
        }

        public ServiceResult<List<PersonDto>, enPeopleResult> GetAllPeople()
        {
            try
            {
                List<Person> person = _peopleRepository.GetAllPeople();
                if (person != null)
                {
                    List<PersonDto> personDto = _MapToDtoList(person);
                    return ServiceResult<List<PersonDto>, enPeopleResult>.Success(personDto, enPeopleResult.Success);
                }
                return ServiceResult<List<PersonDto>, enPeopleResult>.Failure(enPeopleResult.PersonNotFound);
            }
            catch (Exception)
            {
                return ServiceResult<List<PersonDto>, enPeopleResult>.Failure(enPeopleResult.OperationFailed);
            }
        }
        
        public ServiceResult<List<PersonDto>, enPeopleResult> GetPeoplePaged(int pageNumber, int pageSize)
        {
            try
            {
                List<Person> person = _peopleRepository.GetPeoplePaged(pageNumber, pageSize);
                if (person != null)
                {
                    List<PersonDto> personDto = _MapToDtoList(person);
                    return ServiceResult<List<PersonDto>, enPeopleResult>.Success(personDto, enPeopleResult.Success);
                }
                return ServiceResult<List<PersonDto>, enPeopleResult>.Failure(enPeopleResult.PersonNotFound);
            }
            catch (Exception)
            {
                return ServiceResult<List<PersonDto>, enPeopleResult>.Failure(enPeopleResult.OperationFailed);
            }
        }

        public ServiceResult<List<PersonDto>, enPeopleResult> SearchPeople(string keyWord)
        {
            try
            {
                List<Person> person = _peopleRepository.SearchPeople(keyWord);
                if (person != null)
                {
                    List<PersonDto> personDto = _MapToDtoList(person);
                    return ServiceResult<List<PersonDto>, enPeopleResult>.Success(personDto, enPeopleResult.Success);
                }
                return ServiceResult<List<PersonDto>, enPeopleResult>.Failure(enPeopleResult.PersonNotFound);
            }
            catch (Exception)
            {
                return ServiceResult<List<PersonDto>, enPeopleResult>.Failure(enPeopleResult.OperationFailed);
            }
        }

        public ServiceResult<PersonDto, enPeopleResult> GetPersonByEmail(string email)
        {
            try
            {
                Person person = _peopleRepository.GetPersonByEmail(email);
                if (person != null)
                {
                    PersonDto personDto = _MapToDto(person);
                    return ServiceResult<PersonDto, enPeopleResult>.Success(personDto, enPeopleResult.Success);
                }
                return ServiceResult<PersonDto, enPeopleResult>.Failure(enPeopleResult.PersonNotFound);
            }
            catch (Exception)
            {
                return ServiceResult<PersonDto, enPeopleResult>.Failure(enPeopleResult.OperationFailed);
            }
        }

        public ServiceResult<int, enPeopleResult> GetTotalPeopleCount()
        {
            try
            {
                int result = _peopleRepository.GetTotalPeopleCount();
                if (result > 0)
                {
                    return ServiceResult<int, enPeopleResult>.Success(result, enPeopleResult.Success);
                }
                return ServiceResult<int, enPeopleResult>.Failure(enPeopleResult.PersonNotFound);
            }
            catch (Exception)
            {
                return ServiceResult<int, enPeopleResult>.Failure(enPeopleResult.OperationFailed);
            }
        }

        private Person _MapToEntity(PersonDto personDto)
        {
            return new Person
            {
                PersonId = personDto.PersonId,
                FirstName = personDto.FirstName,
                LastName = personDto.LastName,
                Email = personDto.Email,
                ContactNumber = personDto.ContactNumber,
                DateOfBirth = personDto.DateOfBirth,
                Address = string.IsNullOrWhiteSpace(personDto.Address) ? null : personDto.Address,
                Gender = (Entities.Enums.enGender)personDto.Gender,
            };
        }

        private PersonDto _MapToDto(Person person)
        {
            return new PersonDto
            {
                PersonId = person.PersonId,
                FirstName = person.FirstName,
                LastName = person.LastName,
                Email = person.Email,
                Address = person.Address,
                ContactNumber = person.ContactNumber,
                DateOfBirth = (DateTime)person.DateOfBirth,
                Gender = (enGender)person.Gender
            };
        }
        private List<PersonDto> _MapToDtoList(List<Person> people)
        {
            var list = new List<PersonDto>();
            if (people != null)
            {
                foreach (var p in people) list.Add(_MapToDto(p));
            }
            return list;
        }
    }
}
