using System;
using System.Collections.Generic;

namespace Clinic.BLL.Common.Result
{
    public class ServiceResult<T, TEnum> where TEnum : System.Enum
    {
        public T Data { get; set; }
        public TEnum Result { get; set; }
        public bool IsSuccess { get; set; }
        public string Message { get; set; }

        public List<Enum> ValidationErrors { get; set; } = new List<Enum>();

        public ServiceResult(T data, TEnum result, bool isSuccess, string message = "")
        {
            Data = data;
            Result = result;
            IsSuccess = isSuccess;
            Message = message;
        }

        public static ServiceResult<T, TEnum> Success(T data, TEnum result, string message = "")
            => new ServiceResult<T, TEnum>(data, result, true, message);

        public static ServiceResult<T, TEnum> Failure(TEnum result, string message = "", List<Enum> validationErrors = null)
        {
            var res = new ServiceResult<T, TEnum>(default, result, false, message);
            if (validationErrors != null)
                res.ValidationErrors = validationErrors;
            return res;
        }
    }
}