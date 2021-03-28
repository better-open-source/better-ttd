module Seed

open IdentityUser

let users =
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
