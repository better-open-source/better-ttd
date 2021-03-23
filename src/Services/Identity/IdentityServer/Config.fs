module Config

open IdentityServer4
open IdentityServer4.Models

let identityResources : IdentityResource list =
    [ IdentityResources.OpenId()
      IdentityResources.Profile()
      IdentityResources.Address() ]
    
let apiResources =
    [ ApiResource("openttdapi", "OpenTTD API") ]
    
let private betterttdClient =
    let client = Client()
    client.ClientName <- "BetterTTD"
    client.ClientId <- "betterttdclient"
    client.AllowedGrantTypes <- GrantTypes.Hybrid
    client.AccessTokenType <- AccessTokenType.Reference
    client.RequireConsent <- false
    client.AccessTokenLifetime <- 120
    client.UpdateAccessTokenClaimsOnRefresh <- true
    client.AllowOfflineAccess <- true
    client.RedirectUris <- [| "https://localhost:44355/signin-oidc" |]
    client.AllowedScopes <-
        [| IdentityServerConstants.StandardScopes.OpenId
           IdentityServerConstants.StandardScopes.Profile
           IdentityServerConstants.StandardScopes.Address |]
    client.ClientSecrets <-
        [| Secret("secret".Sha256()) |]
    client.PostLogoutRedirectUris <-
        [| "https://localhost:44355/signout-callback-oidc" |]
        
let clients : Client list =
    [ ]