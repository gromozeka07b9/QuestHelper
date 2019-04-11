using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace QuestHelper.Server.Managers
{
    public class HashGenerator
    {
        public static string Generate(string dataForHash)
        {
            string hash = string.Empty;
            using (SHA256 hashGenerator = SHA256.Create())
            {
                byte[] versionAsBytes = Encoding.ASCII.GetBytes(dataForHash);
                byte[] versionHash = hashGenerator.ComputeHash(versionAsBytes);

                hash = BitConverter.ToString(versionHash).Replace("-", string.Empty);
            }
            return hash;
        }
    }
}
