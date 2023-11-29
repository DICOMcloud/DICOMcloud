using DICOMcloud.DataAccess;
using DICOMcloud.Pacs.Commands;
using FellowOakDicom;
using fo = Dicom;

namespace DICOMcloud.Pacs
{
    public interface IObjectStoreService
    {
        DCloudCommandResult StoreDicom ( DicomDataset dataset, InstanceMetadata metadata ) ;
        DCloudCommandResult Delete     ( DicomDataset request, ObjectQueryLevel  level ) ;
    }
}