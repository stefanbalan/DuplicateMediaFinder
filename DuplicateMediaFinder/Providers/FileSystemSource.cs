using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Threading.Tasks;
using DuplicateMediaFinder.Interface;

namespace DuplicateMediaFinder.Providers
{
    internal class FileSystemSource : ISourceProvider
    {
        public string Name => "filesource";

        private readonly ConcurrentQueue<FileSystemInfo> items = new ConcurrentQueue<FileSystemInfo>();
        private readonly Task scanTask;
        private readonly List<string> errors = new List<string>();

        public FileSystemSource(DirectoryInfo sourceDirectoryInfo)
        {
            scanTask = Task.Factory
                .StartNew(() => EnqueItems(sourceDirectoryInfo));
        }

        private void EnqueItems(DirectoryInfo sourceDirectoryInfo)
        {
            try
            {
                var files = sourceDirectoryInfo.EnumerateFiles().ToList();

                files.ForEach(items.Enqueue);

                var dirs = sourceDirectoryInfo.EnumerateDirectories().ToList();


                dirs.ForEach(di =>
                {
                    EnqueItems(di);
                    items.Enqueue(di);
                });
            }
            catch (DirectoryNotFoundException e)
            {
                var error = $"DirectoryNotFound : {sourceDirectoryInfo.Name}";
                Log.Error(error + e.Message);
                errors.Add(error);
            }
            catch (SecurityException e)
            {
                var error = $"DirectoryNotAccessible : {sourceDirectoryInfo.Name}";
                Log.Error(error + e.Message);
                errors.Add(error);
            }
        }


        public async Task<FileSystemInfo> GetNext()
        {
            while (items.IsEmpty &&
                   !(scanTask.IsCompleted || scanTask.IsCanceled || scanTask.IsFaulted))
                await Task.Delay(100);

            if (items.IsEmpty)
                return null;

            return items.TryDequeue(out var result)
                ? result
                : null;
        }

        public bool HasNext()
        {
            return scanTask.IsCompleted || scanTask.IsCanceled || scanTask.IsFaulted;
        }

        public bool HasError()
        {
            return scanTask.IsFaulted;
        }

        public List<string> Errors()
        {
            return errors;
        }
    }
}
