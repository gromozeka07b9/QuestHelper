﻿using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using QuestHelper.Managers.Sync;
using QuestHelper.Model.Messages;
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
            LoadApplication(new App());

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
