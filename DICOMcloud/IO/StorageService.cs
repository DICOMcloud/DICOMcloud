using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DICOMcloud.IO
{
    public abstract class MediaStorageService : IMediaStorageService 
    {
        public virtual Task<Stream> Read ( IMediaId key ) 
        {
            var location = GetLocation ( key ) ;
        
            return location.Download ( ) ;    
        }
        
        public virtual void Write ( Stream stream, IMediaId id, string contentType )
        {
            var location = GetLocation ( id ) ;
        
            location.Upload ( stream, contentType ) ;
        }

        public virtual IStorageLocation GetLocation ( IMediaId id )
        {
            string            key       = KeyProvider.GetStorageKey ( id ) ;
            IStorageContainer container = GetContainer              ( KeyProvider.GetContainerName ( key ) ) ;
            var               location  = container.GetLocation     ( KeyProvider.GetLocationName  ( key ), id ) ; 
            

            return location ;
        }

        public async IAsyncEnumerable<IStorageLocation> EnumerateLocation ( IMediaId id )
        {
            string  key          = KeyProvider.GetStorageKey ( id ) ;
            string containerName = KeyProvider.GetContainerName ( key) ;
            
            
            await foreach ( IStorageContainer container in GetContainers ( containerName ) )
            {
                await foreach ( IStorageLocation location in container.GetLocations ( KeyProvider.GetLocationName (key) ) )
                {
                    yield return location ;
                }
            }

        }

        public void DeleteLocations ( IMediaId id )
        {
            string  key          = KeyProvider.GetStorageKey ( id ) ;
            string containerName = KeyProvider.GetContainerName ( key) ;
            
            
            if ( ContainerExists ( key) )
            {
                var container = GetContainer ( key ) ;

                container.Delete ( ) ;
            }
            else
            {
                var location = GetLocation (id ) ;
                
                location.Delete ( ) ;
            }
        }

        public bool Exists ( IMediaId id )
        {
            string key           = KeyProvider.GetStorageKey ( id ) ;
            string containerName = KeyProvider.GetContainerName ( key) ;
            IStorageContainer container ;


            if ( ContainerExists ( containerName ) )
            {
                container = GetContainer ( containerName ) ;

                return container.LocationExists ( KeyProvider.GetLocationName ( key ) ) ;
            }

            return false ;
        }

        protected virtual IKeyProvider KeyProvider 
        { 
            get
            {
                return GetKeyProvider ( ) ;
            }
        }

        /// <summary>
        /// Returns an object of type <see cref="IKeyProvider"/> 
        /// </summary>
        /// <returns>
        /// An <see cref="IKeyProvider"/>
        /// </returns>
        protected abstract IKeyProvider GetKeyProvider    ( ) ;
        
        /// <summary>
        /// Returns the <<see cref="IStorageContainer "/> for a container key
        /// </summary>
        /// <param name="containerKey">
        /// The container key to be returned.
        /// </param>
        /// <returns>
        /// An object of type <see cref="IStorageContainer"/>
        /// </returns>
        protected abstract IStorageContainer GetContainer      ( string containerKey ) ;
        
        /// <summary>
        /// Returns all instances of <see cref="IStorageContainer"/> for the given <paramref name="containerKey"/>
        /// </summary>
        /// <param name="containerKey">
        /// The container key to be returned
        /// </param>
        /// <returns>
        /// An instance of <see cref="IEnumerable{IStorageContainer}"/>
        /// </returns>
        protected abstract IAsyncEnumerable<IStorageContainer> GetContainers     ( string containerKey ) ;
        
        /// <summary>
        /// Determines whether a container exists based on the provided <paramref name="containerName"/>
        /// </summary>
        /// <param name="containerName">
        /// The container key ot be checked.
        /// </param>
        /// <returns>
        /// True if the container exists; Otherwise, false.
        /// </returns>
        protected abstract bool ContainerExists   ( string containerName );        
    }
}


//public virtual Stream GetWriteStream ( string key)
//{
//    IStorageLocation location = GetLocation ( key ) ;

//    return location.GetWriteStream();
//}
