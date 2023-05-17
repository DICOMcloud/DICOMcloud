using fo = Dicom ;

namespace DICOMcloud
{
    public interface IDicomConverter<T>
    {
        
        T Convert ( fo.DicomDataset dicom ) ;

        fo.DicomDataset Convert ( T value ) ;
    }
}