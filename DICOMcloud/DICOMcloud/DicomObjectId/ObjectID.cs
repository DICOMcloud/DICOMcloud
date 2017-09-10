using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using fo = Dicom ;

namespace DICOMcloud
{
    public class ObjectId : IObjectId
    {
        public string SeriesInstanceUID
        {
            get ;
            set ;
        }

        public string SOPInstanceUID
        {
            get ;
            set ;
        }

        public string StudyInstanceUID
        {
            get ;
            set ;
        }
    
        public int? Frame { get; set; }    
    }
}
