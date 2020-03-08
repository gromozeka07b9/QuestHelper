using Microsoft.EntityFrameworkCore;
using QuestHelper.Server.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuestHelper.Server.Managers
{
    public class PoiManager
    {
        DbContextOptions<ServerDbContext> _db;

        public PoiManager(DbContextOptions<ServerDbContext> db)
        {
            _db = db;
        }

    }
}
