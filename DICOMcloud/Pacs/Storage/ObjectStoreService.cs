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
        
        public StoreResult StoreDicom
        ( 
            fo.DicomDataset dataset,
            InstanceMetadata metadata
        )
        {
            IStoreCommand    storeCommand = CommandFactory.CreateStoreCommand ( ) ;
            StoreCommandData storeData    = new StoreCommandData ( ) { Dataset = dataset, Metadata = metadata } ;
            StoreResult      storeResult  = new StoreResult ( ) ;

            try
            {
                //currently not used
                StoreCommandResult result = new StoreCommandResult ( ) ;


                result = storeCommand.Execute ( storeData ) ;

                storeResult.DataSet = dataset ;
                storeResult.Status  = CommandStatus.Success ;
            }
            catch ( Exception ex )
            {
                string errorMsg = "Error storing object" ;
                System.Diagnostics.Trace.Fail ( errorMsg, ex.ToString ( ) );
                storeResult.Status = CommandStatus.Failed ;

                //TODO: must catch specific exception types and set status, message and "code" accoringely
                storeResult.DataSet = dataset ;
                storeResult.Status  = CommandStatus.Failed ;
                storeResult.Error   = ex ;
                storeResult.Message = errorMsg ;
            }
            
            return storeResult ;
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
            DeleteCommandData  deleteData    = new DeleteCommandData ( ) { Instances = new List<ObjectId> ( ) 
                                                                                        { new ObjectId ( request ) }, 
                                                                          DeleteLevel = level } ;

            try
            {
                deleteResult = deleteCommand.Execute ( deleteData ) ;
            }
            catch ( Exception ex )
            {
                //System.Diagnostics.Trace.Fail ( "Error deleting object", ex.ToString ( ) );
                deleteResult.Status = CommandStatus.Failed;

                //TODO: must catch specific exception types and set status, message and "code" accoringely
                //storeResult.DataSet = dataset;
                deleteResult.Status  = CommandStatus.Failed ;
                deleteResult.Error   = ex ;
                deleteResult.Message = ex.Message ;
            }

            return deleteResult ;
        }
    }
}
