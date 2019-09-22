using DuplicateMediaFinder.Interface;
using DuplicateMediaFinder.Providers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DuplicateMediaFinder
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            //setup our DI
            var serviceProvider = new ServiceCollection()
                .AddLogging()

                .AddOptions<AppConfiguration>().Services

                .AddSingleton<ISourceProvider, FileSystemSource>()
                .AddSingleton<IMetadataProvider, FileNameMedatataProvider>()
                .AddSingleton<IMetadataProvider, FileSizeMedatataProvider>()
                .AddSingleton<IMetadataProvider, Md5MetadataProvider>()

                .BuildServiceProvider();

            //configure console logging
            serviceProvider
                .GetService<ILoggerFactory>()
                .AddConsole(LogLevel.Debug);

            var logger = serviceProvider.GetService<ILoggerFactory>()
                .CreateLogger<Program>();
            logger.LogDebug("Starting application");

            ////do the actual work here
            //var bar = serviceProvider.GetService<IBarService>();
            //bar.DoSomeRealWork();

            logger.LogDebug("All done!");

        }
    }
}
