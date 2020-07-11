using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DICOMcloud.IO;
using Dicom;
using Dicom.Imaging ;
using Dicom.Imaging.Codec;

namespace DICOMcloud.Media
{
    public class NativeMediaWriter : DicomMediaWriterBase
    {
        public NativeMediaWriter ( ) : base ( ) {}
         
        public NativeMediaWriter 
        ( 
            IMediaStorageService mediaStorage, 
            IDicomMediaIdFactory mediaFactory 
        ) : base ( mediaStorage, mediaFactory ) 
        {
        }

        public override string MediaType 
        { 
            get 
            {
                return MimeMediaTypes.DICOM ;
            }
        }

        protected override bool StoreMultiFrames
        {
            get
            {
                return false ;
            }
        }

        protected override DicomDataset GetMediaDataset ( DicomDataset data, DicomMediaProperties mediaInfo  )
        {
            if ( mediaInfo.MediaType != MediaType )
            {
                throw new InvalidOperationException ( string.Format ( "Invalid media type. Supported media type is:{0} and provided media type is:{1}",
                                                      MediaType, mediaInfo.MediaType ) ) ;
            }

            if ( !string.IsNullOrWhiteSpace ( mediaInfo.TransferSyntax ) && mediaInfo.TransferSyntax != "*" )
            {
                var transfer = DicomTransferSyntax.Parse(mediaInfo.TransferSyntax) ;
                
                if (transfer == data.InternalTransferSyntax)
                {
                    return data;
                }

                var ds = data.Clone (transfer).NotValidated();

                ds.AddOrUpdate ( DicomTag.TransferSyntaxUID, transfer.UID.UID ) ;

                return ds ;
            }
            else
            { 
                return base.GetMediaDataset ( data, mediaInfo );
            }
        }

        protected override void Upload
        ( 
            DicomDataset dicomDataset, 
            int frame, 
            IStorageLocation location, 
            DicomMediaProperties mediaProperties 
        )
        {
            DicomFile df = new DicomFile ( dicomDataset) ;

            using (Stream stream = new MemoryStream())
            {
                df.Save(stream);
                stream.Position = 0;

                location.Upload(stream, MediaType);
            }
        }
    }
}
