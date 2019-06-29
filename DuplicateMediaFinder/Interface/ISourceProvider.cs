using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace DuplicateMediaFinder.Interface
{
    internal interface ISourceProvider : IProvider
    {
        Task<FileSystemInfo> GetNext();
        bool HasNext();
        bool HasError();
        List<string> Errors();
    }
}
