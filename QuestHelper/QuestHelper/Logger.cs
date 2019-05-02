using System;
using System.Collections.Generic;
using System.Text;

namespace QuestHelper
{
    public class Logger
    {
        public Logger()
        {

        }

        public void AddStringEvent(string textEvent)
        {
            var currentDateTime = DateTime.Now;
            Console.WriteLine(currentDateTime);
        }
    }
}
