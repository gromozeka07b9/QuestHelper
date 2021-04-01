using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Media;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Views;
using Android.Widget;
using QuestHelper.Droid;

[assembly: Xamarin.Forms.Dependency(typeof(NotificationService))]
namespace QuestHelper.Droid
{
    public class NotificationService : INotificationService
    {
        private static NotificationCompat.Builder progressBuilder = null;

        public void Show(string textMessage)
        {
            var intent = new Intent(Application.Context, typeof(MainActivity));
            intent.AddFlags(ActivityFlags.ClearTop);
            intent.PutExtra("messageBodyText", "text:" + textMessage);
            var pendingIntent = PendingIntent.GetActivity(Application.Context, 0, intent, PendingIntentFlags.UpdateCurrent);

            var notificationBuilder = new NotificationCompat.Builder(Application.Context)
                .SetSmallIcon(Resource.Drawable.icon2)
                .SetContentTitle("Gosh!")
                .SetContentText(textMessage)
                .SetContentIntent(pendingIntent)
                .SetVisibility((int)NotificationVisibility.Public)
                    //.SetSound(RingtoneManager.GetDefaultUri(RingtoneType.Notification))
                .SetAutoCancel(true);

            var notificationManager = NotificationManager.FromContext(Application.Context);
            notificationManager.Notify(0, notificationBuilder.Build());
        }

        /*public void ShowProgress(int progressMax, int progressCurrent)
        {
            var intent = new Intent(Application.Context, typeof(MainActivity));
            intent.AddFlags(ActivityFlags.ClearTop);
            var pendingIntent = PendingIntent.GetActivity(Application.Context, 0, intent, PendingIntentFlags.UpdateCurrent);

            var notificationManager = NotificationManager.FromContext(Application.Context);
            if (progressBuilder == null)
            {
                progressBuilder = new NotificationCompat.Builder(Application.Context)
                    .SetSmallIcon(Resource.Drawable.icon2)
                    .SetContentTitle("Gosh!")
                    .SetContentText("Обновление")
                    .SetContentIntent(pendingIntent)
                    .SetProgress(progressMax, progressCurrent, true);
                notificationManager.Notify(1, progressBuilder.Build());
            }
            else
            {
                progressBuilder.SetProgress(progressMax, progressCurrent, false)
                    .SetContentText($"Обновлено {progressCurrent} из {progressMax}");
                notificationManager.Notify(1, progressBuilder.Build());
            }

            if (progressCurrent >= progressMax)
            {
                progressBuilder.SetProgress(0, 0, false)
                    .SetContentText($"Обновление завершено");
                notificationManager.Notify(1, progressBuilder.Build());

                progressBuilder.Dispose();
                progressBuilder = null;
            }
        }*/

        public void HideProgress()
        {
            if (progressBuilder != null)
            {
                progressBuilder.SetVisibility(0);
                var notificationManager = NotificationManager.FromContext(Application.Context);
                notificationManager.Notify(1, progressBuilder.Build());
            }
        }
    }
}