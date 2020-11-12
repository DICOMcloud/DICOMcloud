using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using fo = Dicom;
using Dicom.Imaging;
using DICOMcloud.IO;
using Dicom.IO.Buffer;
using Dicom;

namespace DICOMcloud.Media
{
    public class UncompressedMediaWriter : DicomMediaWriterBase
    {
        public UncompressedMediaWriter ( ) : base ( ) {}
         
        public UncompressedMediaWriter ( IMediaStorageService mediaStorage, IDicomMediaIdFactory mediaFactory ) : base ( mediaStorage, mediaFactory ) {}

        public override string MediaType
        {
            get
            {
                return MimeMediaTypes.UncompressedData ;
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

        protected override void Upload ( fo.DicomDataset dicomDataset, int frame, IStorageLocation storeLocation, DicomMediaProperties mediaProperties)
        {
            var uncompressedData = new UncompressedPixelDataWrapper ( dicomDataset ) ;
            var buffer           = uncompressedData.PixelData.GetFrame(frame - 1);
            var  data            = new byte[0] ;
            
            
            try
            {
                //TODO: check fo-dicom, dicom file with no data will throw an exception althoug
                //it is wrapped with a RangeByteBuffer but Internal is EmptyBuffer
                //only way to find out is to ignore exception
                data = buffer.Data ;
            }
            catch {}

            storeLocation.Upload ( data, MediaType ) ;
        }
    }
}
