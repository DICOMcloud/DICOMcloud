using DICOMcloud.DataAccess;
using DICOMcloud.DataAccess.Matching;
using System.Collections.Generic;
using fo = Dicom;

namespace DICOMcloud.Pacs
{

    // base class for query services
    public abstract class DicomQueryServiceBase : IDicomQueryService
    {
        public IObjectArchieveDataAccess QueryDataAccess { get; protected set; }

        public DicomQueryServiceBase ( IObjectArchieveDataAccess queryDataAccess )
        {
            QueryDataAccess = queryDataAccess ;
        }

        public IEnumerable<fo.DicomDataset> Find 
        ( 
            fo.DicomDataset request, 
            IQueryOptions options,
            string queryLevel
        )
        {

            IEnumerable<IMatchingCondition> conditions = null;


            conditions = BuildConditions ( request, new ConditionFactory ( ) );

            return DoFind ( request, options, queryLevel, conditions );
        }

        public PagedResult<fo.DicomDataset> FindPaged
        ( 
            fo.DicomDataset request, 
            IQueryOptions options,
            string queryLevel
        ) 
        {

            IEnumerable<IMatchingCondition> conditions = null;


            conditions = BuildConditions ( request, new ConditionFactory ( ) );

            return DoFindPaged ( request, options, queryLevel, conditions );
        }

        protected virtual IEnumerable<IMatchingCondition> BuildConditions
        (
            fo.DicomDataset request,
            ConditionFactory condFactory
        )
        {
            return condFactory.ProcessDataSet ( request ) ;
        }

        protected abstract IEnumerable<fo.DicomDataset> DoFind
        (
            fo.DicomDataset request,
            IQueryOptions options, 
            string queryLevel,
            IEnumerable<IMatchingCondition> conditions
        ) ;

        protected abstract PagedResult<fo.DicomDataset> DoFindPaged
        (
            fo.DicomDataset request,
            IQueryOptions options, 
            string queryLevel,
            IEnumerable<IMatchingCondition> conditions
        ) ;
    }
}
