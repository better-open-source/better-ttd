﻿module Db

open System
open Domain
open Microsoft.EntityFrameworkCore
open FSharp.Control.Tasks.V2

let private testUsers =
    [|
        { SubjectId = "d860efca-22d9-47fd-8249-791ba61b07c7"
          Username = "Frank"
          Password = "password"
          IsActive = true
          Claims = []
          Logins = [] }
        { SubjectId = "b7539694-97e7-4dfe-84da-b4256e1ff5c7"
          Username = "Claire"
          Password = "password"
          IsActive = true
          Claims = []
          Logins = [] }
    |]

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
                do! ctx.users.AddRangeAsync (testUsers)
                let! _ = ctx.SaveChangesAsync ()
                ()
        }
        
let cfg  =
    Action<_> (fun (builder : DbContextOptionsBuilder) ->
        builder.UseSqlServer(
            "Server=identityserver_db;Database=BetterTTD;User=sa;Password=Your_password123;Trusted_Connection=true;MultipleActiveResultSets=true;",
            fun x -> x.MigrationsAssembly "IdentityServer.Migrations" |> ignore)
        |> ignore)