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
        //public override IStorageLocation GetTempLocation ( ) 
        //{
        //    return new LocalStorageLocation ( Path.GetTempFileName () ) ;
        //}

        public FileStorageService ( ) 
        {
            BaseStorePath = ".//" ;
        }

        public FileStorageService ( string storePath ) 
        {
            BaseStorePath = storePath ;
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

        protected override IKeyProvider CreateKeyProvider ( ) 
        {
            return new FileKeyProvider ( ) ;
        }

        protected override IEnumerable<IStorageContainer> GetContainers ( string containerKey ) 
        {
            if ( !Directory.Exists ( BaseStorePath ) )
            {
                yield return null ;
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
    }
}
