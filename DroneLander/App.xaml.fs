namespace DroneLander
open Xamarin.Forms
open Xamarin.Forms.Xaml
open System;
open System.Collections.Generic
open System.Linq
open System.Text
open System.Threading.Tasks

type App() = 
    inherit Application(MainPage = NavigationPage(MainPage()))
    let _ = base.LoadFromXaml(typeof<App>)


