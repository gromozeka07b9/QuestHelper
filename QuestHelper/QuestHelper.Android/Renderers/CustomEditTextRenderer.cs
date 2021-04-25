using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Xamarin.Forms.Platform.Android;

namespace QuestHelper.Droid.Renderers
{
    public class CustomEditTextRenderer : FormsEditText
    {
        public CustomEditTextRenderer(Context context) : base(context)
        {
        }
        protected override void OnAttachedToWindow()
        {
            base.OnAttachedToWindow();

            Enabled = false;
            Enabled = true;
        }
    }
}