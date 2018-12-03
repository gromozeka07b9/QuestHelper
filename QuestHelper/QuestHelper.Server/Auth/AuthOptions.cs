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
        public const string AUDIENCE = "http://localhost:31193/"; // потребитель токена
        const string KEY = "";   // ключ для шифрации
        public const int LIFETIME = 1; // время жизни токена - 1 минута
        public static SymmetricSecurityKey GetSymmetricSecurityKey()
        {
            return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(KEY));
        }
    }
}
