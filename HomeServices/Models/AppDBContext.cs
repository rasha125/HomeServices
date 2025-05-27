using Microsoft.EntityFrameworkCore;

namespace HomeServices.Models
{
    public class AppDBContext : DbContext
    {
        public AppDBContext(DbContextOptions<AppDBContext> options) : base(options)
        {
        }

        public DbSet<Issues> Issues { get; set; }
        public DbSet<Users> Users { get; set; }
        public DbSet<Services> Services { get; set; }
        public DbSet<Messages> Messages { get; set; }
        public DbSet<Orders> Orders { get; set; }
        public DbSet<Persons> Persons { get; set; }
        public DbSet<Providers> Providers { get; set; }
        public DbSet<Ratings> Ratings { get; set; }





    }
}
