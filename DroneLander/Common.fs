namespace DroneLander.Common
open System.Windows.Input
open System
open System.Collections.ObjectModel
open System.ComponentModel
open Microsoft.FSharp.Quotations
open Microsoft.FSharp.Quotations.Patterns
open System.Collections.Generic
open System.Globalization
open System.Text
open System.Threading.Tasks
open Xamarin.Forms

module CoreConstants = 
    let  Gravity = 3.711;      // Mars gravity (m/s2)
    let LanderMass = 17198.0; // Lander mass (kg)
    let PollingIncrement = 500;

    let StartingAltitude = 5000.0;
    let StartingVelocity = 0.0;
    let StartingFuel = 1000.0;
    let StartingThrust = 0.0;

type RelayCommand(execute: (obj -> unit), canExecute: (obj -> bool)) = 
    let event = new DelegateEvent<EventHandler>()

    interface ICommand with     
        [<CLIEvent>]
        member this.CanExecuteChanged = event.Publish

        member this.CanExecute arg = canExecute arg
        member this.Execute arg = execute arg

type ObservableBase() =
    let propertyChanged = new Event<_, _>()
    let toPropName(query : Expr) = 
        match query with
        | PropertyGet(a, b, list) ->
            b.Name
        | _ -> ""

    interface INotifyPropertyChanged with
        [<CLIEvent>]
        member x.PropertyChanged = propertyChanged.Publish

    abstract member OnPropertyChanged: string -> unit
    default x.OnPropertyChanged(propertyName : string) =
        propertyChanged.Trigger(x, new PropertyChangedEventArgs(propertyName))

    member x.OnPropertyChanged(expr : Expr) =
        let propName = toPropName(expr)
        x.OnPropertyChanged(propName)