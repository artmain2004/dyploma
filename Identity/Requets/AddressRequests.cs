using System.ComponentModel.DataAnnotations;

namespace Identity.Requets
{
    public class AddressCreateRequest
    {
        [MaxLength(100)]
        public string? Label { get; set; }

        [Required]
        [MaxLength(200)]
        public string Line1 { get; set; }

        [MaxLength(200)]
        public string? Line2 { get; set; }

        [Required]
        [MaxLength(100)]
        public string City { get; set; }

        [MaxLength(100)]
        public string? Region { get; set; }

        [Required]
        [MaxLength(30)]
        public string PostalCode { get; set; }

        [Required]
        [MaxLength(100)]
        public string Country { get; set; }

        [MaxLength(40)]
        public string? Phone { get; set; }

        public bool IsDefault { get; set; }
    }

    public class AddressUpdateRequest
    {
        [MaxLength(100)]
        public string? Label { get; set; }

        [Required]
        [MaxLength(200)]
        public string Line1 { get; set; }

        [MaxLength(200)]
        public string? Line2 { get; set; }

        [Required]
        [MaxLength(100)]
        public string City { get; set; }

        [MaxLength(100)]
        public string? Region { get; set; }

        [Required]
        [MaxLength(30)]
        public string PostalCode { get; set; }

        [Required]
        [MaxLength(100)]
        public string Country { get; set; }

        [MaxLength(40)]
        public string? Phone { get; set; }

        public bool IsDefault { get; set; }
    }
}
