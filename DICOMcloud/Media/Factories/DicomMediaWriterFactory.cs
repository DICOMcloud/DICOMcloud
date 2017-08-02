using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DICOMcloud.IO;

namespace DICOMcloud.Media
{
    public class DicomMediaWriterFactory : IDicomMediaWriterFactory
    {
        protected Func<string, IDicomMediaWriter> MediaFactory { get; private set; }
        protected IMediaStorageService StorageService { get; set ; }
        protected IDicomMediaIdFactory MediaIdFactory { get; set ; }


        public DicomMediaWriterFactory ( IMediaStorageService storageService, IDicomMediaIdFactory mediaIdFactory ) 
        {
            Init ( CreateDefualtWriters, storageService, mediaIdFactory ) ;
        }

        public DicomMediaWriterFactory 
        ( 
            Func<string, IDicomMediaWriter> mediaFactory, 
            IMediaStorageService storageService,
            IDicomMediaIdFactory mediaIdFactory 
        ) 
        {
            Init ( mediaFactory, storageService, mediaIdFactory ) ;
        }

        private void Init 
        ( 
            Func<string, IDicomMediaWriter> mediaFactory, 
            IMediaStorageService storageService,
            IDicomMediaIdFactory mediaIdFactory
        )
        {
            MediaFactory   = mediaFactory ;
            StorageService = storageService ;
            MediaIdFactory = mediaIdFactory ;
        }

        public virtual IDicomMediaWriter GetMediaWriter ( string mediaType )
        {
            try
            {
                IDicomMediaWriter writer = null ;
            
                writer = MediaFactory ( mediaType ) ;
            
                if ( null == writer )
                {
                    Trace.TraceInformation ( "Requested media writer not registered: " + mediaType ) ;
                }
                
                return writer ;
            }
            catch
            {
                return null ;
            }
        }
        
        protected virtual IDicomMediaWriter CreateDefualtWriters ( string mimeType  ) 
        {
            if ( mimeType == MimeMediaTypes.DICOM )
            {
                return new NativeMediaWriter ( StorageService, MediaIdFactory ) ;
            }

            return null ;
        }
    }
}
