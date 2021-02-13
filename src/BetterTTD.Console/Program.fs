namespace Temp

open System
open System.Net
open Akka.FSharp
open BetterTTD.Console.CoordinatorModule
open BetterTTD.Console.ActorMessagesModule

module Program =
     
    [<EntryPoint>]
    let main _ =
        let system = System.create "betterttd-system" <| Configuration.load ()
        let coordinatorRef = spawn system "coordinator" <| coordinator
        
        let joinMsg = Connect { Address = IPAddress.Parse ("127.0.0.1"); Port = 3977; Password = "p7gvv"; }
        coordinatorRef <! joinMsg
        
        Console.Read() |> ignore
        
        0