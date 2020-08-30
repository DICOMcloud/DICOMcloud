using DICOMcloud.Messaging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;
using System;

namespace DICOMcloud.Azure.Messaging
{
    public class AzureMessageSender : IMessageSender
    {
        public CloudStorageAccount StorageAccount { get; set; }
        
        public AzureMessageSender ( CloudStorageAccount storageAccount )
        {
            StorageAccount = storageAccount ;
        }

        public void SendMessage ( ITransportMessage message, TimeSpan? delay = default (TimeSpan?) ) 
        {
            var client = StorageAccount.CreateCloudQueueClient ( ) ;
        
            var queue = client.GetQueueReference ( message.Name ) ;    
        
            queue.CreateIfNotExistsAsync ( ).Wait() ;
            
            queue.AddMessageAsync( new CloudQueueMessage(JsonConvert.SerializeObject(message) ),
                               null, delay, null, new OperationContext ( ) { ClientRequestID = message.ID} );    
        }
    }
}
