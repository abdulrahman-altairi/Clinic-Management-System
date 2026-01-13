using Clinic.Contracts.Enums;
using System;

namespace Clinic.Contracts.Dtos
{
    public class AppointmentDto
    {
        public int AppointmentId { get; set; }
        public int PatientId { get; set; }
        public string PatientName { get; set; }
        public int DoctorId { get; set; }
        public string DoctorName { get; set; }
        public DateTime AppointmentDate { get; set; }
        public int DurationMinutes { get; set; }
        public enStatus Status { get; set; } 
        public string ReasonForVisit { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}