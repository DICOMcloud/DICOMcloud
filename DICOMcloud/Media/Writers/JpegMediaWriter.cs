using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using fo = Dicom;
using Dicom.Imaging;
using Dicom.Imaging.Codec ;
using DICOMcloud.IO;
using System.IO;
using Dicom;

namespace DICOMcloud.Media
{
    public class JpegMediaWriter : DicomMediaWriterBase
    {
        public JpegMediaWriter ( ) : base ( ) {}
         
        public JpegMediaWriter ( IMediaStorageService mediaStorage, IDicomMediaIdFactory mediaFactory ) : base ( mediaStorage, mediaFactory ) {}

        public override string MediaType
        {
            get
            {
                return MimeMediaTypes.Jpeg ;
            }
        }

        protected override bool StoreMultiFrames
        {
            get
            {
                return true ;
            }
        }

        public override bool CanUpload(DicomDataset ds, int frame)
        {
            var pixelDataItem = ds.GetDicomItem<DicomItem>(fo.DicomTag.PixelData);

            return pixelDataItem != null;
        }

        protected override fo.DicomDataset GetMediaDataset ( fo.DicomDataset data, DicomMediaProperties mediaInfo )
        {
            return base.GetMediaDataset ( data, mediaInfo ) ;
        }

        protected override void Upload 
        ( 
            fo.DicomDataset dicomObject, 
            int frame, 
            IStorageLocation storeLocation, 
            DicomMediaProperties mediaProperties 
        )
        {
            var frameIndex = frame - 1 ;
            var dicomImage = new DicomImage(dicomObject, frameIndex);
            var bitmap = dicomImage.RenderImage(frameIndex).AsSharedBitmap();
            var stream = new MemoryStream ( ) ;

            bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Jpeg);

            stream.Position = 0 ;

            storeLocation.Upload ( stream, MediaType ) ;
        }
    }
}
