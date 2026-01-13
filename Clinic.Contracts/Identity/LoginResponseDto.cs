using Clinic.Contracts.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clinic.Contracts.Identity
{
    public class LoginResponseDto
    {
        public int UserID { get; set; }
        public string Username { get; set; }
        public string FullName { get; set; }
        public enRole Role { get; set; }
    }
}
