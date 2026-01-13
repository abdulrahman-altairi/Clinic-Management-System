

namespace Clinic.BLL.Helper
{
    public static class clsRegexPatterns
    {
        public const string Email = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";

        public const string Phone = @"^\+?[1-9]\d{1,14}$";

        public const string Date = @"^\d{4}(([-./])(0[1-9]|1[012])(\2(0[1-9]|[12][0-9]|3[01]))?)?$";
    }
}
