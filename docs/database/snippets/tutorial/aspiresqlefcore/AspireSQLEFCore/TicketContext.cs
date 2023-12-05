using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata;

namespace AspireSQLEFCore
{
    public class TicketContext : DbContext
    {
        public TicketContext(DbContextOptions options) : base(options) { }

        public DbSet<SupportTicket> Tickets { get; set; }

    }
}
