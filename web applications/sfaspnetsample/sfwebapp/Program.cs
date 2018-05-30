using System;
using System.Collections.Generic;
using System.Fabric;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace sfwebapp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //System.Threading.Tasks.Task.Delay(90000).Wait(); // Uncomment for Version2.0 of the web app
            //System.Threading.Tasks.Task.Delay(600000).Wait(); // Uncomment for Version3.0 of the web app
            BuildWebHost(args).Run(); //Version1.0
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .Build();
    }
}
