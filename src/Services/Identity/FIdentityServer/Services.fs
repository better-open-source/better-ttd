module Services

open System.Security.Claims
open System.Threading.Tasks
open IdentityServer4.Extensions
open FSharp.Control.Tasks.V2
open IdentityServer4.Services

type ProfileService(db : Db.IdentityContext) =
    interface IProfileService with
        member this.GetProfileDataAsync(ctx) =
            task {
                let subjectId = ctx.Subject.GetSubjectId()
                let! claimsForUser = UserRepository.getUserClaimsBySubjectId (db, subjectId)
                
                ctx.IssuedClaims <- claimsForUser
                                    |> List.map (fun c -> Claim(c.ClaimType, c.ClaimValue))
                                    |> ResizeArray<Claim>
            } :> Task
            
        member this.IsActiveAsync(ctx) =
            task {
                let subjectId = ctx.Subject.GetSubjectId()
                let! isActive = UserRepository.isUserActive (db, subjectId)
                ctx.IsActive <- isActive
            } :> Task