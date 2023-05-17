using System.Collections.Generic;
using DICOMcloud.IO;

namespace DICOMcloud.Media
{
    public interface IMediaWriter<T>
    {
        IList<IStorageLocation> CreateMedia ( T data, ILocationProvider storage ) ;

        IList<IStorageLocation> CreateMedia ( T data ) ;

        string MediaType 
        { 
            get ;
        }
    }
}