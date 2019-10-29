using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace QuestHelper.Resources
{
    [ContentProperty("Text")]
    public class TranslationExtension : IMarkupExtension
    {
        public string Text { get; set; }

        public object ProvideValue(IServiceProvider serviceProvider)
        {
            if (Text is null)
            {
                return null;
            }

            return ResourceController.GetString(Text);
        }
    }
}
