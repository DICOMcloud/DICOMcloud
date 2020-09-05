using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DICOMcloud.IO
{
    public class FileStorageService : MediaStorageService 
    {
        public FileStorageService ( ) 
        : this ( ".//" )
        {
        }

        public FileStorageService ( string storePath ) 
        : this ( storePath, new FileKeyProvider ( ) )
        {
        }

        public FileStorageService ( string storePath, IKeyProvider keyProvider ) 
        {
            BaseStorePath = storePath ;
            __KeyProvider = keyProvider  ;
        }

        protected override IStorageContainer GetContainer ( string containerKey ) 
        {
            LocalStorageContainer storage = new LocalStorageContainer ( GetStoragePath (containerKey ) ) ;

            if ( !Directory.Exists ( storage.FolderPath ))
            {
                Directory.CreateDirectory ( storage.FolderPath )  ;
            }

            return storage ;
        }

        protected override IKeyProvider GetKeyProvider ( ) 
        {
            return __KeyProvider ;
        }

        protected override async IAsyncEnumerable<IStorageContainer> GetContainers ( string containerKey ) 
        {
            if ( !Directory.Exists (Path.Combine(BaseStorePath,containerKey)))
            {
                yield break;
            }

            foreach ( string folder in Directory.EnumerateDirectories ( BaseStorePath, containerKey, SearchOption.TopDirectoryOnly ))
            {
                yield return GetContainer ( folder ) ;
            }
        }

        protected virtual string GetStoragePath ( string folderName )
        {
            return Path.Combine (BaseStorePath, folderName ) ; 
        }

        protected override bool ContainerExists ( string containerKey )
        {
            LocalStorageContainer storage = new LocalStorageContainer ( GetStoragePath ( containerKey ) ) ;

            return Directory.Exists ( storage.FolderPath ) ;
        }

        public string BaseStorePath { get; set; }

        private IKeyProvider __KeyProvider { get; set; }
    }
}
