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
        public IMediaStorageService            StorageService     { get; set; }
        public IObjectStorageDataAccess DataAccess         { get; set; }
        public IDicomMediaWriterFactory        MediaWriterFactory { get; set; }
        public IDicomMediaIdFactory            MediaIdFactory     { get; set; }
        

        public DCloudCommandFactory 
        ( 
            IMediaStorageService            storageService,
            IObjectStorageDataAccess dataAccess,
            IDicomMediaWriterFactory        mediaWriterFactory,
            IDicomMediaIdFactory            mediaIdFactory
        ) 
        {
            StorageService     = storageService     ;
            DataAccess         = dataAccess         ;
            MediaWriterFactory = mediaWriterFactory ;
            MediaIdFactory     = mediaIdFactory     ;
        }
        
        public virtual IStoreCommand CreateStoreCommand ( ) 
        {
            return new StoreCommand ( DataAccess, MediaWriterFactory ) ;    
        }

        public virtual IDeleteCommand CreateDeleteCommand ( ) 
        {
            return new DeleteCommand ( StorageService, DataAccess, MediaIdFactory ) ;
        }
    }
}
