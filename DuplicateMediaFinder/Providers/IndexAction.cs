using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DuplicateMediaFinder.Interface;

namespace DuplicateMediaFinder.Providers
{
    internal class IndexAction : IActionProvider
    {
        private readonly ISourceProvider sourceProvider;
        private readonly IEnumerable<IMetadataProvider> metadataProviders;
        private readonly IOutputProvider databaseProvider;

        public string Name => "index";

        public IndexAction(ISourceProvider sourceProvider, IEnumerable<IMetadataProvider> metadataProviders, IOutputProvider databaseProvider) 
        {
            this.sourceProvider = sourceProvider;
            this.metadataProviders = metadataProviders;
            this.databaseProvider = databaseProvider;
        }

        public async Task Perform()
        {
            while (sourceProvider.HasNext())
            {
                var item = await sourceProvider.GetNext();
                var metadatas = new ConcurrentDictionary<string, IMetadata>();

                metadataProviders.AsParallel().ForAll(async mp => metadatas.TryAdd(mp.Name, await mp.GetMetadata(item)));

                databaseProvider.Add(item, metadatas);
            }
        }
    }
}
