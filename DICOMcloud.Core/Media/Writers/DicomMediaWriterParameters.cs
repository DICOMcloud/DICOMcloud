using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using fo = Dicom ;


namespace DICOMcloud.Media
{
    public class DicomMediaWriterParameters
    {
        public fo.DicomDataset Dataset
        {
            get; 
            set;
        }

        public DicomMediaProperties MediaInfo 
        {
            get ; 
            set ;
        }
    }
}
