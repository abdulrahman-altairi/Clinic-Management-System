
namespace Clinic.Contracts
{
    public class UpdateContactInfoDto
    {
        public int PersonId { get; set; }
        public string ContactNumber { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
    }
}
