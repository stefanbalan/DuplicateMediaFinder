using System;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace CopyNew
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");


            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            var configuration = builder.Build();

            var config = configuration.GetSection("Config");

            var children = config.GetChildren();
        }


    }
}
