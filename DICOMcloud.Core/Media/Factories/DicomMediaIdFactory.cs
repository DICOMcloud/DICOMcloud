using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DICOMcloud.IO;
using DICOMcloud;
using fo = Dicom ;

namespace DICOMcloud.Media
{
    public class DicomMediaIdFactory : IDicomMediaIdFactory
    {
        public virtual IMediaId Create ( IObjectId objectId, DicomMediaProperties mediaInfo )
        {
            return new DicomMediaId ( objectId, mediaInfo ) ;
        }

        public IMediaId Create
        ( 
            fo.DicomDataset dataset, 
            int frame, 
            string mediaType,
            string transferSyntax
        ) 
        {
            return new DicomMediaId ( dataset, frame, mediaType, transferSyntax ) ;
        }

        public virtual IMediaId Create ( string[] parts )
        {
            return new DicomMediaId ( parts ) ;
        }
    }
}
