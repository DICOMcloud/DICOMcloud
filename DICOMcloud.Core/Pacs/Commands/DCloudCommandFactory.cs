using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DICOMcloud.IO;
using DICOMcloud.DataAccess;
using DICOMcloud.Media;

namespace DICOMcloud.Pacs.Commands
{
    public class DCloudCommandFactory : IDCloudCommandFactory
    {
        public IMediaStorageService      StorageService     { get; set; }
        public IObjectArchieveDataAccess DataAccess         { get; set; }
        public IDicomMediaWriterFactory  MediaWriterFactory { get; set; }
        public IDicomMediaIdFactory      MediaIdFactory     { get; set; }
        public StorageSettings           StorageSettings    { get; private set; }

        public DCloudCommandFactory
        (
            IMediaStorageService storageService,
            IObjectArchieveDataAccess dataAccess,
            IDicomMediaWriterFactory mediaWriterFactory,
            IDicomMediaIdFactory mediaIdFactory
        ) : this ( storageService, dataAccess, mediaWriterFactory, mediaIdFactory, new StorageSettings())
        {
        }

        public DCloudCommandFactory 
        ( 
            IMediaStorageService      storageService,
            IObjectArchieveDataAccess dataAccess,
            IDicomMediaWriterFactory  mediaWriterFactory,
            IDicomMediaIdFactory      mediaIdFactory,
            StorageSettings           storageSettings
        ) 
        {
            StorageService     = storageService     ;
            DataAccess         = dataAccess         ;
            MediaWriterFactory = mediaWriterFactory ;
            MediaIdFactory     = mediaIdFactory     ;
            StorageSettings    = storageSettings    ;
        }
        
        public virtual IStoreCommand CreateStoreCommand ( ) 
        {
            return new StoreCommand ( DataAccess, MediaWriterFactory, StorageSettings) ;    
        }

        public virtual IDeleteCommand CreateDeleteCommand ( ) 
        {
            return new DeleteCommand ( StorageService, DataAccess, MediaIdFactory ) ;
        }
    }
}
