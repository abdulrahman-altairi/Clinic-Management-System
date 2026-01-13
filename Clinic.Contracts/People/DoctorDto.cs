using Clinic.Contracts.Enums;
using System;

namespace Clinic.Contracts
{
    public class DoctorDto
    {
        public int DoctorId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public enGender Gender { get; set; }
        public string Email { get; set; }
        public string ContactNumber { get; set; }
        public int SpecializationId { get; set; }
        public string SpecializationName { get; set; }
        public string Bio { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public decimal ConsultationFee { get; set; }
        public string Address { get; set; }
        public bool IsAvailable { get; set; }

        public string FullName => $"{FirstName} {LastName}";
    }
}