using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dicom;

namespace DICOMcloud
{
    public class DCloudDuplicateInstanceException : DCloudException
    {
        public DCloudDuplicateInstanceException ( DicomDataset ds )
        : base  ( "SOP Instance already exists" ) 
        { 
            Dataset = ds ;
        }

        public DicomDataset Dataset { get; set; }
    }
}
