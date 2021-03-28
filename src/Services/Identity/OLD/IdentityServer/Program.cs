using System;
using System.Threading.Tasks;
using IdentityServer.DataAccess;
using IdentityServer.DataAccess.Seeder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace IdentityServer
{
    public static class Program
    {
        private static async Task MigrateAsync(IHost host)
        {
            using var scope = host.Services.CreateScope();
            var ctx = scope.ServiceProvider.GetService<IdentityContext>();
            var seeder = scope.ServiceProvider.GetService<IIdentityContextSeeder>();
            
            await ctx!.Database.EnsureDeletedAsync();
            await ctx.Database.MigrateAsync();

            await seeder!.SeedAsync();
        }

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
        
        public static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            try
            {
                await MigrateAsync(host);
                await host.RunAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}