using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using fo = Dicom ;

namespace DICOMcloud.Media
{
    public class DicomMediaProperties
    {
        public DicomMediaProperties ( ) 
        {}

        public DicomMediaProperties ( string mediaType ) : this ( mediaType, null ) {}
        public DicomMediaProperties ( string mediaType, string transferSyntax )
        {
            MediaType      = mediaType ;
            TransferSyntax = transferSyntax ;
        }

        public string MediaType
        {
            get; set;
        }

        public string TransferSyntax
        {
            get; set;
        }

        public override string ToString ( )
        {
            return string.Format ("Media Type:{0}; TransferSyntax{1}", MediaType, TransferSyntax ?? "" ) ;
        }
    }
}
