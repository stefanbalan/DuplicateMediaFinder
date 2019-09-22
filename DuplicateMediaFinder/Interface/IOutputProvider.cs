using System.Collections.Generic;
using System.IO;

namespace DuplicateMediaFinder.Interface
{
    interface IOutputProvider : IProvider
    {
        bool Add(FileSystemInfo item, IDictionary<string, IMetadata> metadatas);
    }
}
