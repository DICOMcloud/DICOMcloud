using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using fo = Dicom;
using Dicom.Imaging;
using Dicom.Imaging.Codec ;
using DICOMcloud.IO;


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


        protected override fo.DicomDataset GetMediaDataset ( fo.DicomDataset data, DicomMediaProperties mediaInfo )
        {
            //TODO: this is still not working with when images BPP are not compatible from one Transfer to the other (12/16-bit -> 8-bit)
            if ( !string.IsNullOrWhiteSpace ( mediaInfo.TransferSyntax ) )
            {
                return data.Clone ( fo.DicomTransferSyntax.Parse ( mediaInfo.TransferSyntax ) ) ;
            }
            else if (data.InternalTransferSyntax != fo.DicomTransferSyntax.JPEGProcess1)
            {
                return data.Clone ( fo.DicomTransferSyntax.JPEGProcess1 ) ;
            }

            return base.GetMediaDataset ( data, mediaInfo ) ;
        }

        protected override void Upload ( fo.DicomDataset dicomObject, int frame, IStorageLocation storeLocation )
        {
            var frameIndex = frame - 1 ;
            
            DicomPixelData pd = DicomPixelData.Create(dicomObject) ;

                
            byte[] buffer = pd.GetFrame ( frameIndex ).Data ;

            storeLocation.Upload ( buffer) ;
        }
    }
}
