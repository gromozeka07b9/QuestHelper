using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Gms.Common;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace QuestHelper.Droid
{
    public class PushReceiverSetup
    {
        internal static readonly string CHANNEL_ID = "my_notification_channel";
        internal static readonly int NOTIFICATION_ID = 100;
        private Context _context = null;

        public string SetupResultMessage { get; private set; }

        public PushReceiverSetup(Context context)
        {
            _context = context;
        }

        public void Setup()
        {

            IsPlayServicesAvailable();

            CreateNotificationChannel();

        }

        public bool IsPlayServicesAvailable()
        {
            int resultCode = GoogleApiAvailability.Instance.IsGooglePlayServicesAvailable(_context);
            if (resultCode != ConnectionResult.Success)
            {
                if (GoogleApiAvailability.Instance.IsUserResolvableError(resultCode))
                {
                    SetupResultMessage = GoogleApiAvailability.Instance.GetErrorString(resultCode);
                }
                else
                {
                    SetupResultMessage = "This device is not supported";
                }
                return false;
            }
            else
            {
                SetupResultMessage = "Google Play Services is available.";
                return true;
            }
        }

        void CreateNotificationChannel()
        {
            if (Build.VERSION.SdkInt < BuildVersionCodes.O)
            {
                // Notification channels are new in API 26 (and not a part of the
                // support library). There is no need to create a notification
                // channel on older versions of Android.
                return;
            }

#if DEBUG
            var channel = new NotificationChannel(CHANNEL_ID, "Gosh Notifications Test", NotificationImportance.Default) { Description = "Channel for Gosh push messages (TEST)" };
#else
            var channel = new NotificationChannel(CHANNEL_ID, "Gosh Notifications", NotificationImportance.Default) { Description = "Channel for Gosh push messages" };
#endif

            var notificationManager = (NotificationManager)_context.GetSystemService(Android.Content.Context.NotificationService);
            notificationManager.CreateNotificationChannel(channel);
        }
    }
}