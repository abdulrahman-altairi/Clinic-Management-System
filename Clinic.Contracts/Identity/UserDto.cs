using Clinic.Contracts.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clinic.Contracts
{
    public class UserDto : PersonDto
    {
        public int UserID { get; set; }
        public string Username { get; set; }
        public enRole Role { get; set; }
        public bool IsActive { get; set; }
        public DateTime? LastLogin { get; set; }
    }
}
