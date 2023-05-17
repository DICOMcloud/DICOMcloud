using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DICOMcloud.IO
{
    public class MemoryStorageLocation : IStorageLocation
    {
        MemoryStream _location ;

        public bool EnableUploadHardCopy
        {
            get; set;
        }

        public MemoryStorageLocation ( string id, IMediaId mediaID ) 
        : this ( id, mediaID, new MemoryStream ( ) )
        {}

        public MemoryStorageLocation ( string id, IMediaId mediaID, MemoryStream stream ) 
        {
            ID                   = id ;
            MediaId              = mediaID ;
            EnableUploadHardCopy = true ;
            
            _location = stream ;
        }

        public string ContentType
        {
            get ; 
            private set ;
        }

        public string ID
        {
            get ;

            private set ;
        }

        public IMediaId MediaId
        {
            get ;

            private set ;
        }

        public long GetSize ( )
        {
            if ( Exists ( ) )
            {
                return _location.Length ;
            }

            return 0 ;
        }

        public string Metadata
        {
            get ;

            set ;
        }

        public string Name
        {
            get
            {
                throw new NotImplementedException ( );
            }
        }

        public void Delete ( )
        {
            _location = null ;
        }

        public Stream Download ( )
        {
            MemoryStream ms = new MemoryStream ( ) ;

            Download ( ms ) ;
            
            return ms ;
        }

        public void Download ( Stream stream )
        {
            _location.CopyTo ( stream ) ;
        }

        public bool Exists ( )
        {
            return _location != null ;
        }

        public Stream GetReadStream ( )
        {
            //throws exception http://stackoverflow.com/questions/1646193/why-does-memorystream-getbuffer-always-throw
            //return new MemoryStream ( _location.GetBuffer ( ), 0, (int) _location.Length, false ) ;

            return _location ;
        }

        public void Upload ( string filename, string contentType = null)
        {
            ContentType = contentType;
            _location = new MemoryStream ( File.ReadAllBytes ( filename ) ) ;
        }

        public void Upload ( byte[] buffer, string contentType = null)
        {
            ContentType = contentType;
            _location = new MemoryStream ( buffer ) ;
        }

        public void Upload ( Stream stream, string contentType = null)
        {
            ContentType = contentType;

            if ( !EnableUploadHardCopy && stream is MemoryStream )
            {
                _location = (MemoryStream) stream ;
            }
            else
            {
                stream.CopyTo ( _location ) ;

                _location.Position = 0 ;
            }
        }
    }
}
