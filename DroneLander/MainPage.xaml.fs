namespace DroneLander

open System
open Xamarin.Forms
open Xamarin.Forms.Xaml

type MainPage() as this = 
    inherit ContentPage()
    let _ = base.LoadFromXaml(typeof<MainPage>)

    let awaitParallel = Array.map Async.AwaitTask >> Async.Parallel >> Async.Ignore
   
    let shakeLandscapeAsync () = 
        let shake =  async {
            do! [|
                    this.ScaleTo(1.1, 20u, Easing.Linear)
                    this.TranslateTo(-30., 0., 20u, Easing.Linear)
                |] |> awaitParallel
            do! [| this.TranslateTo(0., 0., 20u, Easing.Linear) |] |> awaitParallel
            do! [| this.TranslateTo(0., -30., 20u, Easing.Linear) |] |> awaitParallel
            do! [|
                    this.ScaleTo(1.0, 20u, Easing.Linear) 
                    this.TranslateTo(0., 0., 20u, Easing.Linear)
                |] |> awaitParallel
        }
        async { 
            for action in List.replicate 8 shake do 
                do! action 
        } |> Async.StartImmediate


    let handleEgaleLanding landingResult message = 
        if (landingResult = LandingResultType.Kaboom) then shakeLandscapeAsync()
        Device.BeginInvokeOnMainThread(fun () ->
            this.DisplayAlert(landingResult |> string, message, "OK") |> Async.AwaitTask |> Async.StartImmediate)

    do this.BindingContext <- MainViewModel(handleEgaleLanding)



