module UserRepository

open System
open System.Linq
open System.Linq.Expressions
open Db
open IdentityUser
open Microsoft.EntityFrameworkCore
open FSharp.Control.Tasks.V2

type FunAs() =
    static member LinqExpression<'T, 'TResult>(e: Expression<Func<'T, 'TResult>>) = e

type Email = string



let private getUserBy (ctx : IdentityContext, func : Expression<Func<_,bool>>) =
    task {
        let! usr = ctx.users.FirstOrDefaultAsync(func)
        if (box usr = null) then return None
        else return Some usr
    }

let getUserByName (ctx : IdentityContext, username : string) =
    task {
        let expr = FunAs.LinqExpression(fun (u : User) -> u.Username = username)
        return! getUserBy (ctx, expr)
    }

let getUserByEmail (ctx : IdentityContext, email : Email) =
    task {
        let expr =
            FunAs.LinqExpression(fun (u : User) -> u.Claims.Any(fun c -> c.ClaimType = "email" &&
                                                                         c.ClaimValue = email))
        return! getUserBy (ctx, expr)
    }

let getUserByProvider (ctx : IdentityContext, loginProvider : string, providerKey : string) =
    task {
        let expr =
            FunAs.LinqExpression(fun (u : User) -> u.Logins.Any(fun l -> l.LoginProvider = loginProvider &&
                                                                         l.ProviderKey = providerKey))
        return! getUserBy (ctx, expr)
    }

let getUserBySubjectId (ctx : IdentityContext, subjectId : string) =
    task {
        let expr = FunAs.LinqExpression(fun (u : User) -> u.SubjectId = subjectId)
        return! getUserBy (ctx, expr)
    }

let getUserClaimsBySubjectId (ctx : IdentityContext, subjectId : string) =
    task {
        match! getUserBySubjectId(ctx, subjectId) with
        | Some usr -> return usr.Claims
        | None     -> return []
    }

let getUserLoginsBySubjectId (ctx : IdentityContext, subjectId : string) =
    task {
        let! usr = ctx.users.Include("Logins").FirstOrDefaultAsync(fun u -> u.SubjectId = subjectId)
        if (box usr = null) then
            return []
        else
            return usr.Logins
    }

let areUserCredentialsValid (ctx : IdentityContext, username : string, password : string) =
    task {
        match! getUserByName (ctx, username) with
        | Some usr -> return usr.Username = username && usr.Password = password
        | None     -> return false
    }
    
let isUserActive (ctx : IdentityContext, subjectId : string) =
    task {
        match! getUserBySubjectId (ctx, subjectId) with
        | Some usr -> return usr.IsActive
        | None     -> return false
    }
    
let addUser (ctx : IdentityContext, user : User) =
    ctx.users.Add (user)
    
let addUserLogin (ctx : IdentityContext, subjectId : string, loginProvider : string, providerKey : string) =
    task {
        match! getUserBySubjectId (ctx, subjectId) with
        | Some usr ->
            let userLogin = UserLogin.Create (subjectId, loginProvider, providerKey)
            usr.Logins <- [userLogin] |> List.append usr.Logins
            ()
        | None -> return ()
    }
    
let save (ctx : IdentityContext) =
    task {
        let! changes = ctx.SaveChangesAsync()
        return changes >= 0
    }