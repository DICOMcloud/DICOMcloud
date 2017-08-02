using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using fo = Dicom;

namespace DICOMcloud.Pacs.Commands
{
    public interface IStoreCommandResult : IDCloudCommandResult
    {
        fo.DicomDataset ReferencedSopInstance { get; set; }
    }

    public class StoreCommandResult : DCloudCommandResult
    {
        public fo.DicomDataset ReferencedSopInstance { get; set; }
    }
}
