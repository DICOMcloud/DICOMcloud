using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DICOMcloud.Media
{
    public class DicomMediaReaderFactory : IDicomMediaReaderFactory
    {
        protected Func<string, IDicomMediaReader> MediaFactory { get; private set; }

        public DicomMediaReaderFactory ( ) 
        {
            Init ( CreateDefualtReaders ) ;
        }

        public DicomMediaReaderFactory ( Func<string, IDicomMediaReader> mediaFactory ) 
        {
            Init ( mediaFactory ) ;
        }

        public virtual IDicomMediaReader GetMediaReader ( string mimeType )
        {
            try
            {
                IDicomMediaReader reader ;
            
                
                reader = MediaFactory ( mimeType ) ;
            
                if ( null == reader )
                {
                    Trace.TraceInformation ( "Requested media reader not registered: " + mimeType ) ;
                }
                
                return reader ;
            }
            catch
            {
                return null ;
            }
        }
        
        protected virtual IDicomMediaReader CreateDefualtReaders ( string mimeType ) 
        {
            //if ( mimeType == MimeMediaTypes.UncompressedData )
            //{
            //    return new NativeMediaReader ( ) ;
            //}

            return null ;
        }
    
        private void Init ( Func<string, IDicomMediaReader> mediaFactory )
        {
            MediaFactory = mediaFactory ;
        }    
    }
}
