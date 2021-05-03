module Program

open System.IO
open System.Threading.Tasks
open Microsoft.EntityFrameworkCore
open Microsoft.Extensions.DependencyInjection
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.Hosting
open FSharp.Control.Tasks.V2

let migrate (ctx : Db.IdentityContext) =
    ctx.Database.Migrate()
    ctx.EnsureSeedDataAsync()
        |> Async.AwaitTask
        |> Async.RunSynchronously

let migrateDatabase<'db when 'db :> Db.IdentityContext> (host : IHost) =
    task {
        let scope = host.Services.CreateScope()
        let ctx = scope.ServiceProvider.GetRequiredService<'db>()
        do! ctx.Database.MigrateAsync()
        do! ctx.EnsureSeedDataAsync()
    } :> Task

let createHost args =
    Host.CreateDefaultBuilder(args)
        .ConfigureWebHostDefaults(
            fun builder ->
                builder
                    .UseKestrel()
                    .UseContentRoot(Directory.GetCurrentDirectory())
                    .ConfigureServices(Startup.configureServices)
                    .Configure(Startup.configureApp)
                    .ConfigureLogging(Startup.configureLogging)
                    |> ignore)
        .Build()

let run host =
    task {
        do! migrateDatabase<Db.IdentityContext>(host)
        do! host.RunAsync()
    } :> Task

[<EntryPoint>]
let main args =
    createHost args |> run
    |> Async.AwaitTask
    |> Async.RunSynchronously
    0