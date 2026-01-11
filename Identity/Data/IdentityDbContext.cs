using Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Identity.Data
{
    public class IdentityDbContext : IdentityDbContext<Models.User>
    {
        public IdentityDbContext(DbContextOptions<IdentityDbContext> options) : base(options) 
        {
            
        }

        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<ApiKey> ApiKeys { get; set; }
        public DbSet<Persmission> Persmissions { get; set; }
        public DbSet<RolePermissions> RolePermissions { get; set; }
        public DbSet<UserAddress> UserAddresses { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {

            builder.Entity<RefreshToken>(b =>
            {
                b
                    .HasOne(r => r.User)
                    .WithMany(u => u.RefreshTokens)
                    .HasForeignKey(r => r.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<UserAddress>(b =>
            {
                b.HasKey(a => a.Id);
                b.Property(a => a.UserId).IsRequired();
                b.Property(a => a.Label).HasMaxLength(100);
                b.Property(a => a.Line1).IsRequired().HasMaxLength(200);
                b.Property(a => a.Line2).HasMaxLength(200);
                b.Property(a => a.City).IsRequired().HasMaxLength(100);
                b.Property(a => a.Region).HasMaxLength(100);
                b.Property(a => a.PostalCode).IsRequired().HasMaxLength(30);
                b.Property(a => a.Country).IsRequired().HasMaxLength(100);
                b.Property(a => a.Phone).HasMaxLength(40);
                b.Property(a => a.CreatedAtUtc).IsRequired();
                b.HasIndex(a => new { a.UserId, a.IsDefault });
                b.HasOne(a => a.User)
                    .WithMany(u => u.Addresses)
                    .HasForeignKey(a => a.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<ApiKey>(b =>
            {
                b.HasData(
                    new ApiKey
                    {
                        Id = Guid.NewGuid(),
                        Key = "pZD4cE8T0L97ycg4I8gtzn150mf6S9h6",
                        IsValid = true
                    }
                );
            });

            builder.Entity<RolePermissions>(b =>
            {
                b.HasKey(x => new { x.IdentityRoleId, x.PersmissionId });
                b.HasKey(rp => new { rp.IdentityRoleId, rp.PersmissionId });

                b.HasOne(rp => rp.IdentityRole)
                 .WithMany()
                 .HasForeignKey(rp => rp.IdentityRoleId);

                b.HasOne(rp => rp.Persmission)
                 .WithMany()
                 .HasForeignKey(rp => rp.PersmissionId);
            });

            builder.Entity<Persmission>(b =>
            {
                b.HasData(
                    new Persmission
                    {
                        Id = 1,
                        Name = Constants.Persmissions.Read
                    },
                    new Persmission
                    {
                        Id = 2,
                        Name = Constants.Persmissions.Create
                    },
                    new Persmission
                    {
                        Id = 3,
                        Name = Constants.Persmissions.Update
                    },
                    new Persmission
                    {
                        Id = 4,
                        Name = Constants.Persmissions.Delete
                    }
                );
            });

            base.OnModelCreating(builder);
        }
    }
}
