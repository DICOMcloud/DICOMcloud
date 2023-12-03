using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using DICOMcloud.IO;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DICOMcloud.Azure.IO
{
    public class AzureContainer : IStorageContainer
    {
        private BlobContainerClient __Container { get; set; }
        public string ConnectionString { get; private set; }

        public AzureContainer ( BlobContainerClient container, string connectionString )
        {
            __Container      = container ;
            ConnectionString = connectionString;
        }

        public string Connection
        {
            get
            {
                return __Container.Uri.ToString();
            }
        }

        public void Delete()
        {
            __Container.DeleteIfExists ( ) ;
        }

        public IStorageLocation GetLocation(string key = null, IMediaId id = null )
        {
            var blob = __Container.GetBlobClient((key == null) ? Guid.NewGuid().ToString() : key);
            
            return new AzureLocation ( blob, id ) ;
        }

        public IEnumerable<IStorageLocation> GetLocations (string key )
        {
            foreach (var blob in __Container.GetBlobs(BlobTraits.None, BlobStates.None, key))
            {
                BlobClient blobClient = new BlobClient(ConnectionString, __Container.Name, blob.Name);
                yield return new AzureLocation(blobClient);
            }
        }

        public bool LocationExists ( string key )
        {
            return __Container.GetBlobClient(key).Exists();
        }
    }
}
