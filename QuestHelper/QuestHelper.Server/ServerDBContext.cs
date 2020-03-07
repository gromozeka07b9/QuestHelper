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
        public DbSet<RouteShare> RouteShare { get; set; }

        /// <summary>
        /// Таблица с POI
        /// </summary>
        public DbSet<Poi> Poi { get; set; }

        /// <summary>
        /// Таблица с лайками маршрутов
        /// </summary>
        public DbSet<RouteLike> RouteLike { get; set; }
        
        public DbSet<FeedItem> FeedItem { get; set; }

        /// <summary>
        /// Просмотры маршрутов
        /// </summary>
        public DbSet<RouteView> RouteView { get; set; }
        public DbSet<User> User { get; set; }
        public DbSet<OauthUser> OauthUser { get; set; }

        public static DbContextOptions<ServerDbContext> GetOptionsContextDbServer(bool isFake = false)
        {
            if (isFake)
            {
                return new DbContextOptionsBuilder<ServerDbContext>().UseInMemoryDatabase(databaseName: "for unit test").Options;
            }
            else
            {
                string dbLogin = System.Environment.GetEnvironmentVariable("GoshDbLogin");
                string dbPassword = System.Environment.GetEnvironmentVariable("GoshDbPassword");
                if (string.IsNullOrEmpty(dbLogin) || string.IsNullOrEmpty(dbPassword))
                {
                    string errorMsg = "Error reading DB login or password!";
                    Console.WriteLine(errorMsg);
                    throw new Exception(errorMsg);
                }

                string connectionString = $@"Data Source=igosh.pro; Database=questhelper; User Id={dbLogin}; Password={dbPassword};";
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