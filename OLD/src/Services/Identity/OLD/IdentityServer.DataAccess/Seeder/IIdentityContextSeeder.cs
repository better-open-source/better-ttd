using System.Threading.Tasks;

namespace IdentityServer.DataAccess.Seeder
{
    public interface IIdentityContextSeeder
    {
        Task SeedAsync();
    }
}