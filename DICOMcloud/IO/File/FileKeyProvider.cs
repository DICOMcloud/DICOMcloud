using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DICOMcloud.IO
{
    /// <summary>
    /// Returns information about the storage files based on a key.
    /// This class returns the information for a Physical file system (e.g. windows file)
    /// </summary>
    public class FileKeyProvider : IKeyProvider
    {
        public virtual string GetContainerName ( string key ) 
        {
            int index = key.LastIndexOf ( GetLogicalSeparator ( )  );
            
            if ( index == -1 )
            { 
               return key;
            }
            else
            { 
               return key.Substring ( 0,  index ) ;
            }
        }

        public virtual string GetLocationName ( string key ) 
        {
            int index = key.LastIndexOf(GetLogicalSeparator());


            if (index == -1)
            { 
               return string.Empty;
            }
            else
            {
               return key.Substring(key.LastIndexOf(GetLogicalSeparator()) + 1);
            }
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
