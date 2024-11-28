using EQS.KMS.Application.Entities;
using EQS.KMS.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EQS.KMS.Infrastructure.Persistence
{
    public class ApplicationDbContext : DbContext, IApplicationDbContext
    {
        public DbSet<Customer> Customers { get; set; }
        public DbSet<CryptoSession> CryptoSessions { get; set; }
        public DbSet<KeySet> KeySets { get; set; }
        public DbSet<User> Users { get; set; }

        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<KeySet>().HasKey(k => new { k.Id, k.CustomerId });
        }
    }

}