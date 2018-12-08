using Microsoft.EntityFrameworkCore;
using QuestHelper.Server.Models;

namespace QuestHelper.Server
{
    public class ServerDbContext : DbContext
    {
        public DbSet<Route> Route { get; set; }
        public DbSet<RoutePoint> RoutePoint { get; set; }
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
                return new DbContextOptionsBuilder<ServerDbContext>().UseMySql(@"Data Source=dbquesthelper.mysql.database.azure.com; Database=QuestHelper; User Id=sa@dbquesthelper; Password=Klim2002;").Options;
            }
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            /*if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseMySql(@"Data Source=dbquesthelper.mysql.database.azure.com; Database=QuestHelper; User Id=sa@dbquesthelper; Password=Klim2002;");
            }*/
        }

        public ServerDbContext()
        {

        }
        public ServerDbContext(DbContextOptions<ServerDbContext> options) : base(options)
        {

        }
    }
}