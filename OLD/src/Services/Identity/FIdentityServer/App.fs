module App

open System
open System.Web
open Giraffe
open IdentityServer4
open IdentityServer4.Models
open IdentityServer4.Services
open IdentityServer4.Stores
open Microsoft.AspNetCore.Authentication
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.Hosting
open Microsoft.AspNetCore.Http
open Microsoft.Extensions.Logging
open FSharp.Control.Tasks.V2.ContextInsensitive
open Helpers
open Microsoft.Extensions.Primitives

let masterPage next ctx view =
    htmlView (Views.Shared.layout ctx view) next ctx

module Home =
    let indexHandler : HttpHandler =
        fun (next : HttpFunc) (ctx : HttpContext) ->
            masterPage next ctx Views.Home.index

    let errorHandler : HttpHandler =
        fun next ctx ->
            task {
                let interaction = ctx.GetService<IIdentityServerInteractionService>()
                let env = ctx.GetService<IWebHostEnvironment>()
                
                let result = masterPage next ctx 
                let viewBuilder msg = { Views.Shared.Error = msg } |> Views.Shared.errorView
                
                match ctx.GetQueryStringValue "errorId" with
                | Ok errorId ->
                    let! msg = interaction.GetErrorContextAsync(errorId)
                    match toOption msg with
                    | Some msg ->
                        if env.IsDevelopment() then return! result (Some msg |> viewBuilder)
                        else msg.ErrorDescription <- null; return! result (Some msg |> viewBuilder)
                    | None -> return! result (None |> viewBuilder)
                | Error _  -> return! result (None |> viewBuilder)
            }
        
    let routes =
        GET >=> choose [
            routeCi "/" >=> indexHandler
            routeCi "/home/error" >=> errorHandler
        ]


module Account =
    
    open Views.Account
    
    let private getDependencies (ctx : HttpContext) =
        ( ctx.GetService<IIdentityServerInteractionService>(),
          ctx.GetService<IClientStore>(),
          ctx.GetService<IAuthenticationSchemeProvider>(),
          ctx.GetService<IEventService>() )
    
    let private buildLoginViewModelAsync
        (interaction    : IIdentityServerInteractionService)
        (clientStore    : IClientStore)
        (schemeProvider : IAuthenticationSchemeProvider)
        (returnUrl      : string) =
        task {
            let! ctx = interaction.GetAuthorizationContextAsync(returnUrl)
            let ctx = toOption ctx
            let idp = ctx |> Option.bind (fun x -> toOption x.IdP)
            let username = ctx |> Option.bind (fun x -> toOption x.LoginHint) |> Option.defaultValue "N/A"
            
            match idp with
            | Some idp ->
                
                let! authScheme = schemeProvider.GetSchemeAsync(idp)
                let local = idp = IdentityServerConstants.LocalIdentityProvider
                return { ReturnUrl = returnUrl
                         AllowRememberLogin = true
                         EnableLocalLogin = local
                         Username = username
                         ExternalProviders =
                          if local then []
                          else [ { AuthenticationScheme = idp
                                   DisplayName = authScheme.DisplayName }
                               ] }
            | _ ->
                let! schemes = schemeProvider.GetAllSchemesAsync()
                let providers =
                    schemes
                    |> Seq.filter (fun x -> x.DisplayName |> toOption |> Option.isSome)
                    |> Seq.map (fun x -> { DisplayName = x.DisplayName; AuthenticationScheme = x.Name })
                    |> Seq.toList
                
                let result =
                    { AllowRememberLogin = true
                      EnableLocalLogin = true
                      ReturnUrl = returnUrl
                      Username = username
                      ExternalProviders = providers }
                
                match ctx
                      |> Option.bind (fun x -> toOption x.Client)
                      |> Option.bind (fun x -> toOption x.ClientId) with
                | Some clientId ->
                    let! client = clientStore.FindEnabledClientByIdAsync(clientId)
                    match toOption client with
                    | Some client ->
                        let result = { result with EnableLocalLogin = client.EnableLocalLogin }
                        match toOption client.IdentityProviderRestrictions with
                        | Some restrictions ->
                            let providers = providers |> List.filter (fun x -> restrictions.Contains(x.AuthenticationScheme))
                            return { result with ExternalProviders = providers }
                        | None -> return result
                    | None -> return result
                | None -> 
                    return result
        }

    let loginGetHandler : HttpHandler =
        fun (next : HttpFunc) (ctx : HttpContext) ->
            task {
                let result = masterPage next ctx 
                let (interaction, clientStore, schemeProvider, _) = getDependencies ctx
                match ctx.GetQueryStringValue "returnUrl" with
                | Ok returnUrl -> 
                    let! vm = buildLoginViewModelAsync interaction clientStore schemeProvider returnUrl
                    if vm.IsExternalLoginOnly
                    then return! redirectTo true $"/Challenge/External?scheme={vm.ExternalLoginScheme}&returnUrl={vm.ReturnUrl}" next ctx
                    else return! login vm |> result
                | Error err -> 
                    return! text err next ctx
            }

    let loginPostHandler : HttpHandler =
        fun (next : HttpFunc) (ctx : HttpContext) ->
            task {
                let result = masterPage next ctx 
                let (interaction, clientStore, schemeProvider, events) = getDependencies ctx
                
                let! body = ctx.ReadBodyFromRequestAsync()
                let queryItems = HttpUtility.ParseQueryString(body)
                let username = queryItems.Get("username")
                let password = queryItems.Get("password")
                let rememberLogin = queryItems.Get("RememberLogin")
                let returnUrl = queryItems.Get("ReturnUrl")
                let button = queryItems.Get("button")
                
                let! context = interaction.GetAuthorizationContextAsync(returnUrl)
                let context = toOption context
                
                match button with
                | "login" ->
                    return! text "kek" next ctx
                | _ ->
                    match context with
                    | Some context ->
                        do! interaction.DenyAuthorizationAsync(context, AuthorizationError.AccessDenied)
                        if not (context.RedirectUri.StartsWith("https", StringComparison.Ordinal)) &&
                           not (context.RedirectUri.StartsWith("http", StringComparison.Ordinal))
                        then
                            ctx.Response.StatusCode <- 200
                            ctx.Response.Headers.["Location"] <- StringValues ""
                            return! { Views.Shared.RedirectUrl = returnUrl }
                                    |> Views.Shared.redirect
                                    |> result
                        else return! redirectTo false returnUrl next ctx
                    | None -> return! redirectTo false "/" next ctx
            }
    
    let routes =
        choose [
            GET >=> choose [
                routeCi "/account/login" >=> loginGetHandler
            ]
            POST >=> choose [
                routeCi "/account/login" >=> loginPostHandler
            ]
        ]
        

let webApp : HttpFunc -> HttpContext -> HttpFuncResult =
    choose [
        Home.routes
        Account.routes
        setStatusCode 404 >=> text "Not Found"
    ]
    
let errorHandler (ex : Exception) (logger : ILogger) =
    logger.LogError(EventId(), ex, "An unhandled exception has occurred while executing the request.")
    clearResponse >=> setStatusCode 500 >=> text ex.Message
