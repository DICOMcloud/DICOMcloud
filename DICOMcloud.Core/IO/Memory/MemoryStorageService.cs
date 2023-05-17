using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DICOMcloud.IO
{
    public class MemoryStorageProvider : ILocationProvider
    {
        public MemoryStorageProvider ( ) 
        {
        }

        public IStorageLocation GetLocation ( IMediaId key )
        {
            return new MemoryStorageLocation ( Path.Combine ( key.GetIdParts ( ) ), key ) ;
        }
    }
}
