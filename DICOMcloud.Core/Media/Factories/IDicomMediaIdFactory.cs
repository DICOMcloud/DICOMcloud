using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DICOMcloud.IO;
using DICOMcloud;
using fo = Dicom ;
using FellowOakDicom;

namespace DICOMcloud.Media
{
    public interface IDicomMediaIdFactory
    {
        IMediaId Create ( IObjectId objectId, DicomMediaProperties mediaInfo ) ;

        IMediaId Create
        ( 
            DicomDataset dataset, 
            int frame, 
            string mediaType,
            string transferSyntax
        ) ;

        IMediaId Create ( string[] parts );
    }
}
