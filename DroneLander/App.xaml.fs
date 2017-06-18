namespace DroneLander
open Xamarin.Forms
open Xamarin.Forms.Xaml

type App() = 
    inherit Application(MainPage = NavigationPage(MainPage()))
    let _ = base.LoadFromXaml(typeof<App>)


