using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HomeServices.Models
{
    public class AppDBContext : IdentityDbContext<Users>
    {
        public AppDBContext(DbContextOptions<AppDBContext> options) : base(options)
        {
        }

        public DbSet<Issues> Issues { get; set; }

        // Do NOT add DbSet<Users> because IdentityDbContext handles it automatically
        //public DbSet<Users> Users { get; set; }
        public DbSet<Services> Services { get; set; }
        public DbSet<Orders> Orders { get; set; }
        public DbSet<Persons> Persons { get; set; }
        public DbSet<Providers> Providers { get; set; }
        public DbSet<Ratings> Ratings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Orders>()
                .HasOne(o => o.Providers)
                .WithMany()
                .HasForeignKey(o => o.ProviderId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Orders>()
                .HasOne(o => o.Persons)
                .WithMany()
                .HasForeignKey(o => o.PersonId)
                .OnDelete(DeleteBehavior.Restrict);  
        }





    }
}
