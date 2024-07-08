using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace TicketingSystem.Models {
    public class ApplicationDbContext : IdentityDbContext<AppUser, AppRole, string> {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) {
        }
        public DbSet<Ticket> ITS_Tickets { get; set; }
        public DbSet<TicketActivity> ITS_TicketActivities { get; set; }
        protected override void OnModelCreating(ModelBuilder builder) {
            base.OnModelCreating(builder);

            // Customize the ASP.NET Identity model and override the default table names
            builder.Entity<AppUser>(entity => {
                entity.ToTable(name: "ITS_Users");
                entity.Property(field => field.IsActive).HasColumnType("BIT");
                entity.Property(field => field.CreateDate).HasColumnType("DATETIME");
                entity.Property(field => field.UpdateDate).HasColumnType("DATETIME");
            });
            builder.Entity<AppRole>(entity => {
                entity.ToTable(name: "ITS_Roles");
                entity.Property(field => field.IsActive).HasColumnType("BIT");
                entity.Property(field => field.CreateDate).HasColumnType("DATETIME");
                entity.Property(field => field.UpdateDate).HasColumnType("DATETIME");
            });
            builder.Entity<IdentityUserRole<string>>(entity => entity.ToTable(name: "ITS_UserRoles"));
            builder.Entity<IdentityUserClaim<string>>(entity => entity.ToTable(name: "ITS_UserClaims"));
            builder.Entity<IdentityUserLogin<string>>(entity => entity.ToTable(name: "ITS_UserLogins"));
            builder.Entity<IdentityRoleClaim<string>>(entity => entity.ToTable(name: "ITS_RoleClaims"));
            builder.Entity<IdentityUserToken<string>>(entity => entity.ToTable(name: "ITS_UserTokens"));
            builder.Entity<Ticket>(entity => {
                entity.Property(field => field.ClosedFlag).HasColumnType("BIT");
            });
        }

    }
}

