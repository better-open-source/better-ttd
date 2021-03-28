using System.Collections.Generic;
using IdentityServer4;
using IdentityServer4.Models;

namespace IdentityServer
{
    public class Config
    {
        public static IEnumerable<IdentityResource> GetIdentityResources() =>
            new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Address()
            };

        public static IEnumerable<ApiResource> GetApiResources() =>
            new List<ApiResource>();

        public static IEnumerable<Client> GetClients() =>
            new List<Client>
            {
                new()
                {
                    ClientName = "BetterTTD",
                    ClientId = "mvc",
                    AllowedGrantTypes = GrantTypes.Code,

                    AccessTokenType = AccessTokenType.Reference,

                    RequireConsent = false,
                    AccessTokenLifetime = 120,

                    UpdateAccessTokenClaimsOnRefresh = true,

                    AllowOfflineAccess = true,

                    RedirectUris = new List<string> {"https://localhost:6001/signin-oidc"},
                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Address
                    },
                    ClientSecrets = {new Secret("secret".Sha256())},
                    PostLogoutRedirectUris = {"https://localhost:6001/signout-callback-oidc"},
                }
            };
    }
}