using System;

namespace Clinic.Contracts
{
    public class MedicalRecordDto
    {
        public int RecordId { get; set; }
        public int AppointmentId { get; set; }
        public string Diagnosis { get; set; }
        public string Prescription { get; set; }
        public string Notes { get; set; }
        public DateTime CreatedAt { get; set; }
        public string PatientName { get; set; }
        public string DoctorName { get; set; }
        public DateTime AppointmentDate { get; set; }

        public string RecordSummary => $"{AppointmentDate:yyyy-MM-dd} - {Diagnosis}";
    }
}