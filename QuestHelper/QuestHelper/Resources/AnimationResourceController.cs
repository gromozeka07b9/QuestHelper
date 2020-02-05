using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace QuestHelper.Resources
{
    public static class AnimationResourceController
    {
        public static string GetPath(string filename)
        {
            return Device.RuntimePlatform == Device.iOS ? $"./Animations/{filename}.json" : $"Animations/{filename}.json";
        }

    }
}
