using DICOMcloud.DataAccess.Matching;
using System.Collections.Generic;
using fo = Dicom;


namespace DICOMcloud.DataAccess
{
    public interface IObjectStorageQueryDataAccess
    {
        ICollection<fo.DicomDataset> Search
        ( 
            IEnumerable<IMatchingCondition> conditions, 
            IQueryOptions options,
            string queryLevel
        ) ;
    }
}
