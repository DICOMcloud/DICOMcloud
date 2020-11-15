using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dicom;
using fo = Dicom ;
using Dicom.Imaging ;
using Dicom.Imaging.Codec;

namespace DICOMcloud
{
    public class UncompressedPixelDataWrapper
    {
        public UncompressedPixelDataWrapper ( fo.DicomDataset ds )
        {
            if (ds.InternalTransferSyntax.IsEncapsulated)
            {
                Dataset = ds.Clone(DicomTransferSyntax.ImplicitVRLittleEndian).NotValidated();
            }
            else
            {
                // pull uncompressed frame from source pixel data
                Dataset = ds ;
            }

            PixelData = DicomPixelData.Create(Dataset);
        }

        public fo.DicomDataset Dataset { get; private set; }

        public DicomPixelData PixelData
        {
            get;
            private set;
        }
    }
}
