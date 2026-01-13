namespace Clinic.Entity
{
    public class DoctorView
    {
        public int DoctorID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName => $"{FirstName} {LastName}";
        public string Email { get; set; }
        public string ContactNumber { get; set; }
        public int SpecializationID { get; set; }
        public string SpecializationName { get; set; }
        public string Bio { get; set; }
        public decimal ConsultationFee { get; set; }
        public bool IsAvailable { get; set; }
    }
}