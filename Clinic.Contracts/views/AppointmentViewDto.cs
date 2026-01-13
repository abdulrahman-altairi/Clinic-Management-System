using System;

namespace Clinic.Contracts
{
    public class AppointmentViewDto
    {
        public int AppointmentID { get; set; }
        public string PatientName { get; set; }
        public string DoctorName { get; set; }
        public string SpecializationName { get; set; }
        public DateTime AppointmentDate { get; set; }
        public string Status { get; set; }
        public string Reason { get; set; }
        public decimal Fee { get; set; }
    }
}
