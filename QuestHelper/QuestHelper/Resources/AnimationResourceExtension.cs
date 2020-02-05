using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace QuestHelper.Resources
{
    [ContentProperty("Source")]
    public class AnimationResourceExtension : IMarkupExtension
    {
        public string Source { get; set; }

        public object ProvideValue(IServiceProvider serviceProvider)
        {
            if (Source is null)
            {
                return null;
            }
            return AnimationResourceController.GetPath(Source);
        }
    }
}
