using System;
using System.IO;

namespace DuplicateMediaFinder
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var x = new FileSystemSource(new DirectoryInfo(@"\\standbynas\photo"));

        }
    }
}
