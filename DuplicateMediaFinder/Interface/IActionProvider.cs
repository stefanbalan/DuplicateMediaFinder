using System.Threading.Tasks;

namespace DuplicateMediaFinder.Interface
{
    interface IActionProvider : IProvider
    {
        Task Perform();
    }
}
