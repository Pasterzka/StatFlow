using Microsoft.EntityFrameworkCore;
using User_service.Models;

namespace User_service.Data
{
    public class UserDbContext : DbContext
    {
        public UserDbContext(DbContextOptions<UserDbContext> options) : base(options) { 
        }

        public DbSet<User> Users { get; set; }
    }
}
