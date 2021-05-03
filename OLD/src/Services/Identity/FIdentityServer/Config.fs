module Config

open IdentityServer4
open IdentityServer4.Models

let identityResources : IdentityResource list =
    [ IdentityResources.OpenId()
      IdentityResources.Profile()
      IdentityResources.Address() ]
    
let apiResources : ApiResource list =
    [ ]
    
let private betterttdClient =
    let cli = Client()
    
    cli.ClientName <- "BetterTTD"
    cli.ClientId <- "mvc"
    cli.AllowedGrantTypes <- GrantTypes.Code
    
    cli.AccessTokenType <- AccessTokenType.Reference
    
    cli.RequireConsent <- false
    cli.AccessTokenLifetime <- 120
    cli.EnableLocalLogin <- true
    
    cli.UpdateAccessTokenClaimsOnRefresh <- true
    
    cli.AllowOfflineAccess <- true
    
    cli.RedirectUris <-
        [| "https://localhost:6001/signin-oidc" |]
    cli.AllowedScopes <-
        [| IdentityServerConstants.StandardScopes.OpenId
           IdentityServerConstants.StandardScopes.Profile
           IdentityServerConstants.StandardScopes.Address |]
    cli.ClientSecrets <-
        [| Secret("secret".Sha256()) |]
    cli.PostLogoutRedirectUris <-
        [| "https://localhost:6001/signout-callback-oidc" |]
    cli
        

let clients : Client list =
    [ betterttdClient ]