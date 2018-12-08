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
        private HttpRequest _request;

        public ValidateUser(DbContextOptions<ServerDbContext> dbOptions, HttpRequest request)
        {
            _dbOptions = dbOptions;
            _request = request;
        }

        internal bool UserIsValid(string name)
        {
            JwtManager jwt = new JwtManager(_dbOptions);
            return jwt.TokenHashRequestAndDbAreEqual(name, BearerParser.GetTokenFromHeader(_request.Headers));
        }
    }
}
