using Clinic.BLL.Common.Result;
using Clinic.BLL.Enums;
using Clinic.BLL.Results;
using Clinic.BLL.Validators;
using Clinic.Contracts;
using Clinic.DAL;
using Clinic.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Clinic.BLL.Services
{
    public class clsMedicalRecordService
    {
        private readonly clsMedicalRecordRepositroy _repository;

        public clsMedicalRecordService()
        {
            _repository = new clsMedicalRecordRepositroy();
        }

        public ServiceResult<int, enMedicalRecordResult> AddMedicalRecord(MedicalRecordDto recordDto)
        {
            var validator = clsMidecalValidator.ValidateRecord(recordDto);
            if (validator.Count > 0)
                return ServiceResult<int, enMedicalRecordResult>.Failure(
                    enMedicalRecordResult.ValidationError,
                    validationErrors: validator.Cast<Enum>().ToList()
                );

            try
            {
                if (_repository.RecordExistsForAppointment(recordDto.AppointmentId) > 0)
                {
                    return ServiceResult<int, enMedicalRecordResult>.Failure(enMedicalRecordResult.RecordAlreadyExistsForAppointment);
                }

                MedicalRecord record = new MedicalRecord
                {
                    AppointmentID = recordDto.AppointmentId,
                    Diagnosis = recordDto.Diagnosis,
                    Prescription = recordDto.Prescription,
                    Notes = recordDto.Notes
                };

                int newId = _repository.AddRecord(record);
                if (newId > 0)
                    return ServiceResult<int, enMedicalRecordResult>.Success(newId, enMedicalRecordResult.Success);

                return ServiceResult<int, enMedicalRecordResult>.Failure(enMedicalRecordResult.DatabaseError);
            }
            catch (Exception)
            {
                return ServiceResult<int, enMedicalRecordResult>.Failure(enMedicalRecordResult.OperationFailed);
            }
        }

        public ServiceResult<int, enMedicalRecordResult> UpdateMedicalRecord(MedicalRecordDto recordDto)
        {
            var validator = clsMidecalValidator.ValidateRecord(recordDto);
            if (validator.Count > 0)
                return ServiceResult<int, enMedicalRecordResult>.Failure(
                    enMedicalRecordResult.ValidationError,
                    validationErrors: validator.Cast<Enum>().ToList()
                );

            try
            {
                var existingRecord = _repository.GetById(recordDto.RecordId);
                if (existingRecord == null)
                    return ServiceResult<int, enMedicalRecordResult>.Failure(enMedicalRecordResult.NotFound);

                existingRecord.Diagnosis = recordDto.Diagnosis;
                existingRecord.Prescription = recordDto.Prescription;
                existingRecord.Notes = recordDto.Notes;

                int rowsAffected = _repository.UpdateRecord(existingRecord);
                return rowsAffected > 0
                    ? ServiceResult<int, enMedicalRecordResult>.Success(rowsAffected, enMedicalRecordResult.Success)
                    : ServiceResult<int, enMedicalRecordResult>.Failure(enMedicalRecordResult.DatabaseError);
            }
            catch (Exception)
            {
                return ServiceResult<int, enMedicalRecordResult>.Failure(enMedicalRecordResult.OperationFailed);
            }
        }

        public ServiceResult<int, enMedicalRecordResult> DeleteMedicalRecord(int recordId)
        {
            try
            {
                int result = _repository.DeleteRecord(recordId);

                if (result > 0)
                    return ServiceResult<int, enMedicalRecordResult>.Success(result, enMedicalRecordResult.Success);

                return ServiceResult<int, enMedicalRecordResult>.Failure(enMedicalRecordResult.NotFound);
            }
            catch (Exception)
            {
                return ServiceResult<int, enMedicalRecordResult>.Failure(enMedicalRecordResult.OperationFailed);
            }
        }

        public ServiceResult<MedicalRecordDto, enMedicalRecordResult> GetById(int recordId)
        {
            try
            {
                var record = _repository.GetById(recordId);
                if (record == null)
                    return ServiceResult<MedicalRecordDto, enMedicalRecordResult>.Failure(enMedicalRecordResult.NotFound);

                return ServiceResult<MedicalRecordDto, enMedicalRecordResult>.Success(MapToDto(record), enMedicalRecordResult.Success);
            }
            catch (Exception)
            {
                return ServiceResult<MedicalRecordDto, enMedicalRecordResult>.Failure(enMedicalRecordResult.OperationFailed);
            }
        }

        public ServiceResult<MedicalRecordDto, enMedicalRecordResult> GetByAppointmentId(int appointmentId)
        {
            try
            {
                var record = _repository.GetByAppointmentId(appointmentId);
                if (record == null)
                    return ServiceResult<MedicalRecordDto, enMedicalRecordResult>.Failure(enMedicalRecordResult.NotFound);

                return ServiceResult<MedicalRecordDto, enMedicalRecordResult>.Success(MapToDto(record), enMedicalRecordResult.Success);
            }
            catch (Exception)
            {
                return ServiceResult<MedicalRecordDto, enMedicalRecordResult>.Failure(enMedicalRecordResult.OperationFailed);
            }
        }

        public ServiceResult<List<MedicalRecordDto>, enMedicalRecordResult> GetPatientHistory(int patientId)
        {
            try
            {
                var list = _repository.GetPatientHistory(patientId);
                var dtoList = list.Select(r => MapToDto(r)).ToList();

                if (dtoList.Count == 0)
                    return ServiceResult<List<MedicalRecordDto>, enMedicalRecordResult>.Failure(enMedicalRecordResult.PatientHasNoHistory);

                return ServiceResult<List<MedicalRecordDto>, enMedicalRecordResult>.Success(dtoList, enMedicalRecordResult.Success);
            }
            catch (Exception)
            {
                return ServiceResult<List<MedicalRecordDto>, enMedicalRecordResult>.Failure(enMedicalRecordResult.OperationFailed);
            }
        }

        public ServiceResult<List<MedicalRecordDto>, enMedicalRecordResult> GetAllRecordsPaged(int pageNumber, int pageSize)
        {
            try
            {
                var list = _repository.GetAllRecordsPaged(pageNumber, pageSize);
                var dtoList = list.Select(r => MapToDto(r)).ToList();
                return ServiceResult<List<MedicalRecordDto>, enMedicalRecordResult>.Success(dtoList, enMedicalRecordResult.Success);
            }
            catch (Exception)
            {
                return ServiceResult<List<MedicalRecordDto>, enMedicalRecordResult>.Failure(enMedicalRecordResult.OperationFailed);
            }
        }

        public ServiceResult<List<MedicalRecordDto>, enMedicalRecordResult> GetRecordsByDateRange(DateTime from, DateTime to)
        {
            if (from > to)
                return ServiceResult<List<MedicalRecordDto>, enMedicalRecordResult>.Failure(enMedicalRecordResult.InvalidDateRange);

            try
            {
                var list = _repository.GetRecordsByDateRange(from, to);
                var dtoList = list.Select(r => MapToDto(r)).ToList();
                return ServiceResult<List<MedicalRecordDto>, enMedicalRecordResult>.Success(dtoList, enMedicalRecordResult.Success);
            }
            catch (Exception)
            {
                return ServiceResult<List<MedicalRecordDto>, enMedicalRecordResult>.Failure(enMedicalRecordResult.OperationFailed);
            }
        }

        public ServiceResult<List<MedicalRecordDto>, enMedicalRecordResult> Search(string keyword)
        {
            try
            {
                var list = _repository.SearchByDiagnosis(keyword);
                var dtoList = list.Select(r => MapToDto(r)).ToList();
                return ServiceResult<List<MedicalRecordDto>, enMedicalRecordResult>.Success(dtoList, enMedicalRecordResult.Success);
            }
            catch (Exception)
            {
                return ServiceResult<List<MedicalRecordDto>, enMedicalRecordResult>.Failure(enMedicalRecordResult.OperationFailed);
            }
        }

        public int GetTotalRecordsCount()
        {
            try
            {
                return _repository.GetTotalRecordsCount();
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public int RecordExistsForAppointment(int appointmentId)
        {
            try
            {
                return _repository.RecordExistsForAppointment(appointmentId);
            }
            catch (Exception)
            {
                return 0;
            }
        }

        private MedicalRecordDto MapToDto(MedicalRecord record)
        {
            return new MedicalRecordDto
            {
                RecordId = record.RecordId,
                AppointmentId = record.AppointmentID,
                Diagnosis = record.Diagnosis,
                Prescription = record.Prescription,
                Notes = record.Notes,
                CreatedAt = record.CreatedAt,
                PatientName = record.PatientName,
                DoctorName = record.DoctorName,
                AppointmentDate = record.AppointmentDate
            };
        }
    }
}