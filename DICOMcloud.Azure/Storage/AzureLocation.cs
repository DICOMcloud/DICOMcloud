using DICOMcloud.IO;
using Microsoft.WindowsAzure.Storage.Blob;
using System.IO;

namespace DICOMcloud.Azure.IO
{
    public class AzureLocation : ObservableStorageLocation
    {
        private long? _size ;
        private IMediaId _mediaId;

        public AzureLocation ( ICloudBlob blob, IMediaId id = null )
        {
            Blob  = blob;
            _mediaId = id;        
        }

        public override string Name
        {
            get {  return Blob.Name ; }
        }

        public override string ID
        {
            get {  return Blob.Uri.AbsolutePath ; }
        }

        public override bool Exists()
        {
            return Blob.Exists ( ) ;
        }

        public override string ContentType 
        { 
            get
            {
                return Blob.Properties.ContentType ;
            } 
        }

        public override IMediaId MediaId { get { return _mediaId ; } }

        public override long Size
        {
            get
            {
                if ( null != _size )
                {
                    return _size.Value ;
                }
                else if ( Blob.Exists ( ) )
                {

                    Blob.FetchAttributes ( ) ;

                    _size = Blob.Properties.Length ;

                    return _size.Value ;
                }

                //doesn't exist
                return 0 ;
            }
        }

        public override string Metadata
        {
            get
            {
                return Blob.Metadata["meta"] ;
            }

            set
            {
                Blob.Metadata["meta"] = value ;
            }
        }

        protected override void DoDelete()
        {
            Blob.Delete ();
        }

        protected override Stream DoDownload()
        {
            return Blob.OpenRead();
        }

        protected override void DoDownload(Stream stream)
        {
            Blob.DownloadToStream ( stream ) ;
        }

        protected override void DoUpload(Stream stream)
        {
            Blob.Properties.ContentType = ContentType ;
            Blob.UploadFromStream (stream);
            
            WriteMetadata ( ) ;
        }

        protected override void DoUpload ( byte[] buffer )
        {
            Blob.UploadFromByteArray ( buffer, 0, buffer.Length ) ;
            WriteMetadata ( ) ;
        }

        protected override void DoUpload(string filename)
        {
            Blob.UploadFromFile (filename ) ;
            WriteMetadata( ) ;
         }

        protected override Stream DoGetReadStream()
        {
            return Blob.OpenRead ( ) ;
        }

        private void WriteMetadata ( )
        {
            Blob.SetMetadata ( ) ;
            //__Blob.SetProperties ( ) ;
        }

        public ICloudBlob Blob
        {
            get; 
            set; 
        }
    }
}
