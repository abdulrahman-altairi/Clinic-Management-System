using Clinic.BLL.Common.Result;
using Clinic.BLL.Enums;
using Clinic.BLL.Validators;
using Clinic.Contracts;
using Clinic.DAL.Repositories;
using Clinic.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Clinic.BLL.Services
{
    public class clsSpecializationService
    {
        private readonly clsSpecializationsRepositroy _repository;

        public clsSpecializationService()
        {
            _repository = new clsSpecializationsRepositroy();
        }

        public ServiceResult<int, enSpecializationResult> AddSpecialization(SpecializationDto specDto)
        {
            var validatetor = clsMidecalValidator.ValidateSpecialization(specDto);
            if (validatetor.Count > 0)
                return ServiceResult<int, enSpecializationResult>.Failure(
                    enSpecializationResult.ValidationError,
                    validationErrors: validatetor.Cast<Enum>().ToList()
                );
            try
            {

                if (_repository.SpecializationExists(specDto.SpecializationName) > 0)
                {
                    return ServiceResult<int, enSpecializationResult>.Failure(enSpecializationResult.DuplicateName);
                }

                Specialization spec = new Specialization
                {
                    SpecializationName = specDto.SpecializationName,
                    SpecializationDescription = specDto.SpecializationDescription
                };

                int newId = _repository.AddSpecialization(spec);
                if (newId > 0)
                    return ServiceResult<int, enSpecializationResult>.Success(newId, enSpecializationResult.Success);

                return ServiceResult<int, enSpecializationResult>.Failure(enSpecializationResult.DatabaseError);
            }
            catch (Exception)
            {
                return ServiceResult<int, enSpecializationResult>.Failure(enSpecializationResult.OperationFailed);
            }
        }

        public ServiceResult<int, enSpecializationResult> UpdateSpecialization(SpecializationDto specDto)
        {
            var validatetor = clsMidecalValidator.ValidateSpecialization(specDto);
            if (validatetor.Count > 0)
                return ServiceResult<int, enSpecializationResult>.Failure(
                    enSpecializationResult.ValidationError,
                    validationErrors: validatetor.Cast<Enum>().ToList()
                );
            try
            {

                var existingSpec = _repository.GetById(specDto.SpecializationId);
                if (existingSpec == null)
                    return ServiceResult<int, enSpecializationResult>.Failure(enSpecializationResult.NotFound);

                if (_repository.SpecializationExists(specDto.SpecializationName, specDto.SpecializationId) > 0)
                    return ServiceResult<int, enSpecializationResult>.Failure(enSpecializationResult.DuplicateName);

                existingSpec.SpecializationName = specDto.SpecializationName;
                existingSpec.SpecializationDescription = specDto.SpecializationDescription;

                int rowsAffected = _repository.UpdateSpecialization(existingSpec);
                return rowsAffected > 0
                    ? ServiceResult<int, enSpecializationResult>.Success(rowsAffected, enSpecializationResult.Success)
                    : ServiceResult<int, enSpecializationResult>.Failure(enSpecializationResult.DatabaseError);
            }
            catch (Exception)
            {
                return ServiceResult<int, enSpecializationResult>.Failure(enSpecializationResult.OperationFailed);
            }
        }
        
        public ServiceResult<int, enSpecializationResult> DeleteSpecialization(int specId)
        {
            try
            {
                int result = _repository.DeleteSpecialization(specId);

                if (result == -1) 
                    return ServiceResult<int, enSpecializationResult>.Failure(enSpecializationResult.HasRelatedDoctors);

                if (result > 0)
                    return ServiceResult<int, enSpecializationResult>.Success(result, enSpecializationResult.Success);

                return ServiceResult<int, enSpecializationResult>.Failure(enSpecializationResult.NotFound);
            }
            catch (Exception)
            {
                return ServiceResult<int, enSpecializationResult>.Failure(enSpecializationResult.OperationFailed);
            }
        }

        public ServiceResult<SpecializationDto, enSpecializationResult> GetById(int specId)
        {
           try
            {
                var spec = _repository.GetById(specId);
                if (spec == null)
                    return ServiceResult<SpecializationDto, enSpecializationResult>.Failure(enSpecializationResult.NotFound);

                var dto = MapToDto(spec);
                dto.NumberOfDoctors = _repository.GetDoctorsCountInSpecialization(specId);

                return ServiceResult<SpecializationDto, enSpecializationResult>.Success(dto, enSpecializationResult.Success);
            }
            catch(Exception)
            {
                return ServiceResult<SpecializationDto, enSpecializationResult>.Failure(enSpecializationResult.DatabaseError);
            }
        }
        public ServiceResult<List<SpecializationDto>, enSpecializationResult> GetAllSpecializations()
        {
            try
            {
                var list = _repository.GetAllSpecializations();

                if (list == null)
                    return ServiceResult<List<SpecializationDto>, enSpecializationResult>.Success(new List<SpecializationDto>(), enSpecializationResult.Success);

                var dtoList = list.Select(s => MapToDto(s)).ToList();
                return ServiceResult<List<SpecializationDto>, enSpecializationResult>.Success(dtoList, enSpecializationResult.Success);
            }
            catch (Exception)
            {
                return ServiceResult<List<SpecializationDto>, enSpecializationResult>.Failure(enSpecializationResult.OperationFailed);
            }
        }

        public ServiceResult<List<SpecializationDto>, enSpecializationResult> GetAllSpecializationsWithStats()
        {
            try
            {
                var list = _repository.GetAllSpecializations();
                var dtoList = new List<SpecializationDto>();

                foreach (var spec in list)
                {
                    var dto = MapToDto(spec);
                    dto.NumberOfDoctors = _repository.GetDoctorsCountInSpecialization(spec.SpecializationId);
                    dtoList.Add(dto);
                }

                return ServiceResult<List<SpecializationDto>, enSpecializationResult>.Success(dtoList, enSpecializationResult.Success);
            }
            catch (Exception) { return ServiceResult<List<SpecializationDto>, enSpecializationResult>.Failure(enSpecializationResult.OperationFailed); }
        }

        public ServiceResult<List<SpecializationDto>, enSpecializationResult> Search(string keyword)
        {
            try
            {
                var list = _repository.SearchSpecializations(keyword);

                if (list == null)
                    return ServiceResult<List<SpecializationDto>, enSpecializationResult>.Success(new List<SpecializationDto>(), enSpecializationResult.Success);

                var dtoList = list.Select(s => MapToDto(s)).ToList();
                return ServiceResult<List<SpecializationDto>, enSpecializationResult>.Success(dtoList, enSpecializationResult.Success);
            }
            catch (Exception)
            {
                return ServiceResult<List<SpecializationDto>, enSpecializationResult>.Failure(enSpecializationResult.OperationFailed);
            }
        }

        public int SpecializationExists(string name, int excludeId = 0)
        {
            try
            {
                return _repository.SpecializationExists(name, excludeId);
            }
            catch (Exception)
            {
                return 0;
            }
        }
        public int GetDoctorsCount(int specId)
        {
            try
            {
                return _repository.GetDoctorsCountInSpecialization(specId);
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public bool CanDelete(int specId)
        {
            try
            {
                return _repository.CanDeleteSpecialization(specId);
            }
            catch (Exception)
            {
                return false;
            }
        }

        private SpecializationDto MapToDto(Specialization spec)
        {
            return new SpecializationDto
            {
                SpecializationId = spec.SpecializationId,
                SpecializationName = spec.SpecializationName,
                SpecializationDescription = spec.SpecializationDescription
            };
        }
    }
}