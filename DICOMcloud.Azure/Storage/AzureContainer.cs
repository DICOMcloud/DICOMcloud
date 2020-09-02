using DICOMcloud.IO;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

        public void Delete()
        {
            // Todo: This is dirty but is a temp fix till a decision is made with this.
            __Container.DeleteIfExistsAsync( ).Wait() ;
        }

        public IStorageLocation GetLocation(string key = null, IMediaId id = null )
        {
            var blob = __Container.GetBlockBlobReference ( (key == null ) ? Guid.NewGuid().ToString() : key ) ;
            
            return new AzureLocation ( blob, id ) ;
        }

        //public IEnumerable<IStorageLocation> GetLocations (string key )
        //{
        //    // Todo: This is dirty but is a temp fix till a decision is made with this.
        //    var blobs = __Container.ListBlobs(key, true, BlobListingDetails.None).OfType<CloudBlockBlob>();
        //    foreach (var blob in blobs)
        //    {
        //        yield return new AzureLocation(blob);
        //    }
        //}

        public async IAsyncEnumerable<IStorageLocation> GetLocations(string key)
        {
            BlobContinuationToken token = null;

            do
            {
                var result = await __Container.ListBlobsSegmentedAsync(key,
                                                                        true,
                                                                        BlobListingDetails.None,
                                                                        null,
                                                                        token,
                                                                        new BlobRequestOptions(),
                                                                        null);

                var blobs = result.Results.OfType<CloudBlockBlob>();

                token = result.ContinuationToken;

                foreach (var blob in blobs)
                {
                    yield return new AzureLocation(blob);
                }
            } while (token != null);
        }

        public bool LocationExists ( string key )
        {
            var blob = __Container.GetBlockBlobReference ( key ) ;
            return blob.ExistsAsync().Result;
        }
    }
}
