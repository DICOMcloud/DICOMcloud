using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DICOMcloud.IO
{
    public interface IMediaStorageService : ILocationProvider
    {
        //IStorageContainer CreateContainer ( string containerKey ) ;

        //IStorageContainer GetStorageContainer ( string containerKey ) ;
        
        //TODO: 
        //1. create async methods
        //2. methods to search/enumerate keys 
        //4. methods to search/enumerate streams

        void   Write  ( Stream stream, IMediaId key ) ;
        Stream Read   ( IMediaId key ) ;
        bool   Exists ( IMediaId key ) ;

        IEnumerable<IStorageLocation> EnumerateLocation ( IMediaId key ) ;

        void DeleteLocations ( IMediaId key ) ;
    }
}
