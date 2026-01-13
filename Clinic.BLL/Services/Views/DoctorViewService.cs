using Clinic.BLL.Common.Result;
using Clinic.BLL.Enums;
using Clinic.Entity;
using SmartClinic.Contracts;
using SmartClinic.DAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Clinic.BLL.Services
{
    public class clsDoctorViewService
    {
        private readonly clsDoctorView _doctorViewDal = new clsDoctorView();

        private DoctorViewDto MapToDto(DoctorView entity)
        {
            if (entity == null) return null;
            return new DoctorViewDto
            {
                DoctorID = entity.DoctorID,
                FullName = $"{entity.FirstName} {entity.LastName}", 
                SpecializationName = entity.SpecializationName,
                Email = entity.Email,
                ContactNumber = entity.ContactNumber,
                ConsultationFee = entity.ConsultationFee,
                IsAvailable = entity.IsAvailable,
                Bio = entity.Bio
            };
        }

        public ServiceResult<List<DoctorViewDto>, enDoctorViewResult> GetAllDoctors()
        {
            try
            {
                var entities = _doctorViewDal.GetAllDoctorsInfo();
                var dtos = entities.Select(e => MapToDto(e)).ToList();

                return dtos.Any()
                    ? ServiceResult<List<DoctorViewDto>, enDoctorViewResult>.Success(dtos, enDoctorViewResult.Success)
                    : ServiceResult<List<DoctorViewDto>, enDoctorViewResult>.Failure(enDoctorViewResult.NoDoctorsFound);
            }
            catch (Exception)
            {
                return ServiceResult<List<DoctorViewDto>, enDoctorViewResult>.Failure(enDoctorViewResult.DatabaseError);
            }
        }

        public ServiceResult<DoctorViewDto, enDoctorViewResult> GetDoctorById(int id)
        {
            try
            {
                var entity = _doctorViewDal.GetDoctorFullDetails(id);
                if (entity == null)
                    return ServiceResult<DoctorViewDto, enDoctorViewResult>.Failure(enDoctorViewResult.DoctorNotFound);

                return ServiceResult<DoctorViewDto, enDoctorViewResult>.Success(MapToDto(entity), enDoctorViewResult.Success);
            }
            catch (Exception)
            {
                return ServiceResult<DoctorViewDto, enDoctorViewResult>.Failure(enDoctorViewResult.DatabaseError);
            }
        }

        public ServiceResult<List<DoctorViewDto>, enDoctorViewResult> SearchDoctors(string keyword)
        {
            try
            {
                var entities = string.IsNullOrWhiteSpace(keyword)
                    ? _doctorViewDal.GetAllDoctorsInfo()
                    : _doctorViewDal.SearchDoctorsGlobal(keyword);

                var dtos = entities.Select(e => MapToDto(e)).ToList();
                return ServiceResult<List<DoctorViewDto>, enDoctorViewResult>.Success(dtos, enDoctorViewResult.Success);
            }
            catch (Exception)
            {
                return ServiceResult<List<DoctorViewDto>, enDoctorViewResult>.Failure(enDoctorViewResult.DatabaseError);
            }
        }

        public ServiceResult<List<DoctorViewDto>, enDoctorViewResult> GetDoctorsBySpecialization(int specializationId)
        {
            try
            {
                var entities = _doctorViewDal.GetAvailableDoctorsBySpecialization(specializationId);
                var dtos = entities.Select(e => MapToDto(e)).ToList();
                return ServiceResult<List<DoctorViewDto>, enDoctorViewResult>.Success(dtos, enDoctorViewResult.Success);
            }
            catch (Exception)
            {
                return ServiceResult<List<DoctorViewDto>, enDoctorViewResult>.Failure(enDoctorViewResult.DatabaseError);
            }
        }

        public ServiceResult<List<DoctorViewDto>, enDoctorViewResult> GetDoctorsByFeeRange(decimal min, decimal max)
        {
            try
            {
                if (min > max)
                    return ServiceResult<List<DoctorViewDto>, enDoctorViewResult>.Failure(enDoctorViewResult.InvalidFeeRange);

                var entities = _doctorViewDal.FilterDoctorsByFeeRange(min, max);
                var dtos = entities.Select(e => MapToDto(e)).ToList();
                return ServiceResult<List<DoctorViewDto>, enDoctorViewResult>.Success(dtos, enDoctorViewResult.Success);
            }
            catch (Exception)
            {
                return ServiceResult<List<DoctorViewDto>, enDoctorViewResult>.Failure(enDoctorViewResult.DatabaseError);
            }
        }

        public ServiceResult<DataTable, enDoctorViewResult> GetDoctorDistributionStats()
        {
            try
            {
                var dt = _doctorViewDal.GetDoctorCountPerSpecialization();
                return ServiceResult<DataTable, enDoctorViewResult>.Success(dt, enDoctorViewResult.Success);
            }
            catch (Exception)
            {
                return ServiceResult<DataTable, enDoctorViewResult>.Failure(enDoctorViewResult.DatabaseError);
            }
        }
    }
}