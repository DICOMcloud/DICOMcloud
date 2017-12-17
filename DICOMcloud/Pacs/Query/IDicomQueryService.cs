using System.Collections.Generic;
using fo = Dicom;
using DICOMcloud.DataAccess;

namespace DICOMcloud.Pacs
{
    public interface IDicomQueryService
    {
        IEnumerable<fo.DicomDataset> Find 
        ( 
            fo.DicomDataset request, 
            IQueryOptions options,
            string queryLevel
        ) ;

        PagedResult<fo.DicomDataset> FindPaged
        ( 
            fo.DicomDataset request, 
            IQueryOptions options,
            string queryLevel
        ) ;
    }
}