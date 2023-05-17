﻿using DICOMcloud.IO;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.IO;
using System.Net;

namespace DICOMcloud.Azure.IO
{
    public class AzureLocation : ObservableStorageLocation, IPreSignedUrlStorageLocation
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

        public override long GetSize ( )
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
            else
            {
                //doesn't exist
                return 0 ;
            }
        }

        public virtual Uri GetReadUrl(DateTimeOffset? startTime, DateTimeOffset? expiryTime)
        {
            var sasToken = Blob.GetSharedAccessSignature(new SharedAccessBlobPolicy()
                {
                    Permissions = SharedAccessBlobPermissions.Read,
                    SharedAccessStartTime = startTime,
                    SharedAccessExpiryTime = expiryTime
                });
            
            var blobUrl = string.Format("{0}{1}", Blob.Uri.AbsoluteUri, sasToken);

            return new Uri(blobUrl);
        }

        public virtual Uri GetWriteUrl(DateTimeOffset? startTime, DateTimeOffset? expiryTime)
        {
            var sasToken = Blob.GetSharedAccessSignature(new SharedAccessBlobPolicy()
            {
                Permissions = SharedAccessBlobPermissions.Write,
                SharedAccessStartTime = startTime,
                SharedAccessExpiryTime = expiryTime
            });

            var blobUrl = string.Format("{0}{1}", Blob.Uri.AbsoluteUri, sasToken);

            return new Uri(blobUrl);
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
            try
            {
                Blob.Delete ();
            }
            catch ( Microsoft.WindowsAzure.Storage.StorageException ex )
            {
                //if blob doesn't exist for any reason then it is already deleted.
                if ( ex.RequestInformation.HttpStatusCode != (int) HttpStatusCode.NotFound )
                {
                    throw ;
                }
            }
        }

        protected override Stream DoDownload()
        {
            return Blob.OpenRead();
        }

        protected override void DoDownload(Stream stream)
        {
            Blob.DownloadToStream ( stream ) ;
        }

        protected override void DoUpload(Stream stream, string contentType)
        {
            Blob.Properties.ContentType = contentType;
            Blob.UploadFromStream (stream);
            
            WriteMetadata ( ) ;
        }

        protected override void DoUpload ( byte[] buffer, string contentType)
        {
            Blob.Properties.ContentType = contentType;
            Blob.UploadFromByteArray ( buffer, 0, buffer.Length ) ;
            WriteMetadata ( ) ;
        }

        protected override void DoUpload(string filename, string contentType)
        {
            Blob.Properties.ContentType = contentType;
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
