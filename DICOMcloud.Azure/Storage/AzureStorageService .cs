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

        //        public IEnumerator<IStorageContainer> GetEnumerator()
        //        {
        //            foreach ( var container in __CloudClient.ListContainers() )
        //            {
        //                yield return new AzureContainer ( container ) ; 
        //            }
        //        }

        //        public override IStorageLocation GetTempLocation ( ) 
        //        {
        //            var container = CreateContainer (TempContainerName) ;

        //            return container.GetTempLocation ( ) ;    
        //        }

        //        public override string GetLogicalSeparator ( ) 
        //        {
        //            return "/" ;
        //        }

        protected override IStorageContainer GetContainer(string containerKey)
        {
            containerKey = GetValidContainerKey ( containerKey );

            CloudBlobContainer cloudContainer = __CloudClient.GetContainerReference ( containerKey );

            cloudContainer.CreateIfNotExists ( );

            return new AzureContainer ( cloudContainer );
        }

        protected override IEnumerable<IStorageContainer> GetContainers ( string containerKey ) 
        {
            containerKey = GetValidContainerKey ( containerKey );

            foreach ( var container in __CloudClient.ListContainers ( containerKey, ContainerListingDetails.None ) )
            {
                yield return GetContainer ( containerKey ) ;
            }
        }

        private void Init(CloudBlobClient blobClient)
        {
            __CloudClient = blobClient;
        }

        //        IEnumerator IEnumerable.GetEnumerator()
        //        {
        //            foreach ( var container in __CloudClient.ListContainers() )
        //            {
        //                yield return new AzureContainer ( container ) ; 
        //            }
        //        }

        protected override IKeyProvider CreateKeyProvider()
        {
            return new AzureKeyProvider ( ) ;
        }

        protected override bool ContainerExists ( string containerKey )
        {
            containerKey = GetValidContainerKey ( containerKey );

            CloudBlobContainer cloudContainer = __CloudClient.GetContainerReference ( containerKey ) ;

            return cloudContainer.Exists ( ) ;
        }

        private static string GetValidContainerKey ( string containerKey )
        {
            containerKey = containerKey.ToLower ( );


            containerKey = containerKey.Replace ( __Separators, "a" );
            return containerKey;
        }

        private CloudBlobClient __CloudClient { get; set; }

        private static char[] __Separators = "!@#$%^&*()+=[]{}\\|;':\",.<>/?~`".ToCharArray ( )  ;
    }
}
