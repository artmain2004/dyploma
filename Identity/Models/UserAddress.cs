namespace Identity.Models
{
    public class UserAddress
    {
        public Guid Id { get; set; }
        public string UserId { get; set; }
        public string? Label { get; set; }
        public string Line1 { get; set; }
        public string? Line2 { get; set; }
        public string City { get; set; }
        public string? Region { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
        public string? Phone { get; set; }
        public bool IsDefault { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public User User { get; set; }
    }
}
