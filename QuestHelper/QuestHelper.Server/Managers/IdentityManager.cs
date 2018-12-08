using QuestHelper.Server.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace QuestHelper.Server.Managers
{
    /// <summary>
    /// Чтение/запись Identity
    /// </summary>
    public class IdentityManager
    {
        private DbContextOptions<ServerDbContext> _dbOptions;
        public IdentityManager(DbContextOptions<ServerDbContext> dbOptions)
        {
            _dbOptions = dbOptions;
        }
        public ClaimsIdentity GetIdentity(string username, string password)
        {
            User user = new User();
            using (var _db = new ServerDbContext(_dbOptions))
            {
                user = _db.User.FirstOrDefault(x => x.Name == username && x.Password == password);
                if (user != null)
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimsIdentity.DefaultNameClaimType, user.Name),
                        new Claim(ClaimsIdentity.DefaultRoleClaimType, user.Role)
                    };
                    ClaimsIdentity claimsIdentity =
                        new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType,
                            ClaimsIdentity.DefaultRoleClaimType);
                    return claimsIdentity;
                }
            }
            return null;
        }
    }
}
