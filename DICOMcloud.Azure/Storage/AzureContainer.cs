using DICOMcloud.IO;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DICOMcloud.Azure.IO
{
    public class AzureContainer : IStorageContainer
    {
        private CloudBlobContainer __Container { get; set; }
        
        public AzureContainer ( CloudBlobContainer container )
        {
            __Container = container ;
        }

        public string Connection
        {
            get
            {
                return __Container.Uri.ToString();
            }
        }

        public async Task DeleteAsync()
        {
            await __Container.DeleteIfExistsAsync ( ) ;
        }

        public IStorageLocation GetLocation(string key = null, IMediaId id = null )
        {
            var blob = __Container.GetBlockBlobReference ( (key == null ) ? Guid.NewGuid().ToString() : key ) ;
            
            return new AzureLocation ( blob, id ) ;
        }

        public IEnumerable<IStorageLocation> GetLocations (string key )
        {
            foreach (var blob in __Container.ListBlobs(key, true, BlobListingDetails.None).OfType<CloudBlockBlob>())
            {
                yield return new AzureLocation(blob);
            }
        }

        public bool LocationExists ( string key )
        {
            var blob = __Container.GetBlockBlobReference ( key ) ;

            return blob.Exists ( ) ;
        }
    }
}
