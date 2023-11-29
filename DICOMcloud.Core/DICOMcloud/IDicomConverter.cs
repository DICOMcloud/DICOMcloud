using FellowOakDicom;
using fo = Dicom ;

namespace DICOMcloud
{
    public interface IDicomConverter<T>
    {
        
        T Convert ( DicomDataset dicom ) ;

        DicomDataset Convert ( T value ) ;
    }
}