namespace DroneLander.iOS.Renderers

open System
open System.Collections.Generic
open System.Linq
open System.Text
open Xamarin.Forms.Platform.iOS
open Xamarin.Forms

open UIKit
open CoreGraphics


type FuelControlRenderer() =
    inherit ProgressBarRenderer() 

    override this.OnElementChanged(e: ElementChangedEventArgs<ProgressBar>) = 
        base.OnElementChanged(e)
        if (this.Control <> null) then this.Control.TintColor <- UIColor.FromRGB(217, 0, 0);

    override this.LayoutSubviews() = 
        base.LayoutSubviews()

        let X = 1. |> nfloat
        let Y = 4. |> nfloat

        let transform = CGAffineTransform.MakeScale(X, Y)
        this.Control.Transform <- transform

[<assembly: ExportRenderer(typeof<ProgressBar>, typeof<FuelControlRenderer>)>] do()

type ThrottleControlRenderer() =
    inherit SliderRenderer()

    override this.OnElementChanged(e: ElementChangedEventArgs<Slider>) = 
        base.OnElementChanged(e)

        if (this.Control <> null) then 
            this.Control.SetThumbImage(UIImage.FromFile("throttle_thumb.png"), UIControlState.Normal);
            this.Control.TintColor <- UIColor.FromRGB(217, 0, 0);

[<assembly: ExportRenderer(typeof<Slider>, typeof<ThrottleControlRenderer>)>] do()
          



namespace DroneLander.iOS

open System
open UIKit
open Foundation
open Xamarin.Forms
open Xamarin.Forms.Platform.iOS

[<Register ("AppDelegate")>]
type AppDelegate () =
    inherit FormsApplicationDelegate ()

    override this.FinishedLaunching (app, options) =
        Forms.Init()
        this.LoadApplication (new DroneLander.App())
        base.FinishedLaunching(app, options)

module Main =
    [<EntryPoint>]
    let main args =
        UIApplication.Main(args, null, "AppDelegate")
        0

