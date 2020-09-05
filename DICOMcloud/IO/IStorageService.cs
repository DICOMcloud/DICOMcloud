using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DICOMcloud.IO
{
    /// <summary>
    /// An interface for providing a service to perform file operations (read, write, delete).
    /// The interface abstracts the storage source (e.g. file system, cloud storage...) and the representation
    /// of the media to be stored
    /// </summary>
    public interface IMediaStorageService : ILocationProvider
    {
        //TODO: 
        //1. create async methods
        //2. methods to search/enumerate keys 
        //4. methods to search/enumerate streams

        /// <summary>
        /// Write the provided <paramref name="stream"/> to the media
        /// </summary>
        /// <param name="stream">
        /// The <see cref="Stream"/> to be written
        /// </param>
        /// <param name="contentType">
        /// The content type/subtype of the stored data (e.g. image/jpeg)
        /// </param>
        /// <param name="key">
        /// The <see cref="IMediaId"/> where the <paramref name="stream"/> will be written.
        /// </param>
        void Write  ( Stream stream, IMediaId key, string contentType ) ;

        /// <summary>
        /// Reads the data of the provided media key
        /// </summary>
        /// <param name="key">
        /// The <see cref="IMediaId"/> to be read
        /// </param>
        /// <returns>
        /// Returns the <see cref="Stream"/> of the provided media
        /// </returns>
        Task<Stream> Read   ( IMediaId key ) ;

        /// <summary>
        /// Determines whether a media exists by its Key
        /// </summary>
        /// <param name="key">
        /// The key of the <see cref="IMediaId"/> to be checked
        /// </param>
        /// <returns>
        /// True, if the media exists. Otherwise, false.
        /// </returns>
        bool Exists ( IMediaId key ) ;

        /// <summary>
        /// Returns all media objects exists in a given key.
        /// The Key in this case will be a folder representation.
        /// </summary>
        /// <param name="key">
        /// The <see cref="IMediaId"/> of the folder location.
        /// </param>
        /// <returns>
        /// Returns an <see cref="IEnumerable{IStorageLocation}"/> for all the medias in the given <paramref name="key"/>
        /// </returns>
        IAsyncEnumerable<IStorageLocation> EnumerateLocation ( IMediaId key ) ;

        /// <summary>
        /// Deletes all media represented in the given <paramref name="key"/>
        /// </summary>
        /// <param name="key">
        /// The key to the media to be deleted. This could represent a single media (file) or multiple (directory)
        /// </param>
        void DeleteLocations ( IMediaId key ) ;
    }
}
