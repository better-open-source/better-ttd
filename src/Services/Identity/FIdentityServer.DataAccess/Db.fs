module Db

open System
open IdentityUser
open Microsoft.EntityFrameworkCore
open FSharp.Control.Tasks.V2
open Microsoft.EntityFrameworkCore.Design

let private connectionString =
    "Server=localhost,8015;Database=BetterTTD;User=sa;Password=Your_password123;"

type IdentityContext(opt : DbContextOptions<IdentityContext>) =
    inherit DbContext(opt)
    
    [<DefaultValue>]
    val mutable users : DbSet<User>
    member __.Users
        with get() = __.users
        and  set v = __.users <- v
    
    [<DefaultValue>]
    val mutable userLogins : DbSet<UserLogin>
    member __.UserLogins
        with get() = __.userLogins
        and  set v = __.userLogins <- v
        
    [<DefaultValue>]
    val mutable userClaims : DbSet<UserClaim>
    member __.UserClaims
        with get() = __.userClaims
        and  set v = __.userClaims <- v
    
    with
    member ctx.EnsureSeedDataAsync () =
        task {
            match! ctx.users.AnyAsync() with
            | true -> ()
            | false ->
                do! ctx.users.AddRangeAsync (Seed.users)
                let! _ = ctx.SaveChangesAsync ()
                ()
        }

let cfg =
    Action<DbContextOptionsBuilder> (fun (builder : DbContextOptionsBuilder) ->
        let migrationsAssembly = "FIdentityServer.Migrations"
        builder.UseSqlServer(
            connectionString,
            fun x -> x.MigrationsAssembly migrationsAssembly |> ignore)
        |> ignore)
    
type IdentityContextFactory() =
    interface IDesignTimeDbContextFactory<IdentityContext> with
        member this.CreateDbContext _ =
            let optsBuilder = DbContextOptionsBuilder<IdentityContext>()
            cfg.Invoke(optsBuilder)
            new IdentityContext(optsBuilder.Options)