using System.IO;

namespace DuplicateMediaFinder.Interface
{
    interface IDatabaseProvider : IProvider
    {
        bool Add(FileSystemInfo item);

    }
}
