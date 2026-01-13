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
    public class clsInvoiceItemService
    {
        private readonly clsInvoiceItemRepositroy _itemRepository;
        private readonly clsInvoiceRepositroy _invoiceRepository;

        public clsInvoiceItemService()
        {
            _itemRepository = new clsInvoiceItemRepositroy();
            _invoiceRepository = new clsInvoiceRepositroy();
        }

        public ServiceResult<int, enInvoiceItemResult> AddItem(InvoiceItemDto itemDto)
        {
            var validator = clsFinanceValidator.ValidateInvoiceItem(itemDto);
            if (validator.Count > 0)
                return ServiceResult<int, enInvoiceItemResult>.Failure(
                    enInvoiceItemResult.ValidationError,
                    validationErrors: validator.Cast<Enum>().ToList()
                );

            try
            {
                var parentInvoice = _invoiceRepository.GetById(itemDto.InvoiceId);
                if (parentInvoice == null)
                    return ServiceResult<int, enInvoiceItemResult>.Failure(enInvoiceItemResult.ParentInvoiceNotFound);

                if (parentInvoice.InvoiceStatus == enInvoiceStatus.Paid)
                    return ServiceResult<int, enInvoiceItemResult>.Failure(enInvoiceItemResult.InvoiceAlreadyClosed);

                if (parentInvoice.InvoiceStatus == enInvoiceStatus.Cancelled)
                    return ServiceResult<int, enInvoiceItemResult>.Failure(enInvoiceItemResult.InvoiceCancelled);

                InvoiceItem item = new InvoiceItem
                {
                    InvoiceId = itemDto.InvoiceId,
                    ItemDescription = itemDto.ItemDescription,
                    UnitPrice = itemDto.UnitPrice,
                    Quantity = itemDto.Quantity
                };

                int newItemId = _itemRepository.AddItem(item);

                if (newItemId > 0)
                {
                    _SyncInvoiceTotal(itemDto.InvoiceId);
                    return ServiceResult<int, enInvoiceItemResult>.Success(newItemId, enInvoiceItemResult.AddedSuccessfully);
                }

                return ServiceResult<int, enInvoiceItemResult>.Failure(enInvoiceItemResult.DatabaseError);
            }
            catch (Exception)
            {
                return ServiceResult<int, enInvoiceItemResult>.Failure(enInvoiceItemResult.UnexpectedError);
            }
        }

        public ServiceResult<int, enInvoiceItemResult> UpdateItem(InvoiceItemDto itemDto)
        {
            var validator = clsFinanceValidator.ValidateInvoiceItem(itemDto);
            if (validator.Count > 0)
                return ServiceResult<int, enInvoiceItemResult>.Failure(enInvoiceItemResult.ValidationError, validationErrors: validator.Cast<Enum>().ToList());

            try
            {
                var existingItem = _itemRepository.GetItemById(itemDto.ItemId);
                if (existingItem == null) return ServiceResult<int, enInvoiceItemResult>.Failure(enInvoiceItemResult.NotFound);

                var parentInvoice = _invoiceRepository.GetById(existingItem.InvoiceId);
                if (parentInvoice.InvoiceStatus >= enInvoiceStatus.Paid)
                    return ServiceResult<int, enInvoiceItemResult>.Failure(enInvoiceItemResult.InvoiceAlreadyClosed);

                existingItem.ItemDescription = itemDto.ItemDescription;
                existingItem.UnitPrice = itemDto.UnitPrice;
                existingItem.Quantity = itemDto.Quantity;

                int rows = _itemRepository.UpdateItem(existingItem);
                if (rows > 0)
                {
                    _SyncInvoiceTotal(existingItem.InvoiceId);
                    return ServiceResult<int, enInvoiceItemResult>.Success(rows, enInvoiceItemResult.UpdatedSuccessfully);
                }

                return ServiceResult<int, enInvoiceItemResult>.Failure(enInvoiceItemResult.DatabaseError);
            }
            catch (Exception)
            {
                return ServiceResult<int, enInvoiceItemResult>.Failure(enInvoiceItemResult.UnexpectedError);
            }
        }
        
        public ServiceResult<bool, enInvoiceItemResult> DeleteItem(int itemId)
        {
            var item = _itemRepository.GetItemById(itemId);

            if (item == null)
                return ServiceResult<bool, enInvoiceItemResult>.Failure(enInvoiceItemResult.UnexpectedError);

            var invoice = _invoiceRepository.GetById(item.InvoiceId);
            if (invoice == null)
                return ServiceResult<bool, enInvoiceItemResult>.Failure(enInvoiceItemResult.UnexpectedError);

            if ((int)invoice.InvoiceStatus >= 3)
            {
                return ServiceResult<bool, enInvoiceItemResult>.Failure(enInvoiceItemResult.InvoiceAlreadyClosed);
            }

            if (_itemRepository.DeleteItem(itemId) > 0)
            {
                _SyncInvoiceTotal(item.InvoiceId);
                return ServiceResult<bool, enInvoiceItemResult>.Success(true, enInvoiceItemResult.DeletedSuccessfully);
            }

            return ServiceResult<bool, enInvoiceItemResult>.Failure(enInvoiceItemResult.UnexpectedError);
        }

        public ServiceResult<List<InvoiceItemDto>, enInvoiceItemResult> GetItemsByInvoiceId(int invoiceId)
        {
            try
            {
                var items = _itemRepository.GetItemsByInvoiceId(invoiceId);
                var dtoList = items.Select(MapToDto).ToList();
                return ServiceResult<List<InvoiceItemDto>, enInvoiceItemResult>.Success(dtoList, enInvoiceItemResult.Success);
            }
            catch (Exception)
            {
                return ServiceResult<List<InvoiceItemDto>, enInvoiceItemResult>.Failure(enInvoiceItemResult.UnexpectedError);
            }
        }

        private void _SyncInvoiceTotal(int invoiceId)
        {
            decimal newSubTotal = _itemRepository.CalculateInvoiceSubTotal(invoiceId);
            var invoice = _invoiceRepository.GetById(invoiceId);

            if (invoice != null)
            {
                invoice.TotalAmount = newSubTotal;
                _invoiceRepository.UpdateInvoice(invoice);
            }
        }

        private InvoiceItemDto MapToDto(InvoiceItem item)
        {
            return new InvoiceItemDto
            {
                ItemId = item.ItemId,
                InvoiceId = item.InvoiceId,
                ItemDescription = item.ItemDescription,
                UnitPrice = item.UnitPrice,
                Quantity = item.Quantity
            };
        }
    }
}