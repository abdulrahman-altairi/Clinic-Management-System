using Clinic.Contracts.Enums;
using System;

namespace Clinic.Contracts.Dtos
{
    public class AppointmentCreateDto
    {
        public int PatientId { get; set; }
        public int DoctorId { get; set; }
        public DateTime AppointmentDate { get; set; }
        public int DurationMinutes { get; set; }
        public string ReasonForVisit { get; set; }
        public int CreatedBy { get; set; } 
        public enStatus Status { get; set; } = enStatus.Pending;
    }
}