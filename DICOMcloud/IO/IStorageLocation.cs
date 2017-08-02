using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DICOMcloud.IO
{
    public interface IStorageLocation
    {
        string   ContentType { get; }
        string   Name        { get; }
        string   ID          { get; }
        IMediaId MediaId     { get; }
        string   Metadata    { get; set ; }
        long     Size        {get; }
        //IStorageContainer StorageContainer { get;  }


        void   Delete         ( ) ;
        //Stream GetWriteStream ( ) ;
        Stream Download       ( ) ;  
        void   Download       ( Stream stream  ) ;  
        void   Upload         ( Stream stream  ) ;
        void   Upload         ( byte[] buffer ) ;
        void   Upload         ( string filename ) ;
        Stream GetReadStream  ( ) ;

        bool Exists ( ) ;    
    }


}
