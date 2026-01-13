namespace Clinic.Contracts
{
    public class SpecializationDto
    {
        public int SpecializationId { get; set; }
        public string SpecializationName { get; set; }
        public string SpecializationDescription { get; set; }

        public int NumberOfDoctors { get; set; }
    }
}