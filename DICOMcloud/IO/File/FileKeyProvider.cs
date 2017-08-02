using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DICOMcloud.IO
{
    public class FileKeyProvider : IKeyProvider
    {
        public virtual string GetContainerName ( string key ) 
        {
            return key.Substring ( 0, key.LastIndexOf ( GetLogicalSeparator ( )  ) ) ;
        }

        public virtual string GetLocationName ( string key ) 
        {
            return key.Substring ( key.LastIndexOf ( GetLogicalSeparator ( )  ) + 1 ) ;
        }

        public virtual string GetLogicalSeparator ( ) 
        {
            return Path.DirectorySeparatorChar.ToString ( ) ;
        }

        public virtual string GetStorageKey(IMediaId id)
        {
            return Path.Combine ( id.GetIdParts( ) ) ;
        }

     }
}
