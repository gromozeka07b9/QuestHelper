using System;
using System.Collections.Generic;
using System.Text;

namespace QuestHelper.Consts
{
    public class TestMode
    {
        public static bool IsTestMode
        {
            get
            {
#if DEBUG
                return true;
#else
                return false;
#endif
            }
        }
    }
}
