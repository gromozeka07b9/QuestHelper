using QuestHelper.Managers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace QuestHelper
{
    public class Logger : ITextfileLogger
    {
        StringBuilder _sb = new StringBuilder();
        private string _pathToLog = Path.Combine(ImagePathManager.GetPicturesDirectory(), "sync.log");
        private bool _saveEachEvent = false;

        public Logger(bool saveEachEvent)
        {
            _saveEachEvent = saveEachEvent;
        }

        public void AddStringEvent(string textEvent)
        {
            var currentDateTimeString = DateTime.Now.ToString();
            _sb.AppendLine(currentDateTimeString);
            _sb.AppendLine(textEvent);
            Console.WriteLine(currentDateTimeString);
            Console.WriteLine(textEvent);
            if(_saveEachEvent) SaveReport();
        }

        public void SaveReport()
        {
            try
            {
                File.AppendAllText(_pathToLog, _sb.ToString(), Encoding.UTF8);
                _sb = new StringBuilder();
            }
            catch (Exception e)
            {
                HandleError.Process("Logger", "SaveReport", e, false);
            }
        }

        public void NewFile()
        {
            try
            {
                File.WriteAllText(_pathToLog, $"{DateTime.Now.ToString()} started", Encoding.UTF8);
            }
            catch (Exception e)
            {
                HandleError.Process("Logger", "NewFile", e, false);
            }
        }
    }
}
