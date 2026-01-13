using Clinic.Entities.Enums;
using System;

namespace Clinic.Entity
{
    public class AppointmentView
    {
        public int AppointmentID { get; set; }
        public string PatientName { get; set; }
        public string DoctorName { get; set; }
        public string SpecializationName { get; set; }
        public DateTime AppointmentDate { get; set; }
        public string AppointmentStatus { get; set; } 
        public string ReasonForVisit { get; set; }
        public decimal ConsultationFee { get; set; }

        public string AppointmentTime => AppointmentDate.ToShortTimeString();
        public string AppointmentDay => AppointmentDate.ToShortDateString();
    }
}