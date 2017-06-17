namespace DroneLander

open System
open Xamarin.Forms
open Xamarin.Forms.Xaml
open System.Threading.Tasks

type MainPage() as this = 
    inherit ContentPage()
    let _ = base.LoadFromXaml(typeof<MainPage>)

    let ignoreUIExceptions f = 
        try 
            f () 
        with e -> ()

    let shakeLandscapeAsync () = 
        [0..8] |> List.iter (fun _ -> 
            ignoreUIExceptions <| fun _ -> 
                [
                    [| 
                       this.ScaleTo(1.1, 20u, Easing.Linear) 
                       this.TranslateTo(-30., 0., 20u, Easing.Linear)
                    |]
                    [|
                       this.ScaleTo(1.1, 20u, Easing.Linear)
                       this.TranslateTo(-30., 0., 20u, Easing.Linear) 
                    |]
                    [| this.TranslateTo(0., 0., 20u, Easing.Linear) |]
                    [| this.TranslateTo(0., -30., 20u, Easing.Linear)  |]
                    [|
                       this.ScaleTo(1.0, 20u, Easing.Linear)
                       this.TranslateTo(0., 0., 20u, Easing.Linear)
                    |]
                ] |> List.iter (fun f -> f |> Task.WhenAll |> Async.AwaitTask |> Async.Ignore |> Async.StartImmediate)
            )

    let handleEgaleLanding landingResult message = 
        if (landingResult = LandingResultType.Kaboom) then shakeLandscapeAsync()
        Device.BeginInvokeOnMainThread(fun () ->
            this.DisplayAlert(landingResult |> string, message, "OK") |> Async.AwaitTask |> Async.StartImmediate)

    do this.BindingContext <- MainViewModel(handleEgaleLanding)