using DICOMcloud.Extensions;
using DICOMcloud.IO;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System.Collections.Generic;

namespace DICOMcloud.Azure.IO
{
    public class AzureStorageService : MediaStorageService//, IEnumerable<IStorageContainer>
    {
        public AzureStorageService ( string connectionName )
        : this ( CloudStorageAccount.Parse( CloudConfigurationManager.GetSetting(connectionName) ) )
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

        protected override IEnumerable<IStorageContainer> GetContainers ( string containerKey ) 
        {
            containerKey = GetValidContainerKey ( containerKey );

            foreach ( var container in __CloudClient.ListBlobsSegmentedAsync(containerKey, ContainerListingDetails.None ) )
            {
                yield return GetContainer ( containerKey ) ;
            }
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
