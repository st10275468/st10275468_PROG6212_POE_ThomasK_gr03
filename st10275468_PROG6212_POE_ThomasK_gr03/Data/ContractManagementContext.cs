using Microsoft.EntityFrameworkCore;
using st10275468_PROG6212_POE_ThomasK_gr03.Models;

namespace st10275468_PROG6212_POE_ThomasK_gr03.Data
{
    public class ContractManagementContext : DbContext
    {
        public ContractManagementContext(DbContextOptions<ContractManagementContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Claim> Claims { get; set; }
        public DbSet<Document> Documents { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
           
            modelBuilder.Entity<Claim>()
                .HasOne(c => c.User)
                .WithMany(u => u.Claims)
                .HasForeignKey(c => c.userID);

            modelBuilder.Entity<Document>()
                .HasOne(d => d.Claim)
                .WithMany(c => c.Documents)
                .HasForeignKey(d => d.claimID);

           

            modelBuilder.Entity<Claim>()
                .Property(c => c.claimAmount)
                .HasColumnType("decimal(18, 2)");
        }
    }
}
