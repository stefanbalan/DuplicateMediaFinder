using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace DuplicateMediaFinder.Interface
{
    internal interface IMetadata
    {

    }

    internal interface IMetadataProvider : IProvider
    {
        Task<IMetadata> GetMetadata(FileSystemInfo item);
    }



    internal class LongMetadata : IMetadata
    {
        private long metadataValue;
        public LongMetadata(long val)
        {
            metadataValue = val;
        }
    }

    internal class StringMetadata : IMetadata
    {
        private string metadataValue;
        public StringMetadata(string val)
        {
            metadataValue = val;
        }
    }

    internal class FileNameMedatataProvider : IMetadataProvider
    {
        public string Name => "name";
        public async Task<IMetadata> GetMetadata(FileSystemInfo item)
        {
            return await Task.FromResult(new StringMetadata(item.Name));
        }
    }

    internal class FileSizeMedatataProvider : IMetadataProvider
    {
        public string Name => "filesize";
        public async Task<IMetadata> GetMetadata(FileSystemInfo item)
        {
            if (item is FileInfo file)
                return await Task.FromResult(new LongMetadata(file.Length));
            return await Task.FromResult(new LongMetadata(0));
        }
    }

    internal class Md5MetadataProvider : IMetadataProvider
    {
        public string Name => "md5";
        public Task<IMetadata> GetMetadata(FileSystemInfo item)
        {
            return Task.Factory.StartNew(() => (IMetadata)new StringMetadata(Hash_MD5_String((FileInfo)item)));
        }

        private readonly MD5 md5 = MD5.Create();

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

    }
}
