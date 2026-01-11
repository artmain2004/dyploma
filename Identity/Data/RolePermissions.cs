using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Identity.Data
{
    [Table("RolePermissions")]
    public class RolePermissions
    {
        public IdentityRole IdentityRole { get; set; }
        public string IdentityRoleId { get; set; }
        public Persmission Persmission { get; set; }
        public int PersmissionId { get; set; }

    }
}
