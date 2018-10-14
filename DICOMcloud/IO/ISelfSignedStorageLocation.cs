using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DICOMcloud.IO
{
    public interface ISelfSignedUrlStorageLocation : IStorageLocation
    {
        Uri GetReadUrl  ( DateTimeOffset? startTime, DateTimeOffset? expiryTime );
        Uri GetWriteUrl ( DateTimeOffset? startTime, DateTimeOffset? expiryTime );
    }


}
