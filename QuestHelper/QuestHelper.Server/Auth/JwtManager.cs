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
        public JwtManager()
        {
        }
        public string GetEncodedJwt(ClaimsIdentity identity, string userKey)
        {
            var now = DateTime.UtcNow;
            // создаем JWT-токен
            var jwt = new JwtSecurityToken(
                issuer: AuthOptions.ISSUER,
                audience: AuthOptions.AUDIENCE,
                notBefore: now,
                claims: identity.Claims,
                expires: now.Add(TimeSpan.FromDays(30)),
                signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(),
                    SecurityAlgorithms.HmacSha256));
            jwt.Payload["UserKey"] = userKey;

            return new JwtSecurityTokenHandler().WriteToken(jwt);
        }
        public string GetUserKeyFromToken(string jwtToken)
        {
            string userKey = string.Empty;

            var handler = new JwtSecurityTokenHandler();
            if (!string.IsNullOrEmpty(jwtToken))
            {
                var token = handler.ReadJwtToken(jwtToken);
                userKey = token.Payload["UserKey"] as string;
            }

            return userKey;
        }
    }
}
