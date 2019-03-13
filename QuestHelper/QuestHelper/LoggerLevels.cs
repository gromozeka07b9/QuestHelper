using System;
using System.Collections.Generic;
using System.Text;

namespace QuestHelper
{
    public class LoggerLevels
    {
        /// <summary>
        /// Используется для логгирования, чтобы не логгировать кол-во секунд, а только пороги, например - меньше 1 секунды, меньше 3 секунд, больше 3 секунд
        /// </summary>
        /// <param name="startDateTime"></param>
        /// <param name="finishDateTime"></param>
        /// <param name="level1"></param>
        /// <param name="level2"></param>
        /// <param name="level3"></param>
        /// <returns></returns>
        public double GetTimeLevels(DateTime startDateTime, DateTime finishDateTime, double level1, double level2, double level3)
        {
            double seconds = Math.Round((finishDateTime - startDateTime).TotalSeconds);
            if (seconds <= level1)
            {
                return level1;
            }
            else if (seconds <= level2)
            {
                return level2;
            }
            else if (seconds > level3)
            {
                return level3;
            }
            else return 0;
        }
    }
}
