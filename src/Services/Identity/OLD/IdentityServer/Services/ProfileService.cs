using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityServer.Services.Abstractions;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;

namespace IdentityServer.Services
{
    public class ProfileService : IProfileService
    {
        private readonly IIdentityUserRepository _repository;

        public ProfileService(IIdentityUserRepository repository) => 
            _repository = repository;

        public Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var subjectId = context.Subject.GetSubjectId();
            var claimsForUser = _repository.GetUserClaimsBySubjectId(subjectId);

            context.IssuedClaims = claimsForUser.Select
                (c => new Claim(c.ClaimType, c.ClaimValue)).ToList();

            return Task.FromResult(0);
        }

        public Task IsActiveAsync(IsActiveContext context)
        {
            var subjectId = context.Subject.GetSubjectId();
            context.IsActive = _repository.IsUserActive(subjectId);

            return Task.FromResult(0);
        }
    }
}