using Clinic.BLL.Results;
using Clinic.Contracts.Enums;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Clinic.BLL.Helper
{
    public class clsValidationRole
    {
        private readonly string _value;
        private readonly List<enValidationResult> _errors;
        private readonly int _initialErrorCount;

        public clsValidationRole(string value, List<enValidationResult> errors)
        {
            _value = value;
            _errors = errors;
            _initialErrorCount = errors.Count;
        }

        private bool HasError => _errors.Count > _initialErrorCount;

        public clsValidationRole NotEmpty(enValidationResult error)
        {
            if (HasError) return this;

            if (string.IsNullOrWhiteSpace(_value))
                _errors.Add(error);

            return this;
        }

        public clsValidationRole TooLong(int max, enValidationResult error)
        {
            if (HasError) return this;

            if (_value != null && _value.Length > max)
                _errors.Add(error);

            return this;
        }

        public clsValidationRole TooSmall(int min, enValidationResult error)
        {
            if (HasError) return this;

            if (_value != null && _value.Length < min)
                _errors.Add(error);

            return this;
        }


        public clsValidationRole Length(int min, int max, enValidationResult error)
        {
            if (HasError)
                return this;
            if (_value != null && (_value.Length < min || _value.Length > max))
                _errors.Add(error);
            return this;
        }

        public clsValidationRole Matches(string pattern, enValidationResult error)
        {
            if (HasError) return this;

            if (!string.IsNullOrWhiteSpace(_value) && !Regex.IsMatch(_value, pattern))
                _errors.Add(error);

            return this;
        }


        public clsValidationRole Must(Predicate<string> condition, enValidationResult error)
        {
            if (HasError)
                return this;
            if (!condition(_value))
                _errors.Add(error);
            return this;
        }

        public clsValidationRole Between(int min, int max, enValidationResult error)
        {
            if (HasError)
                return this;
            if (Convert.ToInt32(_value) < min || Convert.ToInt32(_value) > max)
                _errors.Add(error);
            return this;
        }

        public clsValidationRole NotFuture(enValidationResult error)
        {
            if (HasError) return this;

            if (DateTime.TryParse(_value, out var date) && date > DateTime.Now)
                _errors.Add(error);

            return this;
        }

        public clsValidationRole IsValidGender(enValidationResult error)
        {
            if (HasError) return this;
            if (!int.TryParse(_value, out int result) ||
                !Enum.IsDefined(typeof(enGender), result) ||
                result == (int)enGender.Unknown)
            {
                _errors.Add(error);
            }

            return this;
        }


        public clsValidationRole IsMoney(enValidationResult error)
        {
            if (HasError) return this;

            if (!decimal.TryParse(_value, out _))
                _errors.Add(error);

            return this;
        }

        public clsValidationRole NotNegative(enValidationResult error)
        {
            if (HasError) return this;

            if (decimal.TryParse(_value, out decimal amount) && amount < 0)
                _errors.Add(error);

            return this;
        }

        public clsValidationRole GreaterThanZero(enValidationResult error)
        {
            if (HasError) return this;

            if (decimal.TryParse(_value, out decimal amount) && amount <= 0)
                _errors.Add(error);

            return this;
        }

        public clsValidationRole IsPercentage(enValidationResult error)
        {
            if (HasError) return this;

            if (decimal.TryParse(_value, out decimal percent) && (percent < 0 || percent > 100))
                _errors.Add(error);

            return this;
        }

        public clsValidationRole MaxValue(decimal max, enValidationResult error)
        {
            if (HasError) return this;

            if (decimal.TryParse(_value, out decimal amount) && amount > max)
                _errors.Add(error);

            return this;
        }

        public clsValidationRole IsInteger(enValidationResult error)
        {
            if (HasError) return this;
            if (!int.TryParse(_value, out _))
                _errors.Add(error);
            return this;
        }

        public clsValidationRole IsValidEnum<TEnum>(enValidationResult error) where TEnum : struct
        {
            if (HasError) return this;
            if (!int.TryParse(_value, out int result) || !Enum.IsDefined(typeof(TEnum), result))
            {
                _errors.Add(error);
            }
            return this;
        }

        public clsValidationRole IsTransactionRef(enValidationResult error)
        {
            if (HasError) return this;
            return Matches(@"^[a-zA-Z0-9\-_]+$", error);
        }

        public clsValidationRole IsFuture(enValidationResult error)
        {
            if (HasError) return this;
            if (DateTime.TryParse(_value, out var date) && date < DateTime.Now)
                _errors.Add(error);
            return this;
        }

        public clsValidationRole WithinWorkingHours(int startHour, int endHour, enValidationResult error)
        {
            if (HasError) return this;
            if (DateTime.TryParse(_value, out var date) && (date.Hour < startHour || date.Hour >= endHour))
                _errors.Add(error);
            return this;
        }

        public clsValidationRole IsAlphaNumeric(enValidationResult error)
        {
            if (HasError) return this;
            return Matches(@"^[\p{L}\p{N}\s]+$", error);
        }

        public clsValidationRole IsEmail(enValidationResult error)
        {
            if (HasError) return this;
            return Matches(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", error);
        }

        public clsValidationRole IsPhone(enValidationResult error)
        {
            if (HasError) return this;
            return Matches(@"^\d{10}$", error);
        }
    }
}