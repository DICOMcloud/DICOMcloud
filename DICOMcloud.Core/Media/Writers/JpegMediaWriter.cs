using FellowOakDicom.Imaging.Render;

using FellowOakDicom.Imaging.Codec ;
using DICOMcloud.IO;
using System.IO;
using FellowOakDicom;
using FellowOakDicom.Imaging;

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
            var pixelDataItem = ds.GetDicomItem<DicomItem>(DicomTag.PixelData);

            return pixelDataItem != null;
        }

        protected override DicomDataset GetMediaDataset ( DicomDataset data, DicomMediaProperties mediaInfo )
        {
            return base.GetMediaDataset ( data, mediaInfo ) ;
        }

        protected override void Upload 
        ( 
            DicomDataset dicomObject, 
            int frame, 
            IStorageLocation storeLocation, 
            DicomMediaProperties mediaProperties 
        )
        {
            var frameIndex = frame - 1 ;
            var dicomImage = new DicomImage(dicomObject, frameIndex);
            FellowOakDicom.Imaging.IImage image = dicomImage.RenderImage(frameIndex);
            var bitmap = image.AsSharpImage();
            var stream = new MemoryStream ();

            bitmap.Save(stream, new SixLabors.ImageSharp.Formats.Jpeg.JpegEncoder());

            stream.Position = 0 ;

            storeLocation.Upload ( stream, MediaType ) ;
        }
    }
}
