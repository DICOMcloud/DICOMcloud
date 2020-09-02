using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DICOMcloud.IO
{
    public class LocalStorageContainer : IStorageContainer
    {
        private List<IStorageLocation> _tempLocations = new List<IStorageLocation> ( ) ;
        
        public LocalStorageContainer () 
        {
            FolderPath = GetDefaultStoragePath ( ) ;
        }

        protected virtual string GetDefaultStoragePath()
        {
            return Environment.CurrentDirectory ;
        }

        public string FolderPath
        {
            get;
            private set;
        }
        

        public LocalStorageContainer ( string folderPath )
        {
            FolderPath = folderPath ;
        }

        public string Connection
        {
            get
            {
                return FolderPath ;
            }
        }
        

        public IStorageLocation GetLocation ( string name = null, IMediaId id = null )
        {
            if ( string.IsNullOrWhiteSpace ( name ) )
            {
                name = Guid.NewGuid ( ).ToString ( );
            }

            return new LocalStorageLocation ( GetLocationPath ( name ), id );
        }

        public void Delete ( )
        {
            Directory.Delete ( FolderPath, true ) ;
        }

        public async IAsyncEnumerable<IStorageLocation> GetLocations ( string name )
        {
            //check if name is really a file 
            string path = Path.Combine ( FolderPath, name ) ;
            

            if ( File.Exists (path))
            {
                yield return GetLocation ( path ) ;
            }
            else if ( Directory.Exists ( path ))
            {
                foreach ( string file in Directory.EnumerateFiles ( path , "*", SearchOption.AllDirectories ) )
                {
                    yield return GetLocation ( file ) ;
                }
            }
            else
            {
                yield break ;
            }
        }

        public bool LocationExists ( string name )
        {
            return File.Exists ( Path.Combine ( FolderPath, name) ) ;
        }

        protected virtual string GetLocationPath ( string name )
        {
            return Path.Combine ( FolderPath, name );
        }
    }
}


        //public IStorageLocation GetTempLocation ( )
        //{
        //    IStorageLocation location = new LocalStorageLocation ( Path.GetTempFileName ( ) ) ;

        //    return location ;
        //}
