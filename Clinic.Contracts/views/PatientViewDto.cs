using System;

namespace SmartClinic.Contracts
{
    public class PatientViewDto
    {
        public int PatientID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
        public string ContactNumber { get; set; }
        public string Email { get; set; }
        public DateTime DateOfBirth { get; set; }
        public int Age { get; set; } 
        public string InsuranceProvider { get; set; }
        public string EmergencyContactName { get; set; }
    }
}