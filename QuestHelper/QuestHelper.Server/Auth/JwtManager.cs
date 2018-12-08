using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using QuestHelper.Server.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace QuestHelper.Server.Auth
{
    public class JwtManager
    {
        private DbContextOptions<ServerDbContext> _dbOptions = ServerDbContext.GetOptionsContextDbServer();

        public JwtManager(DbContextOptions<ServerDbContext> dbOptions)
        {
            _dbOptions = dbOptions;
        }
        public string GetEncodedJwt(ClaimsIdentity identity)
        {
            var now = DateTime.UtcNow;
            // создаем JWT-токен
            var jwt = new JwtSecurityToken(
                issuer: AuthOptions.ISSUER,
                audience: AuthOptions.AUDIENCE,
                notBefore: now,
                claims: identity.Claims,
                expires: now.Add(TimeSpan.FromDays(10)),
                signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(),
                    SecurityAlgorithms.HmacSha256));
            return new JwtSecurityTokenHandler().WriteToken(jwt);
        }

        internal bool TokenHashRequestAndDbAreEqual(string name, string requestToken)
        {
            string requestHash = ByteArrayToString(GetHashForString(requestToken));
            using (var context = new ServerDbContext(_dbOptions))
            {
                User user = context.User.FirstOrDefault(x => x.Name == name);
                if (user != null)
                {
                    return requestHash.Equals(user.TokenHash);
                }
            }
            return false;
        }

        public void WriteJwtHashToDb(User user, string token)
        {
            if (!string.IsNullOrEmpty(user.UserId))
            {
                var hash = GetHashForString(token);
                using (var context = new ServerDbContext(_dbOptions))
                {
                    var entity = context.User.Find(user.UserId);
                    if (entity != null)
                    {
                        user.TokenHash = ByteArrayToString(hash);
                        context.Entry(entity).CurrentValues.SetValues(user);
                        context.SaveChanges();
                    }
                    else throw new Exception("Not found user by UserId");
                }
            }
            else
            {
                throw new Exception("User can't have empty Id!");
            }
        }

        private byte[] GetHashForString(string token)
        {
            byte[] tokenInBytes = ASCIIEncoding.ASCII.GetBytes(token);
            var hash = new MD5CryptoServiceProvider().ComputeHash(tokenInBytes);
            return hash;
        }

        string ByteArrayToString(byte[] arrInput)
        {
            int i;
            StringBuilder sOutput = new StringBuilder(arrInput.Length);
            for (i = 0; i < arrInput.Length; i++)
            {
                sOutput.Append(arrInput[i].ToString("X2"));
            }
            return sOutput.ToString();
        }
    }
}
