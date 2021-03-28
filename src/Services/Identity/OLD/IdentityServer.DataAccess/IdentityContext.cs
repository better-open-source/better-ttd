using IdentityServer.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace IdentityServer.DataAccess
{
    public class IdentityContext : DbContext
    {
        public IdentityContext(DbContextOptions<IdentityContext> opt) 
            : base(opt)
        {
        }
        
        public DbSet<User> Users { get; set; }
    }
}