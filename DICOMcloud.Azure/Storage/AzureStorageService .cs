using DICOMcloud.Extensions;
using DICOMcloud.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System.Collections.Generic;

namespace DICOMcloud.Azure.IO
{
    public class AzureStorageService : MediaStorageService
    {
        public AzureStorageService ( string connectionName, IConfiguration config )
        : this ( CloudStorageAccount.Parse( config.GetConnectionString(connectionName)))
        {}

        public AzureStorageService ( CloudStorageAccount storageAccount )
        {
            // Create a blob client for interacting with the blob service.
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            
            Init(blobClient);
        }

        protected override IStorageContainer GetContainer(string containerKey)
        {
            containerKey = GetValidContainerKey ( containerKey );

            CloudBlobContainer cloudContainer = __CloudClient.GetContainerReference ( containerKey );

            cloudContainer.CreateIfNotExistsAsync( ).Wait();

            return new AzureContainer ( cloudContainer );
        }

        protected override async IAsyncEnumerable<IStorageContainer> GetContainers ( string containerKey ) 
        {
            containerKey = GetValidContainerKey ( containerKey );
            BlobContinuationToken token = null ;

            do
            {
                var result = await __CloudClient.ListContainersSegmentedAsync(containerKey, ContainerListingDetails.None, null, token, new BlobRequestOptions(), null);
                
                token = result.ContinuationToken;

                foreach ( var container in result.Results)
                {
                    yield return GetContainer ( containerKey ) ;
                }

            } while (token != null);
        }

        private void Init(CloudBlobClient blobClient)
        {
            __CloudClient = blobClient;
            __KeyProvider = new AzureKeyProvider ( ) ;
        }

        protected override IKeyProvider GetKeyProvider()
        {
            return __KeyProvider ;
        }

        protected override bool ContainerExists ( string containerKey )
        {
            containerKey = GetValidContainerKey ( containerKey );

            CloudBlobContainer cloudContainer = __CloudClient.GetContainerReference ( containerKey ) ;

            return cloudContainer.ExistsAsync ( ).Result;
        }

        private static string GetValidContainerKey ( string containerKey )
        {
            containerKey = containerKey.ToLower ( );


            containerKey = containerKey.Replace ( __Separators, "a" );
            return containerKey;
        }

        private CloudBlobClient __CloudClient { get; set; }
        private IKeyProvider __KeyProvider { get; set; }

        private static char[] __Separators = "!@#$%^&*()+=[]{}\\|;':\",.<>/?~`".ToCharArray ( )  ;
    }
}
