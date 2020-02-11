using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading;

namespace QuestHelper.Consts
{
    public class DeviceCulture
    {
        public static CultureInfo CurrentCulture
        {
            get
            {
                return Thread.CurrentThread.CurrentCulture;
            }
        }
    }
}
