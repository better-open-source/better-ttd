module IdentityUser

open System.ComponentModel.DataAnnotations
open System.ComponentModel.DataAnnotations.Schema

[<CLIMutable>]
[<Table("UserLogins")>]
type UserLogin = {
    [<Key>]
    [<MaxLength(50)>]
    Id : int
    
    [<Required>]
    [<MaxLength(50)>]
    SubjectId : string
    
    [<Required>]
    [<MaxLength(250)>]
    LoginProvider : string
    
    [<Required>]
    [<MaxLength(250)>]
    ProviderKey : string
} with
    static member Create (subjectId, loginProvider, providerKey) = {
        Id = 0
        SubjectId = subjectId
        LoginProvider = loginProvider
        ProviderKey = providerKey
    }

[<CLIMutable>]
[<Table("Claims")>]
type UserClaim = {
    [<Key>]
    [<MaxLength(50)>]
    Id : int
    
    [<Required>]
    [<MaxLength(50)>]
    SubjectId : string
    
    [<Required>]
    [<MaxLength(250)>]
    ClaimType : string
    
    [<Required>]
    [<MaxLength(250)>]
    ClaimValue : string
} with
    static member Create cType cValue = {
        Id = 0
        SubjectId = null
        ClaimType = cType
        ClaimValue = cValue
    }

[<CLIMutable>]
[<Table("Users")>]
type User = {
    [<Key>]
    [<MaxLength(50)>]
    SubjectId : string
    
    [<Required>]
    [<MaxLength(100)>]
    Username : string
    
    [<Required>]
    [<MaxLength(100)>]
    Password : string
    
    [<Required>]
    IsActive : bool
    
    mutable Claims : List<UserClaim>
    mutable Logins : List<UserLogin>
}
