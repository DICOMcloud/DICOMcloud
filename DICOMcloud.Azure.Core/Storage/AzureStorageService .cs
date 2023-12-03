using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using DICOMcloud.Extensions;
using DICOMcloud.IO;

using System.Collections.Generic;

namespace DICOMcloud.Azure.IO
{
    public class AzureStorageService : MediaStorageService//, IEnumerable<IStorageContainer>
    {
        public AzureStorageService (string connectionString )
        {
            __ConnectionString = connectionString;
            __CloudClient = new BlobServiceClient(connectionString);
            __KeyProvider = new AzureKeyProvider();
        }

        protected override IStorageContainer GetContainer(string containerKey)
        {
            containerKey = GetValidContainerKey ( containerKey );

            var cloudContainer = __CloudClient.GetBlobContainerClient ( containerKey );

            cloudContainer.CreateIfNotExists ( );

            return new AzureContainer ( cloudContainer, __ConnectionString);
        }

        protected override IEnumerable<IStorageContainer> GetContainers ( string parentContainerKey ) 
        {
            parentContainerKey = GetValidContainerKey ( parentContainerKey );

            foreach ( var container in __CloudClient.GetBlobContainers(BlobContainerTraits.None, BlobContainerStates.None, parentContainerKey))
            {
                yield return GetContainer (container.Name) ;
            }
        }

        protected override IKeyProvider GetKeyProvider()
        {
            return __KeyProvider ;
        }

        protected override bool ContainerExists ( string containerKey )
        {
            containerKey = GetValidContainerKey ( containerKey );

            var cloudContainer = __CloudClient.GetBlobContainerClient ( containerKey ) ;

            return cloudContainer.Exists ( ) ;
        }

        private static string GetValidContainerKey ( string containerKey )
        {
            containerKey = containerKey.ToLower ( );


            containerKey = containerKey.Replace ( __Separators, "a" );
            return containerKey;
        }

        private string __ConnectionString;

        private BlobServiceClient __CloudClient { get; set; }
        private IKeyProvider      __KeyProvider { get; set; }

        private static char[] __Separators = "!@#$%^&*()+=[]{}\\|;':\",.<>/?~`".ToCharArray ( )  ;
    }
}
