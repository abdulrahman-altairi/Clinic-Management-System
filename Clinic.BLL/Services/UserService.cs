using Clinic.BLL.Common.Result;
using Clinic.BLL.Enums;
using Clinic.BLL.Helper;
using Clinic.BLL.Results; 
using Clinic.BLL.Validators;
using Clinic.Contracts;
using Clinic.Contracts.Enums;
using Clinic.Contracts.Identity;
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

    public class clsUserService
    {
        private clsUserRepositroy _userRepositroy;
        private clsPeopleRepository _peopleRepository;

        public clsUserService()
        {
            _userRepositroy = new clsUserRepositroy();
            _peopleRepository = new clsPeopleRepository();
        }

        public ServiceResult<int, enUserResult> RegisterUser(RegisterRequestDto registerDto)
        {
            var validator = clsIdentityValidator.ValidateRegistration(registerDto);
            if (validator.Count > 0)
                return ServiceResult<int, enUserResult>.Failure(
                    enUserResult.ValidationError,
                    validationErrors: validator.Cast<Enum>().ToList()
                );


            using (var connection = DBHelper.GetOpenConnection())
            {
                using (var transaction = DBHelper.BeginTransaction(connection))
                {
                    try
                    {
                        if (_userRepositroy.UsernameExists(registerDto.Username, connection: connection, transaction: transaction) > 0)
                            return ServiceResult<int, enUserResult>.Failure(enUserResult.UsernameAlreadyExists);

                        if (_peopleRepository.EmailExists(registerDto.Email, connection: connection, transaction: transaction) > 0)
                            return ServiceResult<int, enUserResult>.Failure(enUserResult.EmailAlreadyExists);

                        Person person = new Person
                        {
                            FirstName = registerDto.FirstName,
                            LastName = registerDto.LastName,
                            Address = registerDto.Address,
                            ContactNumber = registerDto.ContactNumber,
                            DateOfBirth = registerDto.DateOfBirth,
                            Email = registerDto.Email,
                            Gender = (Entities.Enums.enGender)registerDto.Gender,
                        };
                        int personId = _peopleRepository.AddPerson(person, connection, transaction);

                        if (personId <= 0) throw new Exception("Person insertion failed");

                        var passwordHash = clsHashPassword.HashPassword(registerDto.Password);

                        User user = new User
                        {
                            UserId = personId,
                            UserName = registerDto.Username,
                            PasswordHash = passwordHash,
                            Role = (Entities.Enums.enRole)registerDto.Role,
                            IsActive = true,
                            LastLogin = DateTime.Now 
                        };

                        int rowsAffected = _userRepositroy.AddUser(user, connection, transaction);

                        if (rowsAffected > 0)
                        {
                            transaction.Commit();
                            return ServiceResult<int, enUserResult>.Success(personId, enUserResult.Success);
                        }

                        transaction.Rollback();
                        return ServiceResult<int, enUserResult>.Failure(enUserResult.DatabaseError);
                    }
                    catch (Exception)
                    {

                        transaction.Rollback();
                        return ServiceResult<int, enUserResult>.Failure(enUserResult.OperationFailed);
                    }
                }
            }
        }

        public ServiceResult<LoginResponseDto, enUserResult> Login(string username, string password)
        {

            var validator = clsIdentityValidator.ValidateLogin(username, password);
            if (validator.Count > 0)
                return ServiceResult<LoginResponseDto, enUserResult>.Failure(
                    enUserResult.ValidationError,
                    validationErrors: validator.Cast<Enum>().ToList()
                );

            try
            {
                var user = _userRepositroy.GetUserByUsername(username);

                if (user == null)
                    return ServiceResult<LoginResponseDto, enUserResult>.Failure(enUserResult.InvalidCredentials);

                if (!clsHashPassword.VerifyPassword(password, user.PasswordHash))
                    return ServiceResult<LoginResponseDto, enUserResult>.Failure(enUserResult.InvalidCredentials);

                if (!user.IsActive)
                    return ServiceResult<LoginResponseDto, enUserResult>.Failure(enUserResult.UserNotActive);

                var response = new LoginResponseDto
                {
                    UserID = user.UserId,
                    Username = user.UserName,
                    FullName = $"{user.FirstName} {user.LastName}",
                    Role = (enRole)user.Role 
                };

                _userRepositroy.UpdateLastLogin(user.UserId);

                return ServiceResult<LoginResponseDto, enUserResult>.Success(response, enUserResult.Success);
            }
            catch (Exception)
            {
                return ServiceResult<LoginResponseDto, enUserResult>.Failure(enUserResult.OperationFailed);
            }
        }

        public ServiceResult<int, enUserResult> UpdateUsername(int userId, string username)
        {
            try
            {
                if (_userRepositroy.UserExists(userId) == 0)
                    return ServiceResult<int, enUserResult>.Failure(enUserResult.UserNotFount);

                if (_userRepositroy.UsernameExists(username) > 0) 
                    return ServiceResult<int, enUserResult>.Failure(enUserResult.UsernameAlreadyExists);

                int rowAffected = _userRepositroy.UpdateUsername(userId, username);

                if (rowAffected > 0)
                    return ServiceResult<int, enUserResult>.Success(rowAffected, enUserResult.Success); 

                return ServiceResult<int, enUserResult>.Failure(enUserResult.DatabaseError);
            }
            catch(Exception)
            {
                return ServiceResult<int, enUserResult>.Failure(enUserResult.OperationFailed);
            }
        }

        public ServiceResult<int, enUserResult> UpdateUserRole(int userId, enRole role)
        {
            try
            {
                if (_userRepositroy.UserExists(userId) == 0)
                    return ServiceResult<int, enUserResult>.Failure(enUserResult.UserNotFount);

                int rowAffected = _userRepositroy.UpdateUserRole(userId, (Entities.Enums.enRole)role);

                if (rowAffected > 0)
                    return ServiceResult<int, enUserResult>.Success(rowAffected, enUserResult.Success); 

                return ServiceResult<int, enUserResult>.Failure(enUserResult.DatabaseError);
            }
            catch(Exception)
            {
                return ServiceResult<int, enUserResult>.Failure(enUserResult.OperationFailed);
            }
        }

        public ServiceResult<int, enUserResult> ChangePassword(int userId, string newPasswordHash)
        {
            try
            {
                if (_userRepositroy.UserExists(userId) == 0)
                    return ServiceResult<int, enUserResult>.Failure(enUserResult.UserNotFount);

                var passwordHash = clsHashPassword.HashPassword(newPasswordHash);

                int rowAffected = _userRepositroy.ChangePassword(userId, passwordHash);

                if (rowAffected > 0)
                    return ServiceResult<int, enUserResult>.Success(rowAffected, enUserResult.Success); 

                return ServiceResult<int, enUserResult>.Failure(enUserResult.DatabaseError);
            }
            catch (Exception)
            {
                return ServiceResult<int, enUserResult>.Failure(enUserResult.OperationFailed);
            }
        }

        public ServiceResult<int, enUserResult> ActivateUser(int userId)
        {
            try
            {
                if (_userRepositroy.UserExists(userId) == 0)
                    return ServiceResult<int, enUserResult>.Failure(enUserResult.UserNotFount);

                int rowAffected = _userRepositroy.ActivateUser(userId);

                if (rowAffected > 0)
                    return ServiceResult<int, enUserResult>.Success(rowAffected, enUserResult.Success); 

                return ServiceResult<int, enUserResult>.Failure(enUserResult.DatabaseError);
            }
            catch (Exception)
            {
                return ServiceResult<int, enUserResult>.Failure(enUserResult.OperationFailed);
            }
        }

        public ServiceResult<int, enUserResult> DeactivateUser(int userId)
        {
            try
            {
                if (_userRepositroy.UserExists(userId) == 0)
                    return ServiceResult<int, enUserResult>.Failure(enUserResult.UserNotFount);

                int rowAffected = _userRepositroy.DeactivateUser(userId);

                if (rowAffected > 0)
                    return ServiceResult<int, enUserResult>.Success(rowAffected, enUserResult.Success); 

                return ServiceResult<int, enUserResult>.Failure(enUserResult.DatabaseError);
            }
            catch (Exception)
            {
                return ServiceResult<int, enUserResult>.Failure(enUserResult.OperationFailed);
            }
        }

        public ServiceResult<int, enUserResult> UpdateFirstName(int personId, string firstName)
        {
            var validatetor = clsPeopleValidator.ValidateFirstName(firstName);
            if (validatetor.Count > 0)
                return ServiceResult<int, enUserResult>.Failure(
                    enUserResult.ValidationError, 
                    validationErrors: validatetor.Cast<Enum>().ToList()
                );

            try
            {
                int rowsAffected = _peopleRepository.UpdateFirstName(personId, firstName);
                if (rowsAffected > 0)
                    return ServiceResult<int, enUserResult>.Success(rowsAffected, enUserResult.Success);
                return ServiceResult<int, enUserResult>.Failure(enUserResult.DatabaseError);
            }
            catch (Exception)
            {
                return ServiceResult<int, enUserResult>.Failure(enUserResult.OperationFailed);
            }
        }

        public ServiceResult<int, enUserResult> UpdateGender(int personId, enGender gender)
        {
            var validator = clsPeopleValidator.ValidateGender(gender);
            if (validator.Count > 0)
                return ServiceResult<int, enUserResult>.Failure(
                    enUserResult.ValidationError,
                    validationErrors: validator.Cast<Enum>().ToList()
                );

            try
            {
                int rowsAffected = _peopleRepository.UpdateGender(personId, (Entities.Enums.enGender)gender);

                if (rowsAffected > 0)
                    return ServiceResult<int, enUserResult>.Success(rowsAffected, enUserResult.Success);

                return ServiceResult<int, enUserResult>.Failure(enUserResult.DatabaseError);
            }
            catch (Exception)
            {
                return ServiceResult<int, enUserResult>.Failure(enUserResult.OperationFailed);
            }
        }

        public ServiceResult<int, enUserResult> UpdateLastName(int personId, string lastName)
        {
            var validator = clsPeopleValidator.ValidateLastName(lastName);
            if (validator.Count > 0)
                return ServiceResult<int, enUserResult>.Failure(
                    enUserResult.ValidationError,
                    validationErrors: validator.Cast<Enum>().ToList()
                );

            try
            {
                int rowsAffected = _peopleRepository.UpdateLastName(personId, lastName);

                if (rowsAffected > 0)
                    return ServiceResult<int, enUserResult>.Success(rowsAffected, enUserResult.Success);

                return ServiceResult<int, enUserResult>.Failure(enUserResult.DatabaseError);
            }
            catch (Exception)
            {
                return ServiceResult<int, enUserResult>.Failure(enUserResult.OperationFailed);
            }
        }

        public ServiceResult<int, enUserResult> UpdateEmail(int personId, string email)
        {
            var validatetor = clsPeopleValidator.ValidateEmail(email);
            if (validatetor.Count > 0)
                return ServiceResult<int, enUserResult>.Failure(
                    enUserResult.ValidationError,
                    validationErrors: validatetor.Cast<Enum>().ToList()
                );

            try
            {
                if (_peopleRepository.EmailExists(email, personId) > 0)
                
                    return ServiceResult<int, enUserResult>.Failure(enUserResult.EmailAlreadyExists);
                
                int rowsAffected = _peopleRepository.UpdateEmail(personId, email);

                if (rowsAffected > 0)
                    return ServiceResult<int, enUserResult>.Success(rowsAffected, enUserResult.Success);

                return ServiceResult<int, enUserResult>.Failure(enUserResult.DatabaseError);
            }
            catch (Exception)
            {
                return ServiceResult<int, enUserResult>.Failure(enUserResult.OperationFailed);
            }
        }

        public ServiceResult<int, enUserResult> UpdateContactNumber(int personId, string contactNumber)
        {
            var validator = clsPeopleValidator.ValidateContactNumber(contactNumber);
            if (validator.Count > 0)
                return ServiceResult<int, enUserResult>.Failure(
                    enUserResult.ValidationError,
                    validationErrors: validator.Cast<Enum>().ToList()
                );

            try
            {
                if (_peopleRepository.ContactNumberExists(contactNumber, personId) > 0)
                    return ServiceResult<int, enUserResult>.Failure(enUserResult.ContactNumberAlreadyExists);

                int rowsAffected = _peopleRepository.UpdateContactNumber(personId, contactNumber);

                if (rowsAffected > 0)
                    return ServiceResult<int, enUserResult>.Success(rowsAffected, enUserResult.Success);

                return ServiceResult<int, enUserResult>.Failure(enUserResult.DatabaseError);
            }
            catch (Exception)
            {
                return ServiceResult<int, enUserResult>.Failure(enUserResult.OperationFailed);
            }
        }

        public ServiceResult<int, enUserResult> UpdateDateOfBirth(int personId, DateTime dateOfBirth)
        {
            string formattedDate = dateOfBirth.ToString("yyyy-MM-dd");

            var validator = clsPeopleValidator.ValidateDateOfBirth(formattedDate);
            if (validator.Count > 0)
                return ServiceResult<int, enUserResult>.Failure(
                    enUserResult.ValidationError,
                    validationErrors: validator.Cast<Enum>().ToList()
                );

            try
            {
                int rowsAffected = _peopleRepository.UpdateDateOfBirth(personId, dateOfBirth);

                if (rowsAffected > 0)
                    return ServiceResult<int, enUserResult>.Success(rowsAffected, enUserResult.Success);

                return ServiceResult<int, enUserResult>.Failure(enUserResult.DatabaseError);
            }
            catch (Exception)
            {
                return ServiceResult<int, enUserResult>.Failure(enUserResult.OperationFailed);
            }
        }

        public ServiceResult<int, enUserResult> UpdateAddress(int personId, string address)
        {
            try
            {
                int rowsAffected = _peopleRepository.UpdateAddress(personId, address);

                if (rowsAffected > 0)
                    return ServiceResult<int, enUserResult>.Success(rowsAffected, enUserResult.Success);

                return ServiceResult<int, enUserResult>.Failure(enUserResult.DatabaseError);
            }
            catch (Exception)
            {
                return ServiceResult<int, enUserResult>.Failure(enUserResult.OperationFailed);
            }
        }

        public ServiceResult<int, enUserResult> UpdateContactInfo(int personId, string phone, string email)
        {
            try
            {
                if (_peopleRepository.EmailExists(email, personId) > 0)
                    return ServiceResult<int, enUserResult>.Failure(enUserResult.EmailAlreadyExists);

                if (_peopleRepository.ContactNumberExists(phone, personId) > 0)
                    return ServiceResult<int, enUserResult>.Failure(enUserResult.ContactNumberAlreadyExists);

                int rowsAffected = _peopleRepository.UpdateContactInfo(personId, phone, email);

                if (rowsAffected > 0)
                    return ServiceResult<int, enUserResult>.Success(rowsAffected, enUserResult.Success);

                return ServiceResult<int, enUserResult>.Failure(enUserResult.DatabaseError);
            }
            catch (Exception)
            {
                return ServiceResult<int, enUserResult>.Failure(enUserResult.OperationFailed);
            }
        }

        public ServiceResult<int, enUserResult> SoftDeletePerson(int personId)
        {
            try
            {
                if (_peopleRepository.PersonExists(personId) == 0)
                    return ServiceResult<int, enUserResult>.Failure(enUserResult.UserNotFount);

                int rowsAffected = _peopleRepository.SoftDeletePerson(personId);

                if (rowsAffected > 0)
                    return ServiceResult<int, enUserResult>.Success(rowsAffected, enUserResult.Success);

                return ServiceResult<int, enUserResult>.Failure(enUserResult.DatabaseError);
            }
            catch (Exception)
            {
                return ServiceResult<int, enUserResult>.Failure(enUserResult.OperationFailed);
            }
        }

        public ServiceResult<int, enUserResult> RestoreUser(int personId)
        {
            try
            {
                if (_peopleRepository.PersonExists(personId) == 0)
                    return ServiceResult<int, enUserResult>.Failure(enUserResult.UserNotFount);

                int rowsAffected = _peopleRepository.RestorePerson(personId);

                if (rowsAffected > 0)
                    return ServiceResult<int, enUserResult>.Success(rowsAffected, enUserResult.Success);

                return ServiceResult<int, enUserResult>.Failure(enUserResult.DatabaseError);
            }
            catch (Exception)
            {
                return ServiceResult<int, enUserResult>.Failure(enUserResult.OperationFailed);
            }
        }

        public ServiceResult<UserDto, enUserResult> GetUserById(int userId)
        {
            try
            {
                User user = _userRepositroy.GetUserById(userId);
                if (user != null)
                {
                    UserDto userDto = _MapUserToUserDto(user);
                    return ServiceResult<UserDto, enUserResult>.Success(userDto, enUserResult.Success);
                }

                return ServiceResult<UserDto, enUserResult>.Failure(enUserResult.UserNotFount);
            }
            catch (Exception)
            {
                return ServiceResult<UserDto, enUserResult>.Failure(enUserResult.OperationFailed);
            }
        }

        public ServiceResult<UserDto, enUserResult> GetUserByUsername(string username)
        {
            try
            {
                User user = _userRepositroy.GetUserByUsername(username);

                if (user == null)
                    return ServiceResult<UserDto, enUserResult>.Failure(enUserResult.UserNotFount);

                UserDto userDto = _MapUserToUserDto(user);
                return ServiceResult<UserDto, enUserResult>.Success(userDto, enUserResult.Success);
            }
            catch (Exception)
            {
                return ServiceResult<UserDto, enUserResult>.Failure(enUserResult.OperationFailed);
            }
        }

        public ServiceResult<List<UserDto>, enUserResult> GetActiveUsers()
        {
            try
            {
                List<User> users = _userRepositroy.GetActiveUsers();
                if (users != null)
                {
                    List<UserDto> usersDto = _MapToDtoList(users);
                    return ServiceResult<List<UserDto>, enUserResult>.Success(usersDto, enUserResult.Success);
                }
                return ServiceResult<List<UserDto>, enUserResult>.Failure(enUserResult.DatabaseError);
            }
            catch (Exception)
            {
                return ServiceResult<List<UserDto>, enUserResult>.Failure(enUserResult.OperationFailed);

            }

        }

        public ServiceResult<List<UserDto>, enUserResult> GetUsersPaged(int pageNumber, int pageSize)
        {
            try
            {
                List<User> users = _userRepositroy.GetUsersPaged(pageNumber, pageSize);
                if (users != null)
                {
                    List<UserDto> usersDto = _MapToDtoList(users);
                    return ServiceResult<List<UserDto>, enUserResult>.Success(usersDto, enUserResult.Success);
                }
                return ServiceResult<List<UserDto>, enUserResult>.Failure(enUserResult.DatabaseError);
            }
            catch (Exception)
            {
                return ServiceResult<List<UserDto>, enUserResult>.Failure(enUserResult.OperationFailed);

            }

        }

        private UserDto _MapUserToUserDto(User user)
        {
            return new UserDto
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Address = user.Address,
                ContactNumber = user.ContactNumber,
                Email = user.Email,
                DateOfBirth = user.DateOfBirth ?? DateTime.MinValue, 
                Gender = (enGender)user.Gender,

                //  User
                UserID = user.UserId,
                Username = user.UserName,
                Role = (enRole)user.Role,
                IsActive = user.IsActive,
                LastLogin = user.LastLogin,
                PersonId = user.UserId 
            };
        }

        private List<UserDto> _MapToDtoList(List<User> user)
        {
            var list = new List<UserDto>();
            if (user != null)
            {
                foreach (var p in user) list.Add(_MapUserToUserDto(p));
            }
            return list;
        }
    }
}
