using System.Collections.Generic;
using fo = Dicom;
using DICOMcloud.DataAccess;

namespace DICOMcloud.Pacs
{
    public interface IDicomQueryService
    {
        ICollection<fo.DicomDataset> Find 
        ( 
            fo.DicomDataset request, 
            IQueryOptions options,
            string queryLevel
        ) ;
    }
}