namespace DroneLander
open DroneLander.Common
open Xamarin.Forms
open System.Threading.Tasks
open System

type LandingResultType =  Landed | Kaboom

type LandingParameters = {
    Altitude: double
    Velocity: double
    Fuel: double
    Thrust: double
}
with 
    static member Default() = {
        Altitude = CoreConstants.StartingAltitude 
        Velocity = CoreConstants.StartingVelocity 
        Fuel = CoreConstants.StartingFuel 
        Thrust = CoreConstants.StartingThrust
    }

type InFlightDetais = {
    Altitude: double
    DescentRate:double
    FuelRemaining:double
    Thrust:double
    Fuel:double
    Veloicty:double
}
with 
    static member Default() = {
        Altitude = CoreConstants.StartingAltitude; DescentRate = 0.; FuelRemaining = 1000.0; Thrust = 0.; Fuel = 1000.; Veloicty = 0.
    }
   
type MainViewModel(hasLanded) as this =
    inherit Common.ObservableBase()

    let defaultAltitude = LandingParameters.Default().Altitude
    let mutable activeLandingParameters = LandingParameters.Default()
    let mutable inFlightDetails = InFlightDetais.Default()
    let isActive () = activeLandingParameters.Altitude > 0. && activeLandingParameters.Altitude < defaultAltitude
    let isNotOnGround () = activeLandingParameters.Altitude > 0.

    let raisePropertiesChanged () = 
        this.OnPropertyChanged( <@ this.Altitude @>)
        this.OnPropertyChanged( <@ this.DescentRate @>)
        this.OnPropertyChanged( <@ this.FuelRemaining @>)
        this.OnPropertyChanged( <@ this.Thrust @>)
        this.OnPropertyChanged( <@ this.ActionLabel @>)

    let resetLanding () = 
        Task.Delay(500) |> Async.AwaitTask |> Async.StartImmediate

        activeLandingParameters <- LandingParameters.Default()
        inFlightDetails <- InFlightDetais.Default()
        this.Throttle <- 0.0

        raisePropertiesChanged()
   
    let updateFlightParameters () =

        let seconds = (float)CoreConstants.PollingIncrement / 1000.0

        // Compute thrust and remaining fuel
        //thrust = throttle * 1200.0;
        let used = (this.Throttle * seconds) / 10.0 |> min activeLandingParameters.Fuel
        let currentThrust = (used * 25000.0)

        // Compute new flight parameters
        let avgmass = CoreConstants.LanderMass + (used / 2.0)
        let force = currentThrust - (avgmass * CoreConstants.Gravity)
        let acc = force / avgmass

        let vel2 = activeLandingParameters.Velocity + (acc * seconds)
        let avgvel = (activeLandingParameters.Velocity + vel2) / 2.0

        activeLandingParameters <- { activeLandingParameters with 
                                        Thrust = currentThrust
                                        Fuel = activeLandingParameters.Fuel - used
                                        Altitude = activeLandingParameters.Altitude + (avgvel * seconds)
                                        Velocity = vel2 }


    let calculateInFlightDetails inFlightDetails (activeLandingParams: LandingParameters) = 
        {inFlightDetails with 
            Altitude = activeLandingParams.Altitude
            DescentRate = activeLandingParams.Velocity
            FuelRemaining = activeLandingParams.Fuel / 1000.
            Thrust = activeLandingParams.Thrust  }



    let startLanding () = 
        let update () =
            updateFlightParameters()

            if isNotOnGround() then 
                Device.BeginInvokeOnMainThread(fun () ->
                    inFlightDetails <- calculateInFlightDetails inFlightDetails activeLandingParameters)

                raisePropertiesChanged ()
                isNotOnGround()
            else
                activeLandingParameters <- {activeLandingParameters with Altitude = 0.} 

                Device.BeginInvokeOnMainThread(fun () ->
                    inFlightDetails <- calculateInFlightDetails inFlightDetails activeLandingParameters)

                if (activeLandingParameters.Velocity > -5.0) then 
                    hasLanded LandingResultType.Landed "The Eagle has landed!"
                else
                    hasLanded LandingResultType.Kaboom "That's going to leave a mark!"

                resetLanding() 
                this.OnPropertyChanged( <@ this.ActionLabel @>)
                raisePropertiesChanged ()
                false

        update () |> ignore

        Device.StartTimer(TimeSpan.FromMilliseconds(Common.CoreConstants.PollingIncrement |> float), fun () -> 

            if (inFlightDetails = InFlightDetais.Default() && activeLandingParameters = LandingParameters.Default()) then 
                false
            else
                update()
        )

         
    let handleIsActive _ = 
        match isActive () with 
        | true -> 
            resetLanding ()
        | false -> 
            startLanding ()

    let mutable actionLabel = "Start"

    let relayCommand = RelayCommand(handleIsActive, (fun _ -> true))

    let doubleToString (d:double) = d.ToString("F0")

    member val Throttle = 50.0 with get, set
    member this.AttemptLandingCommand with get() = relayCommand
    member this.ActionLabel with get() = if isActive() then "Reset" else "Start"
    member this.Altitude with get() = activeLandingParameters.Altitude |> doubleToString
    member this.DescentRate with get() = inFlightDetails.DescentRate |> doubleToString
    member this.Thrust with get() = inFlightDetails.Thrust |> doubleToString
    member this.FuelRemaining with get() = inFlightDetails.FuelRemaining



    
