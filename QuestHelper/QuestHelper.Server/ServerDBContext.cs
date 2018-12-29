using System;
using Microsoft.EntityFrameworkCore;
using QuestHelper.Server.Models;

namespace QuestHelper.Server
{
    public class ServerDbContext : DbContext
    {
        public DbSet<Route> Route { get; set; }
        public DbSet<RoutePoint> RoutePoint { get; set; }
        public DbSet<RouteAccess> RouteAccess { get; set; }
        public DbSet<RoutePointMediaObject> RoutePointMediaObject { get; set; }
        public DbSet<User> User { get; set; }

        public static DbContextOptions<ServerDbContext> GetOptionsContextDbServer(bool isFake = false)
        {
            if (isFake)
            {
                return new DbContextOptionsBuilder<ServerDbContext>().UseInMemoryDatabase(databaseName: "for unit test").Options;
            }
            else
            {
                string dbLogin = System.Environment.ExpandEnvironmentVariables("%GoshDbLogin%");
                string dbPassword = System.Environment.ExpandEnvironmentVariables("%GoshDbPassword%");
                if (string.IsNullOrEmpty(dbLogin) || string.IsNullOrEmpty(dbPassword))
                {
                    throw new Exception("Error reading DB login or password!");
                }
                return new DbContextOptionsBuilder<ServerDbContext>().UseMySql($@"Data Source=dbquesthelper.mysql.database.azure.com; Database=QuestHelper; User Id={dbLogin}; Password={dbPassword};").Options;
            }
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        }

        public ServerDbContext(DbContextOptions<ServerDbContext> options) : base(options)
        {

        }
    }
}