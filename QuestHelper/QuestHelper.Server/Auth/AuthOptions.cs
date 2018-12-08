using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;

namespace QuestHelper.Server.Auth
{
    public class AuthOptions
    {
        public const string ISSUER = "iGosh.pro"; // издатель токена
#if DEBUG
        public const string AUDIENCE = "http://localhost:31193/"; // потребитель токена
#else
        public const string AUDIENCE = "http://igosh.pro"; // потребитель токена
#endif
        static string KEY = System.Environment.ExpandEnvironmentVariables("%GoshAuthTokenKey%");   // ключ для шифрации
        public static SymmetricSecurityKey GetSymmetricSecurityKey()
        {
            return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(KEY));
        }
    }
}
