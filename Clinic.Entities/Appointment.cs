using System;
using Clinic.Entities.Enums;
namespace Clinic.Entities
{
    public class Appointment
    {
        public int AppointmentId { get; set; }
        public int PatientId { get; set; }
        public int DoctorId { get; set; }
        public DateTime AppointmentDate { get; set; }
        public DateTime AppointmentEndDate { get; set; }
        public int DurationMinutes { get; set; }
        public  enStatus Status { get; set; }
        public string ReasonForVisit { get; set; }
        public DateTime LastUpdatedAt { get; set; }
        public DateTime CreatedAt { get; set; }

        public int CreatedBy { get; set; }
        public int? LastUpdatedBy { get; set; }

        public string PatientName { get; set; }
        public string DoctorName { get; set; }
    }
}
