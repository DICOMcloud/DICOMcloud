using System;
using System.Collections.Generic;
using DICOMcloud.IO;
using DICOMcloud;
using DICOMcloud.DataAccess;
using DICOMcloud.Media;
using DICOMcloud.Pacs.Commands;
using fo = Dicom;

namespace DICOMcloud.Pacs
{
    public class ObjectStoreService : IObjectStoreService
    {
        public IDCloudCommandFactory CommandFactory { get; set; }
        

        public ObjectStoreService 
        ( 
            IDCloudCommandFactory commandFactory
        )
        {
            CommandFactory = commandFactory ;
        }
        
        public DCloudCommandResult StoreDicom
        ( 
            fo.DicomDataset dataset,
            InstanceMetadata metadata
        )
        {
            IStoreCommand    storeCommand = CommandFactory.CreateStoreCommand ( ) ;
            StoreCommandData storeData    = new StoreCommandData ( ) { Dataset = dataset, Metadata = metadata } ;
            
            return storeCommand.Execute ( storeData ) ;
        }

        //TODO: update this to return a type showing what objects got deleted e.g. IObjectId[]
        //the "reuest" dataset is assumed to have the Object ID values. However, 
        //an extended implementation might send a query dataset and this method will query the DB and generate multiple Object IDs
        //Example: the request dataset has a date range, wild-card or SOP Class UID...
        public DCloudCommandResult Delete
        ( 
            fo.DicomDataset request,
            ObjectQueryLevel  level
        )
        {
            DCloudCommandResult deleteResult  = null ;
            IDeleteCommand     deleteCommand = CommandFactory.CreateDeleteCommand ( ) ;
            DeleteCommandData  deleteData    = new DeleteCommandData ( ) { Instances = new List<IObjectId> ( ) 
                                                                                        { DicomObjectIdFactory.Instance.CreateObjectId ( request ) }, 
                                                                          DeleteLevel = level } ;

            return deleteResult = deleteCommand.Execute ( deleteData ) ;
            
        }
    }
}
