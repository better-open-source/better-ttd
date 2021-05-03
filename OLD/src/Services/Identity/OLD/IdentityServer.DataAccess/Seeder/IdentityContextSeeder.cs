using System.Collections.Generic;
using System.Threading.Tasks;
using IdentityServer.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace IdentityServer.DataAccess.Seeder
{
    public class IdentityContextSeeder : IIdentityContextSeeder
    {
        private readonly IdentityContext _ctx;

        public IdentityContextSeeder(IdentityContext ctx) => 
            _ctx = ctx;

        public async Task SeedAsync()
        {
            if (await _ctx.Users.AnyAsync()) return;

            await _ctx.Users.AddRangeAsync(_users);
            await _ctx.SaveChangesAsync();
        }
        
        private readonly List<User> _users = new()
        {
            new User
            {
                SubjectId = "d860efca-22d9-47fd-8249-791ba61b07c7",
                Username = "Frank",
                Password = "password",
                IsActive = true
            },
            new User
            {
                SubjectId = "b7539694-97e7-4dfe-84da-b4256e1ff5c7",
                Username = "Claire",
                Password = "password",
                IsActive = true
            }
        };
    }
}