using Microsoft.SqlServer.Server;
using System;

namespace Clinic.Entities
{
    public class MedicalRecord
    {
        public int RecordId { get; set; }
        public int AppointmentID { get; set; }
        public string Diagnosis {  get; set; }
        public string Prescription { get; set; }
        public string Notes { get; set; }
        public DateTime CreatedAt { get; set; }


        public string PatientName { get; set; }
        public string DoctorName { get; set; }
        public DateTime AppointmentDate { get; set; }
    }
}
