using DICOMcloud.Pacs;
using DICOMcloud.DataAccess;
using DICOMcloud.DataAccess.Matching;
using System;
using System.Collections.Generic;
using fo = Dicom;


namespace DICOMcloud.Pacs
{
    public class ObjectArchieveQueryService : DicomQueryServiceBase, IObjectArchieveQueryService
    {
        public ObjectArchieveQueryService ( IObjectStorageDataAccess dataAccess ) : base ( dataAccess )
        {}

        public ICollection<fo.DicomDataset> FindStudies 
        ( 
            fo.DicomDataset request, 
            IQueryOptions options
        )
        {
            return Find ( request, options, Enum.GetName (typeof(ObjectQueryLevel), ObjectQueryLevel.Study ) ) ;
        }

        public ICollection<fo.DicomDataset> FindObjectInstances
        (
            fo.DicomDataset request,
            IQueryOptions options
        )
        {
            return Find ( request, options, Enum.GetName (typeof(ObjectQueryLevel), ObjectQueryLevel.Instance ) ) ;
        }

        public ICollection<fo.DicomDataset> FindSeries
        (
            fo.DicomDataset request,
            IQueryOptions options
        )
        {
            return Find ( request, options, Enum.GetName (typeof(ObjectQueryLevel), ObjectQueryLevel.Series ) ) ;
        }

        protected override ICollection<fo.DicomDataset> DoFind
        (
           fo.DicomDataset request,
           IQueryOptions options,
           string queryLevel,
           IEnumerable<IMatchingCondition> conditions
        )
        {
            return QueryDataAccess.Search ( conditions, options, queryLevel ) ;
        }
    }
}
