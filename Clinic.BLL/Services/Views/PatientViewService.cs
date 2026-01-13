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
    public class clsPatientViewService
    {
        private readonly clsPatientView _patientViewDal = new clsPatientView();

        private PatientViewDto MapToDto(PatientView entity)
        {
            if (entity == null) return null;

            return new PatientViewDto
            {
                PatientID = entity.PatientID,
                FirstName = entity.FirstName,
                LastName = entity.LastName,
                FullName = $"{entity.FirstName} {entity.LastName}",
                Email = entity.Email,
                ContactNumber = entity.ContactNumber,
                DateOfBirth = entity.DateOfBirth,
                Age = DateTime.Now.Year - entity.DateOfBirth.Year - (DateTime.Now.DayOfYear < entity.DateOfBirth.DayOfYear ? 1 : 0),
                InsuranceProvider = string.IsNullOrEmpty(entity.InsuranceProvider) ? "Self-Paid" : entity.InsuranceProvider,
                EmergencyContactName = entity.EmergencyContactName ?? "N/A"
            };
        }

        public ServiceResult<List<PatientViewDto>, enPatientViewResult> GetAllPatients()
        {
            try
            {
                var entities = _patientViewDal.GetAllPatients();
                var dtos = entities.Select(e => MapToDto(e)).ToList();

                return dtos.Any()
                    ? ServiceResult<List<PatientViewDto>, enPatientViewResult>.Success(dtos, enPatientViewResult.Success)
                    : ServiceResult<List<PatientViewDto>, enPatientViewResult>.Failure(enPatientViewResult.NoPatientsFound);
            }
            catch (Exception)
            {
                return ServiceResult<List<PatientViewDto>, enPatientViewResult>.Failure(enPatientViewResult.DatabaseError);
            }
        }

        public ServiceResult<PatientViewDto, enPatientViewResult> GetPatientById(int id)
        {
            try
            {
                var entity = _patientViewDal.GetPatientDetails(id);
                if (entity == null)
                    return ServiceResult<PatientViewDto, enPatientViewResult>.Failure(enPatientViewResult.PatientNotFound);

                return ServiceResult<PatientViewDto, enPatientViewResult>.Success(MapToDto(entity), enPatientViewResult.Success);
            }
            catch (Exception)
            {
                return ServiceResult<PatientViewDto, enPatientViewResult>.Failure(enPatientViewResult.DatabaseError);
            }
        }

        public ServiceResult<List<PatientViewDto>, enPatientViewResult> SearchPatients(string keyword)
        {
            try
            {
                var entities = string.IsNullOrWhiteSpace(keyword)
                    ? _patientViewDal.GetAllPatients()
                    : _patientViewDal.SearchPatients(keyword);

                var dtos = entities.Select(e => MapToDto(e)).ToList();
                return ServiceResult<List<PatientViewDto>, enPatientViewResult>.Success(dtos, enPatientViewResult.Success);
            }
            catch (Exception)
            {
                return ServiceResult<List<PatientViewDto>, enPatientViewResult>.Failure(enPatientViewResult.DatabaseError);
            }
        }

        public ServiceResult<List<PatientViewDto>, enPatientViewResult> GetPatientsByInsurance(string provider)
        {
            try
            {
                var entities = _patientViewDal.GetPatientsByInsurance(provider);
                var dtos = entities.Select(e => MapToDto(e)).ToList();
                return ServiceResult<List<PatientViewDto>, enPatientViewResult>.Success(dtos, enPatientViewResult.Success);
            }
            catch (Exception)
            {
                return ServiceResult<List<PatientViewDto>, enPatientViewResult>.Failure(enPatientViewResult.DatabaseError);
            }
        }

        public ServiceResult<List<PatientViewDto>, enPatientViewResult> GetPatientsWithEmergencyContacts()
        {
            try
            {
                var entities = _patientViewDal.GetPatientsWithEmergencyContact();
                var dtos = entities.Select(e => MapToDto(e)).ToList();
                return ServiceResult<List<PatientViewDto>, enPatientViewResult>.Success(dtos, enPatientViewResult.Success);
            }
            catch (Exception)
            {
                return ServiceResult<List<PatientViewDto>, enPatientViewResult>.Failure(enPatientViewResult.DatabaseError);
            }
        }

        public ServiceResult<DataTable, enPatientViewResult> GetInsuranceStats()
        {
            try
            {
                var dt = _patientViewDal.GetPatientsCountByInsurance();
                return ServiceResult<DataTable, enPatientViewResult>.Success(dt, enPatientViewResult.Success);
            }
            catch (Exception)
            {
                return ServiceResult<DataTable, enPatientViewResult>.Failure(enPatientViewResult.DatabaseError);
            }
        }
    }
}