using QuestHelper.iOS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[assembly: Xamarin.Forms.Dependency(typeof(ToastService))]
namespace QuestHelper.iOS
{
    public class ToastService : IToastService
    {
        public void LongToast(string message)
        {
            //Toast.MakeText(Application.Context, message, ToastLength.Long).Show();
        }

        public void ShortToast(string message)
        {
            //Toast.MakeText(Application.Context, message, ToastLength.Short).Show();
        }
    }
}