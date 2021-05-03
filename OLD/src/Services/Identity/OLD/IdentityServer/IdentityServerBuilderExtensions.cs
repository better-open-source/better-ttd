using IdentityServer.DataAccess;
using IdentityServer.Services;
using IdentityServer.Services.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityServer
{
    public static class IdentityServerBuilderExtensions
    {
        public static IIdentityServerBuilder AddIdentityUserStore(this IIdentityServerBuilder builder)
        {
            builder
                .AddProfileService<ProfileService>()
                .Services
                .AddTransient<IIdentityUserRepository, IdentityUserRepository>()
                .AddDbContext<IdentityContext>(opt =>
                    opt.UseSqlServer(
                        "Server=localhost,55004;Database=BetterTTD;User=sa;Password=YourStrong@Passw0rd;TrustServerCertificate=true"));
            return builder;
        }
    }
}
