using DICOMcloud.IO;

namespace DICOMcloud.Azure.IO
{
    public class AzureKeyProvider : IKeyProvider
    {
        public virtual string GetContainerName(string key)
        {
            key = key.TrimStart ( GetLogicalSeparator ( ).ToCharArray ( ) ) ;

            int index = key.IndexOf(GetLogicalSeparator());
            index = (index > -1 ? index : key.Length);

            return key.Substring(0, index);
        }

        public virtual string GetLocationName(string key)
        {
            key = key.TrimStart ( GetLogicalSeparator ( ).ToCharArray ( ) ) ;

            int index = key.IndexOf(GetLogicalSeparator());

            index = (index > -1 ? index : 0);
            
            key = key.Substring(index, key.Length - index);
            
            return key.TrimStart ( GetLogicalSeparator ( ).ToCharArray ( ) ) ;
        }

        public string GetLogicalSeparator()
        {
            return "/" ;
        }

        public string GetStorageKey(IMediaId key)
        {
            return string.Join ( GetLogicalSeparator ( ), key.GetIdParts ( ) ) ;
        }
    }
}
