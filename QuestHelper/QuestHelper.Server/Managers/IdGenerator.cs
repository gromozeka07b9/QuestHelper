using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuestHelper.Server.Managers
{
    /// <summary>
    /// Генерация id из словаря, может быть неуникален.
    /// </summary>
    public class IdGenerator
    {
        private static string _dictionary = "qwertyuiopasdfghjklzxcvbnm1234567890QWERTYUIOPASDFGHJKLZXCVBNM";
        /// <summary>
        /// Генерация Id
        /// </summary>
        /// <param name="length">длина итогой строки идентификатора</param>
        /// <returns></returns>
        public static string Generate(int length)
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < length; i++)
            {
                var rand = new Random();
                int position = rand.Next(_dictionary.Length - 1);
                sb.Append(_dictionary.Substring(position, 1));
            }

            return sb.ToString();
        }
    }
}
