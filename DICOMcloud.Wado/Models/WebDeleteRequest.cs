
using fo = Dicom;

namespace DICOMcloud.Wado.Models
{
    public interface IWebDeleteRequest
    {
        ObjectQueryLevel     DeleteLevel { get; set; } 
        fo.DicomDataset Dataset     { get; set;  }
    }

    public class WebDeleteRequest : IWebDeleteRequest
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
