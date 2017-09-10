using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DICOMcloud.IO
{
    public class LocationDeletedMessage : LocationMessage
    {
        public LocationDeletedMessage ( IStorageLocation location ) : base ( location ) 
        {
            
        }
    }
}
