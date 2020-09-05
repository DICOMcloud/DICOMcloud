using DICOMcloud;
using DICOMcloud.IO;
using DICOMcloud.Media;
using System.Collections.Generic;

namespace DICOMcloud.Pacs
{
    public interface IObjectRetrieveService
    { 
        IStorageLocation RetrieveSopInstance ( IObjectId query, DicomMediaProperties mediaInfo ) ;

        IAsyncEnumerable<IStorageLocation> RetrieveSopInstances ( IObjectId query, DicomMediaProperties mediaInfo ) ;

        IAsyncEnumerable<ObjectRetrieveResult> FindSopInstances 
        ( 
            IObjectId query, 
            string mediaType, 
            IEnumerable<string> transferSyntaxes, 
            string defaultTransfer
        ) ;

        IAsyncEnumerable<ObjectRetrieveResult> GetTransformedSopInstances 
        ( 
            IObjectId query, 
            string fromMediaType, 
            string fromTransferSyntax, 
            string toMediaType, 
            string toTransferSyntax 
        ) ;

        bool ObjetInstanceExist ( IObjectId objectId, string mediaType, string transferSyntax ) ;
    }
}
