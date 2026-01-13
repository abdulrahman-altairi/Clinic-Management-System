using Clinic.BLL.Common.Result;
using Clinic.BLL.Enums;
using Clinic.Contracts.DTOs;
using Clinic.DAL.Repositories;
using Clinic.Entities;
using Clinic.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Data;

namespace Clinic.BLL.Services
{
    public class clsPaymentService
    {
        private readonly clsPaymentRepositroy _paymentRepository;
        private readonly clsInvoiceRepositroy _invoiceRepository;

        public clsPaymentService()
        {
            _paymentRepository = new clsPaymentRepositroy();
            _invoiceRepository = new clsInvoiceRepositroy();
        }
        public ServiceResult<int, enPaymentResult> ProcessPayment(PaymentDto paymentDto)
        {
            try
            {
                var invoice = _invoiceRepository.GetById(paymentDto.InvoiceId);
                if (invoice == null)
                    return ServiceResult<int, enPaymentResult>.Failure(enPaymentResult.InvoiceNotFound);

                if (invoice.InvoiceStatus == enInvoiceStatus.Cancelled)
                    return ServiceResult<int, enPaymentResult>.Failure(enPaymentResult.InvoiceCancelled);

                if (invoice.InvoiceStatus == enInvoiceStatus.Paid)
                    return ServiceResult<int, enPaymentResult>.Failure(enPaymentResult.InvoiceAlreadyPaid);

                decimal totalPaid = _paymentRepository.GetTotalPaidForInvoice(paymentDto.InvoiceId);
                decimal remainingBalance = invoice.TotalAmount - totalPaid;

                if (paymentDto.PaymentAmount <= 0)
                    return ServiceResult<int, enPaymentResult>.Failure(enPaymentResult.InvalidPaymentAmount);

                if (paymentDto.PaymentAmount > remainingBalance)
                    return ServiceResult<int, enPaymentResult>.Failure(enPaymentResult.AmountExceedsRemainingBalance);

                Payment paymentEntity = new Payment
                {
                    InvoiceId = paymentDto.InvoiceId,
                    PaymentAmount = paymentDto.PaymentAmount,
                    PaymentMethod = (enPaymentMethod)paymentDto.PaymentMethod,
                    TransactionRef = paymentDto.TransactionRef
                };

                int paymentId = _paymentRepository.AddPayment(paymentEntity);

                if (paymentId > 0)
                {
                    _UpdateInvoiceStatusAfterPayment(paymentDto.InvoiceId, invoice.TotalAmount);
                    return ServiceResult<int, enPaymentResult>.Success(paymentId, enPaymentResult.Success);
                }

                return ServiceResult<int, enPaymentResult>.Failure(enPaymentResult.CreationFailed);
            }
            catch (Exception)
            {
                return ServiceResult<int, enPaymentResult>.Failure(enPaymentResult.OperationFailed);
            }
        }


        private void _UpdateInvoiceStatusAfterPayment(int invoiceId, decimal totalInvoiceAmount)
        {
            try
            {
                decimal currentTotalPaid = _paymentRepository.GetTotalPaidForInvoice(invoiceId);
                enInvoiceStatus newStatus;

                if (currentTotalPaid >= totalInvoiceAmount)
                    newStatus = enInvoiceStatus.Paid;
                else if (currentTotalPaid > 0)
                    newStatus = enInvoiceStatus.PartiallyPaid;
                else
                    newStatus = enInvoiceStatus.Issued;

                _invoiceRepository.UpdateStatus(invoiceId, newStatus);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public ServiceResult<List<PaymentDto>, enPaymentResult> GetInvoicePayments(int invoiceId)
        {
            try
            {
                var payments = _paymentRepository.GetPaymentsByInvoiceId(invoiceId);
                List<PaymentDto> dtoList = new List<PaymentDto>();

                if (payments != null)
                {
                    foreach (var p in payments)
                    {
                        dtoList.Add(new PaymentDto
                        {
                            PaymentId = p.PaymentId,
                            InvoiceId = p.InvoiceId,
                            PaymentAmount = p.PaymentAmount,
                            PaymentDate = p.PaymentDate,
                            PaymentMethod = (Contracts.Enums.enPaymentMethod)p.PaymentMethod,
                            TransactionRef = p.TransactionRef
                        });
                    }
                }

                return ServiceResult<List<PaymentDto>, enPaymentResult>.Success(dtoList, enPaymentResult.Success);
            }
            catch (Exception)
            {
                return ServiceResult<List<PaymentDto>, enPaymentResult>.Failure(enPaymentResult.OperationFailed);
            }
        }

        public ServiceResult<DataTable, enPaymentResult> GetDailyIncomeReport(DateTime date)
        {
            try
            {
                DataTable report = _paymentRepository.GetDailyIncomeByMethod(date);

                if (report == null)
                    return ServiceResult<DataTable, enPaymentResult>.Success(new DataTable(), enPaymentResult.Success);

                return ServiceResult<DataTable, enPaymentResult>.Success(report, enPaymentResult.Success);
            }
            catch (Exception)
            {
                return ServiceResult<DataTable, enPaymentResult>.Failure(enPaymentResult.OperationFailed);
            }
        }

        public PaymentDto GetPaymentDetails(int paymentId)
        {
            var p = _paymentRepository.GetById(paymentId);
            if (p == null) return null;

            return new PaymentDto
            {
                PaymentId = p.PaymentId,
                InvoiceId = p.InvoiceId,
                PaymentAmount = p.PaymentAmount,
                PaymentDate = p.PaymentDate,
                PaymentMethod = (Contracts.Enums.enPaymentMethod)p.PaymentMethod,
                TransactionRef = p.TransactionRef
            };
        }
    }
}






/*
 
 
 
 
 using Clinic.BLL.Common.Result;
using Clinic.BLL.Enums;
using Clinic.Contracts.DTOs;
using Clinic.DAL.Repositories;
using Clinic.Entities;
using Clinic.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Data;

namespace Clinic.BLL.Services
{
    public class clsPaymentService
    {
        private readonly clsPaymentRepositroy _paymentRepository;
        private readonly clsInvoiceRepositroy _invoiceRepository;

        public clsPaymentService()
        {
            _paymentRepository = new clsPaymentRepositroy();
            _invoiceRepository = new clsInvoiceRepositroy();
        }

        public ServiceResult<int, enPaymentResult> ProcessPayment(PaymentDto paymentDto)
        {
            var invoice = _invoiceRepository.GetById(paymentDto.InvoiceId);
            if (invoice == null)
                return ServiceResult<int, enPaymentResult>.Failure(enPaymentResult.InvoiceNotFound);

            if (invoice.InvoiceStatus == enInvoiceStatus.Cancelled)
                return ServiceResult<int, enPaymentResult>.Failure(enPaymentResult.InvoiceCancelled);

            if (invoice.InvoiceStatus == enInvoiceStatus.Paid)
                return ServiceResult<int, enPaymentResult>.Failure(enPaymentResult.InvoiceAlreadyPaid);

            decimal totalPaid = _paymentRepository.GetTotalPaidForInvoice(paymentDto.InvoiceId);
            decimal remainingBalance = invoice.TotalAmount - totalPaid;

          
            if (paymentDto.PaymentAmount <= 0)
                return ServiceResult<int, enPaymentResult>.Failure(enPaymentResult.InvalidPaymentAmount);

            if (paymentDto.PaymentAmount > remainingBalance)
                return ServiceResult<int, enPaymentResult>.Failure(enPaymentResult.AmountExceedsRemainingBalance);

            Payment paymentEntity = new Payment
            {
                InvoiceId = paymentDto.InvoiceId,
                PaymentAmount = paymentDto.PaymentAmount,
                PaymentMethod = (enPaymentMethod)paymentDto.PaymentMethod,
                TransactionRef = paymentDto.TransactionRef
            };

            int paymentId = _paymentRepository.AddPayment(paymentEntity);

            if (paymentId > 0)
            {
                _UpdateInvoiceStatusAfterPayment(paymentDto.InvoiceId, invoice.TotalAmount);

                return ServiceResult<int, enPaymentResult>.Success(paymentId, enPaymentResult.Success);
            }

            return ServiceResult<int, enPaymentResult>.Failure(enPaymentResult.CreationFailed);
        }

        private void _UpdateInvoiceStatusAfterPayment(int invoiceId, decimal totalInvoiceAmount)
        {
            decimal currentTotalPaid = _paymentRepository.GetTotalPaidForInvoice(invoiceId);

            enInvoiceStatus newStatus;

            if (currentTotalPaid >= totalInvoiceAmount)
                newStatus = enInvoiceStatus.Paid;
            else if (currentTotalPaid > 0)
                newStatus = enInvoiceStatus.PartiallyPaid;
            else
                newStatus = enInvoiceStatus.Issued;

            _invoiceRepository.UpdateStatus(invoiceId, newStatus);
        }

        public List<PaymentDto> GetInvoicePayments(int invoiceId)
        {
            var payments = _paymentRepository.GetPaymentsByInvoiceId(invoiceId);
            List<PaymentDto> dtoList = new List<PaymentDto>();

            foreach (var p in payments)
            {
                dtoList.Add(new PaymentDto
                {
                    PaymentId = p.PaymentId,
                    InvoiceId = p.InvoiceId,
                    PaymentAmount = p.PaymentAmount,
                    PaymentDate = p.PaymentDate,
                    PaymentMethod = (Contracts.Enums.enPaymentMethod)p.PaymentMethod,
                    TransactionRef = p.TransactionRef
                });
            }
            return dtoList;
        }

        public DataTable GetDailyIncomeReport(DateTime date)
        {
            return _paymentRepository.GetDailyIncomeByMethod(date);
        }

        public PaymentDto GetPaymentDetails(int paymentId)
        {
            var p = _paymentRepository.GetById(paymentId);
            if (p == null) return null;

            return new PaymentDto
            {
                PaymentId = p.PaymentId,
                InvoiceId = p.InvoiceId,
                PaymentAmount = p.PaymentAmount,
                PaymentDate = p.PaymentDate,
                PaymentMethod = (Contracts.Enums.enPaymentMethod)p.PaymentMethod,
                TransactionRef = p.TransactionRef
            };
        }
    }
}
 
 
 */