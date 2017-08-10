using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using fo = Dicom;

namespace DICOMcloud.Pacs.Commands
{
    public interface IStoreCommand : IDCloudCommand<StoreCommandData,DCloudCommandResult>
    {
        
    }
}
