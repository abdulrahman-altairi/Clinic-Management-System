using Clinic.BLL.Common.Result;
using Clinic.BLL.Enums;
using Clinic.BLL.Validators;
using Clinic.Contracts.Dtos;
using Clinic.DAL.Repositories;
using Clinic.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Clinic.BLL.Services
{
    public class clsAppointmentService
    {
        private readonly clsAppointmentRepository _appointmentRepository;

        public clsAppointmentService()
        {
            _appointmentRepository = new clsAppointmentRepository();
        }

        public ServiceResult<int, enAppointmentResult> BookAppointment(AppointmentCreateDto appDto)
        {

            var validator = clsMidecalValidator.ValidateAppointment(appDto);
            if (validator.Count > 0)
                return ServiceResult<int, enAppointmentResult>.Failure(
                    enAppointmentResult.ValidationError,
                    validationErrors: validator.Cast<Enum>().ToList()
                );

            try
            {
                
                int conflicts = _appointmentRepository.GetConflictCount(
                    appDto.DoctorId,
                    appDto.AppointmentDate,
                    appDto.DurationMinutes);

                if (conflicts > 0)
                    return ServiceResult<int, enAppointmentResult>.Failure(enAppointmentResult.DoctorBusy);

                Appointment appointment = new Appointment
                {
                    PatientId = appDto.PatientId,
                    DoctorId = appDto.DoctorId,
                    AppointmentDate = appDto.AppointmentDate,
                    DurationMinutes = appDto.DurationMinutes,
                    Status = (Clinic.Entities.Enums.enStatus)appDto.Status,
                    ReasonForVisit = appDto.ReasonForVisit,
                    CreatedBy = appDto.CreatedBy,
                    CreatedAt = DateTime.Now
                };

                int newId = _appointmentRepository.BookAppointment(appointment);

                if (newId > 0)
                    return ServiceResult<int, enAppointmentResult>.Success(newId, enAppointmentResult.Success);

                return ServiceResult<int, enAppointmentResult>.Failure(enAppointmentResult.DatabaseError);
            }
            catch (Exception)
            {
                return ServiceResult<int, enAppointmentResult>.Failure(enAppointmentResult.OperationFailed);
            }
        }

        public ServiceResult<bool, enAppointmentResult> UpdateStatus(int appointmentId, Clinic.Contracts.Enums.enStatus newStatus, int updatedBy)
        {
            try
            {

                Entities.Enums.enStatus entityStatus = (Clinic.Entities.Enums.enStatus)((int)newStatus);

                int rows = _appointmentRepository.UpdateStatus(appointmentId, entityStatus, updatedBy);

                if (rows > 0)
                    return ServiceResult<bool, enAppointmentResult>.Success(true, enAppointmentResult.Success);

                return ServiceResult<bool, enAppointmentResult>.Failure(enAppointmentResult.AppointmentNotFound);
            }
            catch (Exception)
            {
                return ServiceResult<bool, enAppointmentResult>.Failure(enAppointmentResult.OperationFailed);
            }
        }

        public ServiceResult<bool, enAppointmentResult> CancelAppointment(int appointmentId, int updatedBy)
        {
            return UpdateStatus(appointmentId, Contracts.Enums.enStatus.Canceled, updatedBy);
        }

        public ServiceResult<List<AppointmentDto>, enAppointmentResult> GetAllAppointments()
        {
            try
            {
                var dataTable = _appointmentRepository.GetAllAppointments();
                var list = new List<AppointmentDto>();

                foreach (System.Data.DataRow row in dataTable.Rows)
                {
                    list.Add(new AppointmentDto
                    {
                        AppointmentId = Convert.ToInt32(row["AppointmentID"]),
                        PatientId = Convert.ToInt32(row["PatientID"]),
                        DoctorId = Convert.ToInt32(row["DoctorID"]),
                        AppointmentDate = Convert.ToDateTime(row["AppointmentDate"]),
                        DurationMinutes = Convert.ToInt32(row["DurationMinutes"]),
                        Status = (Contracts.Enums.enStatus)Convert.ToInt32(row["Status"]),
                        ReasonForVisit = row["ReasonForVisit"]?.ToString(),
                        CreatedAt = Convert.ToDateTime(row["CreatedAt"])
                    });
                }

                return ServiceResult<List<AppointmentDto>, enAppointmentResult>.Success(list, enAppointmentResult.Success);
            }
            catch (Exception)
            {
                return ServiceResult<List<AppointmentDto>, enAppointmentResult>.Failure(enAppointmentResult.DatabaseError);
            }
        }

        public bool IsSlotAvailable(int doctorId, DateTime start, int duration)
        {
            return _appointmentRepository.GetConflictCount(doctorId, start, duration) == 0;
        }
    }
}