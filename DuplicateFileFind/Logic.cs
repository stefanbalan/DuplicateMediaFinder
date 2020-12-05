using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace DuplicateFileFind
{
    internal class Logic
    {
        private readonly DffContext db = new DffContext();
        private readonly MD5 md5;
        readonly string[] extensions = { ".jpg", ".jpeg", ".mp4" };

        public Logic()
        {
            md5 = MD5.Create();
            db.Database.EnsureCreated();
        }

        public async Task AddReference(DirectoryInfo refDirectory, bool recurse = false)
        {

            // switched to using "manual" recursion (see end of function)
            //var enumerateFiles = refDirectory.EnumerateFiles("*.*", recurse ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly).ToList();
            var enumerateFiles = refDirectory.EnumerateFiles("*.*", SearchOption.TopDirectoryOnly).ToList();
            
            var imageFiles = enumerateFiles.Where(fi => extensions.Contains(fi.Extension.ToLowerInvariant())).ToList();

            var a = enumerateFiles.Count;
            var c = imageFiles.Count;

            var unsaved = 0;

            foreach (var fileInfo in imageFiles)
            {
                try
                {
                    var directory = await db.Directories.FirstOrDefaultAsync(dir => dir.Path == fileInfo.DirectoryName);
                    if (directory is null)
                    {
                        directory = new Directory { Path = fileInfo.DirectoryName };
                        await db.Directories.AddAsync(directory);
                        await db.SaveChangesAsync();
                    }

                    var file = db.Files.FirstOrDefault(f => f.DirectoryId == directory.Id && f.Name == fileInfo.Name && f.Length == fileInfo.Length);
                    if (!(file is null))
                    {
                        //Console.WriteLine($"{fileInfo.FullName} already indexed");
                        Console.WriteLine(".");
                        continue;
                    }


                    var hashString = Hash_MD5_String(fileInfo);

                    file = new File
                    {
                        Name = fileInfo.Name,
                        Directory = directory,
                        CreationTime = fileInfo.CreationTime,
                        Length = fileInfo.Length,
                        Hash = hashString
                    };

                    db.Files.Add(file);
                    if (unsaved++ >= 100)
                    {
                        unsaved = 0;
                        await db.SaveChangesAsync();
                    }
                    Console.WriteLine($"{fileInfo.FullName}, {fileInfo.Length}, " + $"{fileInfo.CreationTime}, {hashString}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }

            await db.SaveChangesAsync();

            if (!recurse) return;

            var enumerateDirs = refDirectory.EnumerateDirectories("*.*", SearchOption.TopDirectoryOnly);
            foreach (var di in enumerateDirs)
            {
                await AddReference(di, true);
            }
        }

        public async Task PersistFileData(FileInfo fi)
        {

        }

        private string Hash_MD5_String(FileInfo fileInfo)
        {
            byte[] hash;
            using (var stream = fileInfo.OpenRead())
            {
                hash = md5.ComputeHash(stream);
            }

            var hashString = hash.Aggregate("", (s, b) => s + b.ToString("X2"));
            return hashString;
        }

        public async Task List(bool directories)
        {
            var dirs = await db.Directories.ToListAsync();
            foreach (var di in dirs)
            {
                Console.WriteLine(di.Path);
            }
        }

        public async Task Delete(DirectoryInfo delDirectory, bool recurse)
        {
            var enumerateFiles = delDirectory.EnumerateFiles("*.*", recurse ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
            var imageFiles = enumerateFiles.Where(fi => extensions.Contains(fi.Extension));

            foreach (var deleteFile in imageFiles)
            {
                var sameSeizeFiles = await db.Files.Where(f => f.Length == deleteFile.Length).ToListAsync();
                if (sameSeizeFiles.Count == 0) continue;

                var hash = Hash_MD5_String(deleteFile);
                var existingFile = sameSeizeFiles.FirstOrDefault(f => string.Equals(f.Hash, hash, StringComparison.InvariantCultureIgnoreCase));
                if (existingFile is null) continue;

                // delete action
                if (existingFile.Directory.Path == deleteFile.DirectoryName)
                    deleteFile.Delete();
                Console.WriteLine($"Deleted: {deleteFile.Name} - {existingFile.Name}");
            }
        }
    }

}
