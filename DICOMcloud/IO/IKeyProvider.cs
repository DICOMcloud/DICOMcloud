namespace DICOMcloud.IO
{
    public interface IKeyProvider
    {
        string GetStorageKey (IMediaId key ) ;
    
        string GetLogicalSeparator ( ) ;

        string GetContainerName ( string key ) ;

        string GetLocationName ( string key ) ;    
    }
}
