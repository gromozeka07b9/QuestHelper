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
                //string dbLogin = System.Environment.ExpandEnvironmentVariables("%GoshDbLogin%");
                //string dbPassword = System.Environment.ExpandEnvironmentVariables("%GoshDbPassword%");
                string dbLogin = System.Environment.GetEnvironmentVariable("GoshDbLogin");
                string dbPassword = System.Environment.GetEnvironmentVariable("GoshDbPassword");
                Console.WriteLine("dbLogin:'" + dbLogin + "'");
                Console.WriteLine("dbPassword:'" + dbPassword + "'");
                if (string.IsNullOrEmpty(dbLogin) || string.IsNullOrEmpty(dbPassword))
                {
                    string errorMsg = "Error reading DB login or password!";
                    Console.WriteLine(errorMsg);
                    throw new Exception(errorMsg);
                }

                string connectionString = $@"Data Source=igosh.pro; Database=questhelper; User Id={dbLogin}; Password={dbPassword};";
                Console.WriteLine(connectionString);
                return new DbContextOptionsBuilder<ServerDbContext>().UseMySql(connectionString).Options;
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