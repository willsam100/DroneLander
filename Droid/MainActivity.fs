namespace DroneLander.Droid.Renderers

open System
open Android.App
open Android.Content
open Android.Content.PM
open Android.Runtime
open Android.Views
open Android.Widget
open Android.OS
open Xamarin.Forms.Platform.Android
open Xamarin.Forms
open Android.Graphics

open Xamarin.Forms.Platform.Android
open Xamarin.Forms
open Android.Graphics
open Android.Graphics.Drawables
open Android.Support.V4.Content

type Resource = DroneLander.Droid.Resource

type ThrottleControlRenderer() = 
    inherit SliderRenderer()

    override this.OnElementChanged(e : ElementChangedEventArgs<Slider>) = 
        base.OnElementChanged(e)

        if (this.Control <> null) then 
            this.Control.ProgressDrawable.SetColorFilter(Android.Graphics.Color.Rgb(217, 0, 0), PorterDuff.Mode.SrcIn)
            let myThumb = ContextCompat.GetDrawable(this.Context, Resource.Drawable.throttle_thumb)
            this.Control.SetThumb(myThumb)

[<assembly: ExportRenderer(typeof<Slider>, typeof<ThrottleControlRenderer>)>] do()


type FuelControlRenderer() = 
    inherit ProgressBarRenderer()

    override this.OnElementChanged(e: ElementChangedEventArgs<Xamarin.Forms.ProgressBar>) = 
        base.OnElementChanged(e)

        if (this.Control <> null) then
            this.Control.ScaleY <- 4.0f;
            // Uncomment this and it will be nice and red but won't show as progress ie the bar is static and doesn't update
            //this.Control.ProgressDrawable.SetColorFilter(Android.Graphics.Color.Rgb(217, 0, 0), PorterDuff.Mode.SrcIn);

[<assembly: ExportRenderer(typeof<Xamarin.Forms.ProgressBar>, typeof<FuelControlRenderer>)>] do()


namespace DroneLander.Droid
open System
open Android.App
open Android.Content
open Android.Content.PM
open Android.Runtime
open Android.Views
open Android.Widget
open Android.OS

[<Activity (Label = "DroneLander.Droid", ConfigurationChanges = (ConfigChanges.ScreenSize ||| ConfigChanges.Orientation))>]
type MainActivity() =
    inherit Xamarin.Forms.Platform.Android.FormsApplicationActivity()
    override this.OnCreate (bundle: Bundle) =
        base.OnCreate (bundle)

        Xamarin.Forms.Forms.Init (this, bundle)
        this.LoadApplication (new DroneLander.App ())

[<Activity(Label = "Drone Lander", MainLauncher = true, NoHistory = true)>]
type SplashActivity() = 
    inherit Activity()

    override this.OnResume() = 
        base.OnResume()

        async {
            do! Async.Sleep 300
            this.StartActivity(new Intent(Application.Context, typeof<MainActivity>));
        } |> Async.StartImmediate

