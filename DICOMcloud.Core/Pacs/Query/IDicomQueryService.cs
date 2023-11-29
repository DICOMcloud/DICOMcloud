using System.Collections.Generic;
using fo = Dicom;
using DICOMcloud.DataAccess;
using FellowOakDicom;

namespace DICOMcloud.Pacs
{
    public interface IDicomQueryService
    {
        IEnumerable<DicomDataset> Find 
        ( 
            DicomDataset request, 
            IQueryOptions options,
            string queryLevel
        ) ;

        PagedResult<DicomDataset> FindPaged
        ( 
            DicomDataset request, 
            IQueryOptions options,
            string queryLevel
        ) ;
    }
}