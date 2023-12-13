using DICOMcloud.IO;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using System;
using System.IO;
using System.Net;
using System.Reflection.Metadata;

namespace DICOMcloud.Azure.IO
{
    public class AzureLocation : ObservableStorageLocation, IPreSignedUrlStorageLocation
    {
        private long? _size ;
        private IMediaId _mediaId;

        public AzureLocation ( BlobClient blob, IMediaId id = null )
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
                return Blob.GetProperties().Value.ContentType;
            } 
        }

        public override IMediaId MediaId { get { return _mediaId ; } }

        public override long GetSize ( )
        {
            if ( null != _size )
            {
                return _size.Value ;
            }
            else if ( Blob.Exists ( ) )
            {
                _size = Blob.GetProperties().Value.ContentLength;

                return _size.Value ;
            }
            else
            {
                //doesn't exist
                return 0 ;
            }
        }

        public virtual Uri GetReadUrl(DateTimeOffset expiryTime)
        {
            BlobSasBuilder saasBuilder = new BlobSasBuilder();

            saasBuilder.SetPermissions(BlobSasPermissions.Read);
            saasBuilder.ExpiresOn = expiryTime;
            saasBuilder.StartsOn = DateTimeOffset.UtcNow;
            return Blob.GenerateSasUri(saasBuilder);
        }

        public virtual Uri GetWriteUrl(DateTimeOffset expiryTime)
        {
            BlobSasBuilder saasBuilder = new BlobSasBuilder();

            saasBuilder.SetPermissions(BlobSasPermissions.Write);
            saasBuilder.ExpiresOn = expiryTime;
            saasBuilder.StartsOn = DateTimeOffset.UtcNow;
            return Blob.GenerateSasUri(saasBuilder);
        }

        public override string Metadata
        {
            get
            {
                return Blob.GetProperties().Value.Metadata["meta"];
            }

            set
            {
                var meta = Blob.GetProperties().Value.Metadata;

                meta["meta"] = value;
                Blob.SetMetadata(meta);
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
            Blob.DownloadTo(stream);
        }

        protected override void DoUpload(Stream stream, string contentType)
        {
            var options = new BlobUploadOptions()
            {
                HttpHeaders = new BlobHttpHeaders() { ContentType = contentType }
            };

            Blob.Upload(stream, options);
        }

        protected override void DoUpload ( byte[] buffer, string contentType)
        {
            var options = new BlobUploadOptions() 
            { 
                HttpHeaders = new BlobHttpHeaders() { ContentType = contentType } 
            };
            
            Blob.Upload(new BinaryData(buffer), options);
        }

        protected override void DoUpload(string filename, string contentType)
        {
            var options = new BlobUploadOptions()
            {
                HttpHeaders = new BlobHttpHeaders() { ContentType = contentType }
            };

            Blob.Upload(filename, options) ;
         }

        protected override Stream DoGetReadStream()
        {
            return Blob.OpenRead();
        }

        public BlobClient Blob
        {
            get; 
            set; 
        }
    }
}
