namespace SmartClinic.Contracts
{
    public class DoctorViewDto
    {
        public int DoctorID { get; set; }
        public string FullName { get; set; }
        public string SpecializationName { get; set; }
        public string Email { get; set; }
        public string ContactNumber { get; set; }
        public decimal ConsultationFee { get; set; }
        public bool IsAvailable { get; set; }
        public string Bio { get; set; }
    }
}