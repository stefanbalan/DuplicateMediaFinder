using System.IO;

namespace DuplicateMediaFinder.Interface
{

    interface IMetadataProvider<out T> : IProvider
    {
        T GetMetadata(FileSystemInfo item);
    }

}
