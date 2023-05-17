using fo = Dicom;

namespace DICOMcloud.Pacs.Commands
{
    public interface IDCloudCommand<T, R>
    {
        R Execute ( T dataObject );
    }
}