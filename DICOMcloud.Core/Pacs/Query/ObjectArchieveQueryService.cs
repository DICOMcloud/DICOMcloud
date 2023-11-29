using DICOMcloud.Pacs;
using DICOMcloud.DataAccess;
using DICOMcloud.DataAccess.Matching;
using System;
using System.Collections.Generic;
using fo = Dicom;
using FellowOakDicom;

namespace DICOMcloud.Pacs
{
    public class ObjectArchieveQueryService : DicomQueryServiceBase, IObjectArchieveQueryService
    {
        public ObjectArchieveQueryService ( IObjectArchieveDataAccess dataAccess ) : base ( dataAccess )
        {}

        public IEnumerable<DicomDataset> FindStudies 
        ( 
            DicomDataset request, 
            IQueryOptions options
        )
        {
            return Find ( request, options, Enum.GetName (typeof(ObjectQueryLevel), ObjectQueryLevel.Study ) ) ;
        }

        public IEnumerable<DicomDataset> FindObjectInstances
        (
            DicomDataset request,
            IQueryOptions options
        )
        {
            return Find ( request, options, Enum.GetName (typeof(ObjectQueryLevel), ObjectQueryLevel.Instance ) ) ;
        }

        public IEnumerable<DicomDataset> FindSeries
        (
            DicomDataset request,
            IQueryOptions options
        )
        {
            return Find ( request, options, Enum.GetName (typeof(ObjectQueryLevel), ObjectQueryLevel.Series ) ) ;
        }

        protected override IEnumerable<DicomDataset> DoFind
        (
           DicomDataset request,
           IQueryOptions options,
           string queryLevel,
           IEnumerable<IMatchingCondition> conditions
        )
        {
            return QueryDataAccess.Search ( conditions, options, queryLevel ) ;
        }


        public PagedResult<DicomDataset> FindStudiesPaged
        ( 
            DicomDataset request, 
            IQueryOptions options
        )
        {
            return FindPaged ( request, options, Enum.GetName (typeof(ObjectQueryLevel), ObjectQueryLevel.Study ) ) ;
        }

        public PagedResult<DicomDataset> FindObjectInstancesPaged
        (
            DicomDataset request,
            IQueryOptions options
        )
        {
            return FindPaged ( request, options, Enum.GetName (typeof(ObjectQueryLevel), ObjectQueryLevel.Instance ) ) ;
        }

        public PagedResult<DicomDataset> FindSeriesPaged
        (
            DicomDataset request,
            IQueryOptions options
        )
        {
            return FindPaged ( request, options, Enum.GetName (typeof(ObjectQueryLevel), ObjectQueryLevel.Series ) ) ;
        }

        protected override PagedResult<DicomDataset> DoFindPaged
        (
           DicomDataset request,
           IQueryOptions options,
           string queryLevel,
           IEnumerable<IMatchingCondition> conditions
        )
        {
            return QueryDataAccess.SearchPaged ( conditions, options, queryLevel ) ;
        }
    }
}
