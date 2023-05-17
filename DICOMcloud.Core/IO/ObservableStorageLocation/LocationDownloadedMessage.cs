using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DICOMcloud.IO
{
    public class LocationDownloadedMessage : LocationMessage
    {
        public LocationDownloadedMessage ( IStorageLocation location ) 
        : base ( location )
        {
        }

    }
}
