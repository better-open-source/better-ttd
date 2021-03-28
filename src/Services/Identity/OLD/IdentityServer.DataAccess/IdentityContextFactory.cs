using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace IdentityServer.DataAccess
{
    public class IdentityContextFactory : IDesignTimeDbContextFactory<IdentityContext>
    {
        public IdentityContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<IdentityContext>()
                .UseSqlServer("Server=localhost,55004;Database=BetterTTD;User=sa;Password=YourStrong@Passw0rd;TrustServerCertificate=true");
            
            return new IdentityContext(builder.Options);
        }
    }
}