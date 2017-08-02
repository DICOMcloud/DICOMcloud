using DICOMcloud.DataAccess;

namespace DICOMcloud.Pacs.Commands
{
    public abstract class DCloudCommand<T,R> : IDCloudCommand<T,R>
    {
        public IObjectStorageDataAccess DataAccess   { get; set; }
        
        public DCloudCommand ( ) : this ( null ) 
        {}

        public DCloudCommand
        ( 
            IObjectStorageDataAccess dataStorage
            //, 
//            IDicomMediaWriterFactory mediaFactory
        )
        {
            DataAccess   = dataStorage ;
            //MediaFactory = mediaFactory ; //?? new DicomMediaWriterFactory ( ) ;
        }

        public abstract R Execute(T commandData ) ;
    }
}
