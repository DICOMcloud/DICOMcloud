using DICOMcloud.DataAccess;
using DICOMcloud.Messaging;

namespace DICOMcloud.Pacs.Commands
{
    public class DicomStoreSuccessMessage : TransportMessage
    {
        public DicomStoreSuccessMessage ( InstanceMetadata instanceMetadata )
        {
            InstanceMetadata = instanceMetadata ;
        }

        public InstanceMetadata InstanceMetadata { get ; private set ; }
    }
}
