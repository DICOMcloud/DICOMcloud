using DICOMcloud.DataAccess;
using DICOMcloud.Pacs.Commands;
using fo = Dicom;

namespace DICOMcloud.Pacs
{
    public interface IObjectStoreService
    {
        StoreResult        StoreDicom ( fo.DicomDataset dataset, InstanceMetadata metadata ) ;
        DCloudCommandResult Delete     ( fo.DicomDataset request, ObjectQueryLevel  level ) ;
    }
}