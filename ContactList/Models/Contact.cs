namespace ContactList.Models
{
    public class Contact
    {
        public int ContactID { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        public string? Notes { get; set; }
    }
}
