using System;

namespace Clinic.Entities
{
    public class Doctor : Person
    {
        public int DoctorId { get; set; }
        public int SpecializationId { get; set; }
        public string SpecializationName { get; set; }
        public string Bio {  get; set; }
        public decimal ConsultationFee { get; set; }
        public bool IsAvilable { get; set; }
        public DateTime JoinedAt { get; set; }
    }
}
