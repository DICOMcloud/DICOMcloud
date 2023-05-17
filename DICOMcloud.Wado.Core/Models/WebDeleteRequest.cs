
using fo = Dicom;

namespace DICOMcloud.Wado.Models
{
    public class WebDeleteRequest
    {
        public fo.DicomDataset Dataset
        {
            get ;
            set ;
        }

        public ObjectQueryLevel DeleteLevel
        {
            get ;
            set ;
        }
    }
}
