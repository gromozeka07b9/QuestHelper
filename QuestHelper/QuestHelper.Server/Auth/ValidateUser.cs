using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using QuestHelper.Server.Managers;
using QuestHelper.Server.Models;

namespace QuestHelper.Server.Auth
{
    public class ValidateUser
    {
        private DbContextOptions<ServerDbContext> _dbOptions;
        private string _token;

        public ValidateUser(DbContextOptions<ServerDbContext> dbOptions, string token)
        {
            _dbOptions = dbOptions;
            _token = token;
        }

        public bool UserIsValid(string name)
        {
            JwtManager jwt = new JwtManager();

            if (!string.IsNullOrEmpty(_token) && !string.IsNullOrEmpty(name))
            {
                string userKey = jwt.GetUserKeyFromToken(_token);
                try
                {
                    using (var context = new ServerDbContext(_dbOptions))
                    {
                        var result = context.User.Where(x => x.TokenKey == userKey && x.Name == name);
                        return result.Count() > 0;
                    }
                }
                catch (Exception e)
                {
                    throw new Exception("Check user error", e);
                }
            }

            return false;
        }
    }
}
