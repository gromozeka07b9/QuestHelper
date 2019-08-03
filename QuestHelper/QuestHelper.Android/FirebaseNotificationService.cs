using Android.App;
using Android.Content;
using Firebase.Messaging;
using Microsoft.AppCenter.Analytics;
using QuestHelper.Model.Messages;
using System.Collections.Generic;

namespace QuestHelper.Droid
{
    [Service]
    [IntentFilter(new[] { "com.google.firebase.MESSAGING_EVENT" })]
    public class FirebaseNotificationService : FirebaseMessagingService
    {
        public override void OnMessageReceived(RemoteMessage message)
        {
            string messageBody = message.GetNotification().Body;
            string messageTitle = message.GetNotification().Title;
            Analytics.TrackEvent("Push message: received", new Dictionary<string, string> { { "Message", messageBody } });
            Xamarin.Forms.MessagingCenter.Send<SyncMessage>(new SyncMessage(), string.Empty);
            SendNotification(messageBody);
            Xamarin.Forms.MessagingCenter.Send<ReceivePushMessage>(new ReceivePushMessage(){MessageBody = messageBody, MessageTitle = messageTitle }, string.Empty);
        }
        void SendNotification(string messageBody)
        {
            NotificationService notify = new NotificationService();
            notify.Show(messageBody);
        }
    }
}