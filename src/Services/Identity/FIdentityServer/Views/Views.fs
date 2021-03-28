module Views

open System
open System.ComponentModel.DataAnnotations
open Giraffe
open GiraffeViewEngine
open IdentityServer4.Models
open Microsoft.AspNetCore.Http
open ViewHelpers
open Helpers
open System.Linq

let private (!?) x =
    toOption x

module Shared =
    type ScopeViewModel =
        { Value : string
          DisplayName : string
          Description : string option
          Emphasize : bool
          Required : bool
          Checked : bool }

    let scopeListItem (model : ScopeViewModel) =
        li [ _class "list-group-item" ] [
            label [] [
                input [ _class "consent-scopecheck"
                        _type "checkbox"
                        _name "ScopesConsented"
                        _id $"scopes_{model.Value}"
                        _value model.Value
                        if model.Checked then _checked else ()
                        if model.Required then _disabled else () ]
                
                if model.Required
                then input [ _type "hidden"; _name "ScopesConsented"; _value model.Value ]
                else ()
                
                strong [] [ str model.DisplayName ]
                
                if model.Emphasize
                then span [ _class "glyphicon glyphicon-exclamation-sign" ] [ ]
                else ()
            ]
            
            if model.Required
            then span [] [
                    em [] [ str "(required)" ]
                ]
            else ()
            
            match model.Description with
            | Some description ->
                div [ _class "consent-description" ] [
                    label [ _for $"scopes_{model.Value}" ] [
                        str description
                    ]
                ]
            | None -> () 
        ]
        
    type ErrorViewModel = { Error : ErrorMessage option }
        
    let errorView (model : ErrorViewModel) =
        let error = model.Error |> Option.bind (fun x -> !? x.Error)
        let description = model.Error |> Option.bind (fun x -> !? x.ErrorDescription)
        let requestId = model.Error |> Option.bind (fun x -> !? x.RequestId)
        
        div [ _class "error-page" ] [
            div [ _class "lead" ] [
                h1 [] [ str "Error" ]
            ]
            
            div [ _class "row" ] [
                div [ _class "col-sm-6" ] [
                    div [ _class "alert alert-danger" ] [
                        str "Sorry, there was an error"
                        
                        match error with
                        | Some error ->
                            strong [] [
                                em [] [ str $": {error}" ]
                            ]
                            match description with
                            | Some description -> div [] [ str description ]
                            | None -> ()
                        | None -> ()
                    ]
                    
                    match requestId with
                    | Some requestId -> div [ _class "request-id" ] [ str $"Request Id: {requestId}" ]
                    | None -> ()
                ]
            ]
        ]

    type RedirectViewModel = { RedirectUrl : string }

    let redirect (model : RedirectViewModel) =
        div [] [
            div [ _class "redirect-page" ] [
                div [ _class "lead" ] [
                    h1 [ ] [ str "You are now being returned to the application" ]
                    p [ ] [ str "Once complete, you may close this tab." ]
                ]
            ]
            
            meta [ _httpEquiv "refresh"; _content $"0;url={model.RedirectUrl}"; _dataUrl model.RedirectUrl ]
            script [ _src "/js/signin-redirect.js" ] [ ]
        ]

    let navigation (ctx : HttpContext) =
        div [ _class "nav-page" ] [
            nav [ _class "navbar navbar-expand-lg navbar-dark bg-dark" ] [
                a [ _href "/"; _class "navbar-brand" ] [
                    img [ _src "/icon.png"; _class "icon-banner" ]
                    str "IdentityServer4"
                ]
                
                match !? ctx.User.Identity.Name with
                | Some name ->
                    ul [ _class "navbar-nav mr-auto" ] [
                        li [ _class "nav-item dropdown" ] [
                            a [ _href "#"; _class "nav-link dropdown-toggle"; _dataToggle "dropdown" ] [
                                str name
                                b [ _class "caret" ] [ ]
                            ]
                            
                            div [ _class "dropdown-menu" ] [
                                a [ _class "dropdown-item"; _href "Logout/Account" ] [
                                    str "Logout"
                                ]
                            ]
                        ]
                    ]
                | None -> ()
            ]
        ]

    let layout (ctx : HttpContext) renderBody =
        html [ _lang "en" ] [
            head [] [
                meta [ _charset "utf-8" ]
                meta [ _httpEquiv "X-UA-Compatible"; _content "IE=edge" ]
                meta [ _name "viewport"; _content "width=device-width, initial-scale=1.0, shrink-to-fit=no" ]
                
                title [ ] [ str "IdentityServer4" ]
                
                link [ _rel "icon"; _type "image/x-icon"; _href "/favicon.ico" ]
                link [ _rel "shortcut icon"; _type "image/x-icon"; _href "/favicon.ico" ]
                
                link [ _rel "stylesheet"; _href "/lib/bootstrap/dist/css/bootstrap.min.css" ]
                link [ _rel "stylesheet"; _href "/css/site.css" ]
            ]
            
            body [] [
                navigation ctx
                
                div [ _class "container body-container" ] [
                    renderBody
                ]
                
                script [ _src "/lib/jquery/dist/jquery.slim.min.js"] [ ]
                script [ _src "/lib/bootstrap/dist/js/bootstrap.bundle.min.js"] [ ]
            ]
        ]
        
module Home = 
    let index =
        div [ _class "welcome-page" ] [
            h1 [] [
                img [ _src "/icon.png" ]
                str " Welcome to IdentityServer4 "
                small [ _class "text-muted" ] [ str "(version 4)" ]
            ]
            
            ul [] [
                li [] [
                    str "IdentityServer publishes a "
                    a [ _href "/.well-known/openid-configuration" ] [ str "discovery document" ]
                    str " where you can find metadata and links to all the endpoints, key material, etc."
                ]
                li [] [
                    str "Click "; a [ _href "/diagnostics" ] [ str "here" ]; str " to see the claims for your current session."
                ]
                li [] [
                    str "Click "; a [ _href "/grants" ] [ str "here" ]; str " to manage your stored grants."
                ]
                li [] [
                    str "Here are links to the"
                    a [ _href "https://github.com/identityserver/IdentityServer4" ] [ str "source code repository" ]
                    str ", and "
                    a [ _href "https://github.com/IdentityServer/IdentityServer4/tree/main/samples" ] [ str "ready to use samples" ]
                    str "."
                ]
            ]
        ]
        
module Account =
    type LoginInputModel =
        { [<Required>]
          Username      : string
          [<Required>]
          Password      : string
          RememberLogin : bool
          ReturnUrl     : string }
    
    type ExternalProvider =
        { DisplayName : string
          AuthenticationScheme : string }
    
    type LoginViewModel =
        { ReturnUrl          : string
          AllowRememberLogin : bool
          Username           : string
          EnableLocalLogin   : bool
          ExternalProviders  : ExternalProvider list }
          member this.VisibleExternalProviders =
              this.ExternalProviders |> List.filter (fun x -> not (String.IsNullOrWhiteSpace(x.DisplayName)))
          member this.IsExternalLoginOnly =
              this.EnableLocalLogin = false && this.ExternalProviders.Length = 1
          member this.ExternalLoginScheme =
              if this.IsExternalLoginOnly
              then
                  match this.ExternalProviders.SingleOrDefault() |> toOption with
                  | Some provider -> toOption provider.AuthenticationScheme
                  | None -> None
              else None
    
    let login (model : LoginViewModel) =
        div [ _class "login-page" ] [
            div [ _class "lead" ] [
                h1 [] [ str "Login" ]
            ]
            
            div [ _class "row" ] [
                if model.EnableLocalLogin then
                    div [ _class "col-sm-6" ] [
                        div [ _class "card" ] [
                            div [ _class "card-header" ] [
                                h2 [] [ str "Local Account" ]
                            ]
                            
                            div [ _class "card-body"] [
                                form [ _action "/account/login"; _method "POST" ] [
                                    input [ _type "hidden"; _name "ReturnUrl"; _value model.ReturnUrl ]
                                    
                                    div [ _class "form-group" ] [
                                        label [ _for "Username" ] [ str "Username" ]
                                        input [ _class "form-control"; _placeholder "Username"; _name "Username"; _autofocus ]
                                    ]
                                    div [ _class "form-group" ] [
                                        label [ _for "Password" ] [ str "Password" ]
                                        input [ _type "password"; _class "form-control"; _placeholder "Password"; _name "Password"; _autocomplete "off" ]
                                    ]
                                    if model.AllowRememberLogin then
                                        div [ _class "form-group" ] [
                                            div [ _class "form-check" ] [
                                                input [ _class "form-check-input"; _for "RememberLogin"]
                                                label [ _class "form-check-label"; _name "RememberLogin"] [
                                                    str "Remember My Login"
                                                ]
                                            ]
                                        ]
                                    else ()
                                    button [ _class "btn btn-primary"; _name "button"; _value "login" ] [ str "Login" ]
                                    button [ _class "btn btn-secondary"; _name "button"; _value "cancel" ] [ str "Cancel" ]
                                ]
                            ]
                        ]
                    ]
                else ()
                
                if model.VisibleExternalProviders.Any() then
                    div [ _class "col-sm-6" ] [
                        div [ _class "card" ] [
                            div [ _class "card-header" ] [
                                h2 [] [ str "External Account" ]
                            ]
                            div [ _class "card-body" ] [
                                ul [ _class "list-inline" ] [
                                    for provider in model.ExternalProviders do
                                        li [ _class "list-inline-item" ] [
                                            a [ _class "btn btn-secondary"
                                                _href $"/External/Challenge?scheme={provider.AuthenticationScheme}&returnUrl={model.ReturnUrl}" ] [
                                                str provider.DisplayName
                                            ]
                                        ]
                                ]
                            ]
                        ]
                    ]
                else ()
                
                if not model.EnableLocalLogin && model.VisibleExternalProviders.Any() then
                    div [ _class "alert alert-warning" ] [
                        strong [] [ str "Invalid login request" ]
                        str " There are no login schemes configured for this request."
                    ]
                else ()
            ]
        ]
        
    type LogoutViewModel =
        { LogoutId : string }
        
    let logout (model : LogoutViewModel) =
        div [ _class "logout-page" ] [
            div [ _class "lead" ] [
                h1 [] [ str "logout" ]
                p [] [ str "Would you like to logout of IdentityServer?" ]
            ]
            
            form [ _action "/account/logout"; _method "POST" ] [
                input [ _type "hidden"; _name "logoutId"; _value model.LogoutId ]
                div [ _class "form-group" ] [
                    button [ _class "btn btn-primary" ] [ str "Yes" ]
                ]
            ]
        ]
    
    type LoggedOutViewModel =
        { PostLogoutRedirectUri         : string option
          ClientName                    : string
          SignOutIframeUrl              : string option
          AutomaticRedirectAfterSignOut : bool
          LogoutId                      : string }
        
    let loggedOut (model : LoggedOutViewModel) =
        [
            div [ _class "logged-out-page" ] [
                h1 [] [
                    str "Logout "
                    small [] [ str "You are now logged out" ]
                ]
                
                match model.PostLogoutRedirectUri with
                | Some postLogoutRedirectUri ->
                    div [] [
                        str "Click "
                        a [ _class "PostLogoutRedirectUri"; _href postLogoutRedirectUri ] [ str "here" ]
                        str " to return to the "
                        span [] [ str model.ClientName ]
                        str " application."
                    ]
                | None -> ()
                
                match model.SignOutIframeUrl with
                | Some signOutIframeUrl ->
                    iframe [ _width "0"; _height "0"; _class "signout"; _src signOutIframeUrl ] [ ]
                | None -> ()
            ]
            
            if model.AutomaticRedirectAfterSignOut
            then script [ _src "/js/signout-redirect.js" ] []
            else ()
        ]
        
    let accessDenied =
        div [ _class "container" ] [
            div [ _class "lead" ] [
                h1 [] [ str "Access Denied" ]
                p [] [ str "You do not have access to that resource." ]
            ]
        ]