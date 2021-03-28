module Startup

open App
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Cors.Infrastructure
open Microsoft.Extensions.DependencyInjection
open Giraffe
open Microsoft.Extensions.Logging

type IIdentityServerBuilder with
    member builder.AddIdentityUserStore() =
        builder
            .AddProfileService<Services.ProfileService>()
            .Services
            .AddDbContext<Db.IdentityContext>(Db.cfg) |> ignore
        builder

let private configureCors (builder : CorsPolicyBuilder) =
    builder.WithOrigins("http://localhost:8080").AllowAnyMethod().AllowAnyHeader() |> ignore

let configureLogging (builder : ILoggingBuilder) =
    let filter (l : LogLevel) =
        l.Equals LogLevel.Debug
    builder.AddFilter(filter)
           .AddConsole()
           .AddDebug() |> ignore

let configureServices (services : IServiceCollection) =
    services.AddMvc() |> ignore
    services
      .AddIdentityServer()
      .AddIdentityUserStore()
      .AddDeveloperSigningCredential()
      .AddInMemoryIdentityResources(Config.identityResources)
      .AddInMemoryApiResources(Config.apiResources)
      .AddInMemoryClients(Config.clients) |> ignore
    
let configureApp (app : IApplicationBuilder) =
    app.UseCors(configureCors)
       .UseGiraffeErrorHandler(errorHandler)
       .UseIdentityServer()
       .UseHttpsRedirection()
       .UseStaticFiles()
       .UseRouting()
       .UseAuthorization()
       .UseGiraffe webApp
