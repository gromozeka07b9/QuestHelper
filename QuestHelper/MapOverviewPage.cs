using System;

using Xamarin.Forms;

namespace QuestHelper
{
    public class MapOverviewPage : ContentPage
    {
        public MapOverviewPage()
        {
            Content = new StackLayout
            {
                Children = {
                    new Label { Text = "Hello ContentPage" }
                }
            };
        }
    }
}

