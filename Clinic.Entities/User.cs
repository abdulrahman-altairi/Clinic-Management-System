using System;
using Clinic.Entities.Enums;

namespace Clinic.Entities
{
    public class User : Person
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string PasswordHash { get; set; }
        public enRole Role {  get; set; }
        public bool IsActive { get; set; }
        public DateTime LastLogin {  get; set; }
    }
}
