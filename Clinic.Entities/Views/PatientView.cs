using System;

namespace Clinic.Entity
{
    public class PatientView
    {
        public int PatientID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName => $"{FirstName} {LastName}";
        public string Email { get; set; }
        public string ContactNumber { get; set; }
        public DateTime DateOfBirth { get; set; }

        public int Age => DateTime.Now.Year - DateOfBirth.Year - (DateTime.Now.DayOfYear < DateOfBirth.DayOfYear ? 1 : 0);

        public string InsuranceProvider { get; set; }
        public string EmergencyContactName { get; set; }
    }
}