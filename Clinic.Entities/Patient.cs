using System;

namespace Clinic.Entities
{
    public class Patient : Person
    {
        public int PatintId { get; set; }
        public string InsuranceProvider { get; set; }
        public string InsurancePolicyNumber { get; set; }
        public string EmergencyContactName { get; set; }
        public string EmergencyContactPhone { get; set; }
    }
}
