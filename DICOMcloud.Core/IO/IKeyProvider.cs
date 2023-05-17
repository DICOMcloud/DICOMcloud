namespace DICOMcloud.IO
{
    /// <summary>
    /// Returns information about the storage files based on a key
    /// </summary>
    public interface IKeyProvider
    {
        /// <summary>
        /// Returns the full path of the provided <paramref name="key"/>
        /// </summary>
        /// <param name="key">
        /// The <see cref="IMediaId"/> which storage path to be returned.
        /// </param>
        /// <returns>The storage path of the <paramref name="key"/></returns>
        string GetStorageKey (IMediaId key ) ;
    
        /// <summary>
        /// Returns the seperator for a path
        /// </summary>
        /// <returns>
        /// A <see cref="string"/> representing the seperator for a path
        /// </returns>
        string GetLogicalSeparator ( ) ;

        /// <summary>
        /// The Folder/Container part of the <paramref name="key"/>
        /// </summary>
        /// <param name="key">
        /// The key which folder to be returned.
        /// </param>
        /// <returns>
        /// A <see cref="string"/> representing the folder of the <paramref name="key"/>
        /// </returns>
        string GetContainerName ( string key ) ;

        /// <summary>
        /// The file name part of the <paramref name="key"/>
        /// </summary>
        /// <param name="key">
        /// The key which file name to be returned.
        /// </param>
        /// <returns>
        /// A <see cref="string"/> representing the file name of the <paramref name="key"/>
        /// </returns>
        string GetLocationName ( string key ) ;    
    }
}
