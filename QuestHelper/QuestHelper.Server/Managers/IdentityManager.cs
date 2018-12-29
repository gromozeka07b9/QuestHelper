using QuestHelper.Server.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Http;

namespace QuestHelper.Server.Managers
{
    /// <summary>
    /// Чтение/запись Identity
    /// </summary>
    public class IdentityManager
    {
        public ClaimsIdentity GetIdentity(User user)
        {
            if (user != null)
            {
                var claims = new List<Claim>
                {
                    new Claim(JwtRegisteredClaimNames.UniqueName, user.Name),
                    new Claim(JwtRegisteredClaimNames.NameId, user.UserId)
                };
                ClaimsIdentity claimsIdentity =
                    new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType,
                        ClaimsIdentity.DefaultRoleClaimType);
                return claimsIdentity;
            }
            return null;
        }

        public static string GetUserId(HttpContext context)
        {
            object userIdObj;
            if (context.Items.TryGetValue("UserId", out userIdObj))
            {
                return userIdObj.ToString();
            }

            return string.Empty;
        }
    }
}
