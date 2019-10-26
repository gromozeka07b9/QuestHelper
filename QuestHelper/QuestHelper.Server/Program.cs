using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using System;

namespace QuestHelper.Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
            Console.WriteLine("QuestHelper server started");
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .Build();
    }
}
