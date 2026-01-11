using Identity.Data;
using Microsoft.AspNetCore.Identity;

namespace Identity.Models
{
    public class User : IdentityUser    
    {
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public int Age { get; set; }

        public ICollection<RefreshToken> RefreshTokens { get; set; } = [];
        public ICollection<UserAddress> Addresses { get; set; } = [];
    }
}
