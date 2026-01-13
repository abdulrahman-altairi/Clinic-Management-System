using Clinic.BLL.Common.Result;
using Clinic.BLL.Enums;
using Clinic.Contracts;
using Clinic.Entity; 
using SmartClinic.DAL; 
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Clinic.BLL.Services
{
    public class clsAppointmentViewService
    {
        private readonly clsAppointmentView _appointmentViewDal = new clsAppointmentView();

        private AppointmentViewDto MapToDto(AppointmentView entity)
        {
            if (entity == null) return null;
            return new AppointmentViewDto
            {
                AppointmentID = entity.AppointmentID,
                PatientName = entity.PatientName,
                DoctorName = entity.DoctorName,
                SpecializationName = entity.SpecializationName,
                AppointmentDate = entity.AppointmentDate,
                Status = entity.AppointmentStatus, 
                Reason = entity.ReasonForVisit,
                Fee = entity.ConsultationFee
            };
        }

        public ServiceResult<List<AppointmentViewDto>, enAppointmentViewResult> GetAllAppointments()
        {
            try
            {
                var entities = _appointmentViewDal.GetAllAppointments();
                var dtos = entities.Select(e => MapToDto(e)).ToList();

                return dtos.Any()
                    ? ServiceResult<List<AppointmentViewDto>, enAppointmentViewResult>.Success(dtos, enAppointmentViewResult.Success)
                    : ServiceResult<List<AppointmentViewDto>, enAppointmentViewResult>.Failure(enAppointmentViewResult.NoAppointmentsFound);
            }
            catch (Exception)
            {
                return ServiceResult<List<AppointmentViewDto>, enAppointmentViewResult>.Failure(enAppointmentViewResult.DatabaseError);
            }
        }

        public ServiceResult<AppointmentViewDto, enAppointmentViewResult> GetAppointmentById(int id)
        {
            try
            {
                var entity = _appointmentViewDal.GetAppointmentById(id);
                if (entity == null)
                    return ServiceResult<AppointmentViewDto, enAppointmentViewResult>.Failure(enAppointmentViewResult.AppointmentNotFound);

                return ServiceResult<AppointmentViewDto, enAppointmentViewResult>.Success(MapToDto(entity), enAppointmentViewResult.Success);
            }
            catch (Exception)
            {
                return ServiceResult<AppointmentViewDto, enAppointmentViewResult>.Failure(enAppointmentViewResult.DatabaseError);
            }
        }

        public ServiceResult<List<AppointmentViewDto>, enAppointmentViewResult> SearchAppointments(string keyword)
        {
            try
            {
                var entities = string.IsNullOrWhiteSpace(keyword)
                    ? _appointmentViewDal.GetAllAppointments()
                    : _appointmentViewDal.SearchAppointments(keyword);

                var dtos = entities.Select(e => MapToDto(e)).ToList();
                return ServiceResult<List<AppointmentViewDto>, enAppointmentViewResult>.Success(dtos, enAppointmentViewResult.Success);
            }
            catch (Exception)
            {
                return ServiceResult<List<AppointmentViewDto>, enAppointmentViewResult>.Failure(enAppointmentViewResult.DatabaseError);
            }
        }

        public ServiceResult<decimal, enAppointmentViewResult> GetExpectedRevenue(DateTime start, DateTime end)
        {
            try
            {
                if (start > end)
                    return ServiceResult<decimal, enAppointmentViewResult>.Failure(enAppointmentViewResult.InvalidDateTime);

                var revenue = _appointmentViewDal.GetExpectedRevenue(start, end);
                return ServiceResult<decimal, enAppointmentViewResult>.Success(revenue, enAppointmentViewResult.Success);
            }
            catch (Exception)
            {
                return ServiceResult<decimal, enAppointmentViewResult>.Failure(enAppointmentViewResult.DatabaseError);
            }
        }

        public ServiceResult<DataTable, enAppointmentViewResult> GetSpecializationStats()
        {
            try
            {
                var dt = _appointmentViewDal.GetAppointmentCountBySpecialization();
                return ServiceResult<DataTable, enAppointmentViewResult>.Success(dt, enAppointmentViewResult.Success);
            }
            catch (Exception)
            {
                return ServiceResult<DataTable, enAppointmentViewResult>.Failure(enAppointmentViewResult.DatabaseError);
            }
        }

        public ServiceResult<List<AppointmentViewDto>, enAppointmentViewResult> GetPatientHistory(int patientId)
        {
            try
            {
                var entities = _appointmentViewDal.GetPatientAppointmentHistory(patientId);
                var dtos = entities.Select(e => MapToDto(e)).ToList();
                return ServiceResult<List<AppointmentViewDto>, enAppointmentViewResult>.Success(dtos, enAppointmentViewResult.Success);
            }
            catch (Exception)
            {
                return ServiceResult<List<AppointmentViewDto>, enAppointmentViewResult>.Failure(enAppointmentViewResult.DatabaseError);
            }
        }
    }
}