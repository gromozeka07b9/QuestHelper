using System;
using System.Collections.Generic;
using System.Text;

namespace QuestHelper.Model.Messages
{
    public class PageNavigationMessage
    {
        public MainPages PageToOpen = MainPages.Private;
    }

    public enum MainPages { Feed = 0, OverviewMap, RouteMaker, Loaded, Private};
}
