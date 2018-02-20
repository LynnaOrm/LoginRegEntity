using Microsoft.EntityFrameworkCore;
 
namespace LoginRegEntity.Models
{
    public class LogRegContext : DbContext
    {
        // base() calls the parent class' constructor passing the "options" parameter along
        public LogRegContext(DbContextOptions<LogRegContext> options) : base(options) { }

        public DbSet<User> users {get;set;}
    }
}
