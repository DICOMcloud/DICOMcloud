
using FellowOakDicom;
using fo = Dicom;

namespace DICOMcloud.Wado.Models
{
    public class WebDeleteRequest
    {
        public DicomDataset Dataset
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
