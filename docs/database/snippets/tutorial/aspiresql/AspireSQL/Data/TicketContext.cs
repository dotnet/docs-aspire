using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata;

namespace AspireSQL.Data
{
    public class TicketContext : DbContext
    {
        public TicketContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //use this to configure the contex
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SupportTicket>().HasData(new SupportTicket { Id = 1, Title = "Hello", Description = "World" });
        }

        public DbSet<SupportTicket> Tickets { get; set; }

    }
}
