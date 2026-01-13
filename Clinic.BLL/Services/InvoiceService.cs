using Clinic.BLL.Common.Result;
using Clinic.BLL.Enums;
using Clinic.BLL.Validators;
using Clinic.Contracts;
using Clinic.DAL.Repositories;
using Clinic.Entities;
using Clinic.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Clinic.BLL.Services
{
    public class clsInvoiceService
    {
        private readonly clsInvoiceRepositroy _repository;

        public clsInvoiceService()
        {
            _repository = new clsInvoiceRepositroy();
        }

        public ServiceResult<int, enInvoiceResult> CreateInvoice(InvoiceDto invoiceDto)
        {
            var validator = clsFinanceValidator.ValidateInvoice(invoiceDto);
            if (validator.Count > 0)
                return ServiceResult<int, enInvoiceResult>.Failure(
                    enInvoiceResult.ValidationError,
                    validationErrors: validator.Cast<Enum>().ToList()
                );

            try
            {

                if (_repository.InvoiceExistsForAppointment(invoiceDto.AppointmentId) > 0)
                    return ServiceResult<int, enInvoiceResult>.Failure(enInvoiceResult.AppointmentAlreadyHasInvoice);

                Invoice invoice = new Invoice
                {
                    AppointmentId = invoiceDto.AppointmentId,
                    PatientId = invoiceDto.PatientId,
                    TotalAmount = invoiceDto.TotalAmount,
                    TaxAmount = invoiceDto.TaxAmount,
                    DiscountAmount = invoiceDto.DiscountAmount,
                    DueDate = invoiceDto.DueDate,
                    InvoiceStatus = (enInvoiceStatus)invoiceDto.InvoiceStatus
                };

                int newId = _repository.CreateInvoice(invoice);

                if (newId > 0)
                    return ServiceResult<int, enInvoiceResult>.Success(newId, enInvoiceResult.CreatedSuccessfully);

                return ServiceResult<int, enInvoiceResult>.Failure(enInvoiceResult.DatabaseError);
            }
            catch (Exception)
            {
                return ServiceResult<int, enInvoiceResult>.Failure(enInvoiceResult.UnexpectedError);
            }
        }

        public ServiceResult<int, enInvoiceResult> UpdateInvoice(InvoiceDto invoiceDto)
        {
            var validator = clsFinanceValidator.ValidateInvoice(invoiceDto);
            if (validator.Count > 0)
                return ServiceResult<int, enInvoiceResult>.Failure(
                    enInvoiceResult.ValidationError,
                    validationErrors: validator.Cast<Enum>().ToList()
                );

            try
            {
                var existingInvoice = _repository.GetById(invoiceDto.InvoiceId);
                if (existingInvoice == null)
                    return ServiceResult<int, enInvoiceResult>.Failure(enInvoiceResult.NotFound);

                existingInvoice.TotalAmount = invoiceDto.TotalAmount;
                existingInvoice.TaxAmount = invoiceDto.TaxAmount;
                existingInvoice.DiscountAmount = invoiceDto.DiscountAmount;

                int rowsAffected = _repository.UpdateInvoice(existingInvoice);

                if (rowsAffected == -1)
                    return ServiceResult<int, enInvoiceResult>.Failure(enInvoiceResult.InvoiceAlreadyPaid);

                return rowsAffected > 0
                    ? ServiceResult<int, enInvoiceResult>.Success(rowsAffected, enInvoiceResult.UpdatedSuccessfully)
                    : ServiceResult<int, enInvoiceResult>.Failure(enInvoiceResult.DatabaseError);
            }
            catch (Exception)
            {
                return ServiceResult<int, enInvoiceResult>.Failure(enInvoiceResult.UnexpectedError);
            }
        }

        public ServiceResult<int, enInvoiceResult> UpdateStatus(int invoiceId, Contracts.Enums.enInvoiceStatus newStatus)
        {
            try
            {
                var entityStatus = (enInvoiceStatus)newStatus;

                int result = _repository.UpdateStatus(invoiceId, entityStatus);

                if (result == -1)
                    return ServiceResult<int, enInvoiceResult>.Failure(enInvoiceResult.InvalidStatusTransition);

                return result > 0
                    ? ServiceResult<int, enInvoiceResult>.Success(result, enInvoiceResult.StatusChangedSuccessfully)
                    : ServiceResult<int, enInvoiceResult>.Failure(enInvoiceResult.DatabaseError);
            }
            catch (Exception)
            {
                return ServiceResult<int, enInvoiceResult>.Failure(enInvoiceResult.UnexpectedError);
            }
        }

        public ServiceResult<InvoiceDto, enInvoiceResult> GetById(int invoiceId)
        {
            try
            {
                var invoice = _repository.GetById(invoiceId);
                if (invoice == null) return ServiceResult<InvoiceDto, enInvoiceResult>.Failure(enInvoiceResult.NotFound);

                return ServiceResult<InvoiceDto, enInvoiceResult>.Success(MapToDto(invoice), enInvoiceResult.Success);
            }
            catch (Exception)
            {
                return ServiceResult<InvoiceDto, enInvoiceResult>.Failure(enInvoiceResult.UnexpectedError);
            }
        }

        public ServiceResult<List<InvoiceDto>, enInvoiceResult> GetPatientInvoices(int patientId)
        {
            try
            {
                var invoices = _repository.GetPatientInvoices(patientId);
                var dtoList = invoices.Select(MapToDto).ToList();
                return ServiceResult<List<InvoiceDto>, enInvoiceResult>.Success(dtoList, enInvoiceResult.Success);
            }
            catch (Exception)
            {
                return ServiceResult<List<InvoiceDto>, enInvoiceResult>.Failure(enInvoiceResult.UnexpectedError);
            }
        }

        public ServiceResult<List<InvoiceDto>, enInvoiceResult> GetInvoicesByDateRange(DateTime start, DateTime end)
        {
            if (start > end) return ServiceResult<List<InvoiceDto>, enInvoiceResult>.Failure(enInvoiceResult.ValidationError);

            try
            {
                var invoices = _repository.GetInvoicesByDateRange(start, end);
                return ServiceResult<List<InvoiceDto>, enInvoiceResult>.Success(invoices.Select(MapToDto).ToList(), enInvoiceResult.Success);
            }
            catch (Exception)
            {
                return ServiceResult<List<InvoiceDto>, enInvoiceResult>.Failure(enInvoiceResult.UnexpectedError);
            }
        }

        public ServiceResult<decimal, enInvoiceResult> GetPatientOutstandingBalance(int patientId)
        {
            try
            {
                decimal balance = _repository.GetTotalOutstandingBalance(patientId);
                return ServiceResult<decimal, enInvoiceResult>.Success(balance, enInvoiceResult.Success);
            }
            catch (Exception)
            {
                return ServiceResult<decimal, enInvoiceResult>.Failure(enInvoiceResult.UnexpectedError);
            }
        }

        public ServiceResult<decimal, enInvoiceResult> GetTotalRevenue(DateTime start, DateTime end)
        {
            try
            {
                decimal revenue = _repository.GetTotalRevenue(start, end);
                return ServiceResult<decimal, enInvoiceResult>.Success(revenue, enInvoiceResult.Success);
            }
            catch (Exception)
            {
                return ServiceResult<decimal, enInvoiceResult>.Failure(enInvoiceResult.UnexpectedError);
            }
        }

        public ServiceResult<int, enInvoiceResult> CheckInvoiceExists(int appointmentId)
        {
            int count = _repository.InvoiceExistsForAppointment(appointmentId);
            return ServiceResult<int, enInvoiceResult>.Success(count, enInvoiceResult.Success);
        }

        private InvoiceDto MapToDto(Invoice inv)
        {
            return new InvoiceDto
            {
                InvoiceId = inv.InvoiceId,
                InvoiceNumber = inv.InvoiceNumber,
                AppointmentId = inv.AppointmentId,
                PatientId = inv.PatientId,
                TotalAmount = inv.TotalAmount,
                TaxAmount = inv.TaxAmount,
                DiscountAmount = inv.DiscountAmount,
                NetAmount = inv.NetAmount,
                InvoiceDate = inv.InvoiceDate,
                DueDate = inv.DueDate,
                InvoiceStatus = (Contracts.Enums.enInvoiceStatus)inv.InvoiceStatus,
                PatientName = inv.PatientName
            };
        }
    }
}