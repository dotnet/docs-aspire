using AspirePostgreSQLEFCore.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace AspirePostgreSQLEFCore.Data;

public class TicketContext(DbContextOptions<TicketContext> options) : DbContext(options)
{
    public DbSet<SupportTicket> Tickets => Set<SupportTicket>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure PostgreSQL-specific settings
        modelBuilder.Entity<SupportTicket>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).HasMaxLength(200);
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.Priority).HasMaxLength(20);
            
            // Add some sample data
            entity.HasData(
                new SupportTicket
                {
                    Id = 1,
                    Title = "Initial Test Ticket",
                    Description = "This is a test ticket created during migration.",
                    Status = "Open",
                    Priority = "Low",
                    CreatedAt = DateTime.UtcNow
                }
            );
        });
    }
}