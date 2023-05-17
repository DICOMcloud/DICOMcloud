using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DICOMcloud.IO
{
    public class LocationUploadedMessage : LocationMessage
    {
        public LocationUploadedMessage ( IStorageLocation location ) : base ( location ) 
        {
            
        }
    }
}
