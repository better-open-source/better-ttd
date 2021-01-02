namespace BetterTTD.FOAN

open System
open Akka.FSharp
open Avalonia
open Avalonia.Controls.ApplicationLifetimes
open Avalonia.FuncUI

open BetterTTD.FOAN.Actors.ActorsModule
open BetterTTD.FOAN.Actors.ActorStateMessages

type App() =
    inherit Application()

    let system = System.create "System" <| Configuration.load ()
    
    override this.Initialize() =
        
        let adminRef = spawn system "adminCoordinator" <| adminCoordinator
        adminRef <! Connect("127.0.0.1", "p7gvv", 3977)
        
        this.Styles.Load "avares://Avalonia.Themes.Default/DefaultTheme.xaml"
        this.Styles.Load "avares://Avalonia.Themes.Default/Accents/BaseDark.xaml"
        this.Styles.Load "avares://BetterTTD.FOAN/Styles.xaml"

    override this.OnFrameworkInitializationCompleted() =
        match this.ApplicationLifetime with
        | :? IClassicDesktopStyleApplicationLifetime as desktopLifetime ->
            desktopLifetime.MainWindow <- Shell.MainWindow()
        | _ -> ()

module Program =

    [<EntryPoint>]
    let main (args: string []) =
        AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .UseSkia()
            .StartWithClassicDesktopLifetime(args)
