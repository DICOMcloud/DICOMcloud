using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DICOMcloud.IO
{
    public interface IStorageContainer
    { 
        string Connection
        {
            get;
        }

        IStorageLocation              GetLocation    ( string name = null, IMediaId id = null ) ;
        IAsyncEnumerable<IStorageLocation> GetLocations   (string key );
        bool                          LocationExists ( string key );
        void                          Delete ( );
    }
}
