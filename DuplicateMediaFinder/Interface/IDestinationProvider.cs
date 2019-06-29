using System.IO;
using System.Threading.Tasks;

namespace DuplicateMediaFinder.Interface
{
    internal interface IDestinationProvider : IProvider
    {
        Task Save(FileSystemInfo file);

        bool HasError();
        string Error();
    }
}
