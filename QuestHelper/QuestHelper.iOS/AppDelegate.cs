using System;
using System.Collections.Generic;
using System.Linq;
using Acr.UserDialogs;
using FFImageLoading.Forms.Platform;
using Foundation;
using ImageCircle.Forms.Plugin.iOS;
//using Lottie.Forms.iOS.Renderers;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using QuestHelper.Consts;
using QuestHelper.Managers.Sync;
using QuestHelper.Model.Messages;
using Syncfusion.ListView.XForms.iOS;
using UIKit;
using Xamarin.Forms;

namespace QuestHelper.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        static SyncPossibility _syncPossibility = new SyncPossibility();

        //
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            global::Xamarin.Forms.Forms.Init();
            SfListViewRenderer.Init();
            CachedImageRenderer.Init();
            Syncfusion.XForms.iOS.ProgressBar.SfCircularProgressBarRenderer.Init();
            Syncfusion.SfChart.XForms.iOS.Renderers.SfChartRenderer.Init();

            DeviceSize.FullScreenWidth = (int)UIScreen.MainScreen.Bounds.Size.Width;
            DeviceSize.FullScreenHeight = (int)UIScreen.MainScreen.Bounds.Size.Height;

            LoadApplication(new App());
            Xamarin.FormsMaps.Init();
            ImageCircleRenderer.Init();

            AppCenter.Start("f091513f-6c14-4845-8737-dbdc8dbeff2f", typeof(Analytics), typeof(Crashes));

            MessagingCenter.Subscribe<SyncMessage>(this, string.Empty, async (sender) =>
            {
                if (await _syncPossibility.CheckAsync(true))
                {
                    SyncService sync = new SyncService();
                    sync.Start(sender?.RouteId, sender.NeedCheckVersionRoute);
                }
            });

            return base.FinishedLaunching(app, options);
        }
    }
}
