using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clinic.Contracts.Identity
{
    public class LoginRequsetDto
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
