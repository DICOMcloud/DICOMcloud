using DICOMcloud.DataAccess;
using DICOMcloud.DataAccess.Matching;
using FellowOakDicom;
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

        public IEnumerable<DicomDataset> Find 
        ( 
            DicomDataset request, 
            IQueryOptions options,
            string queryLevel
        )
        {

            IEnumerable<IMatchingCondition> conditions = null;


            conditions = BuildConditions ( request, new ConditionFactory ( ) );

            return DoFind ( request, options, queryLevel, conditions );
        }

        public PagedResult<DicomDataset> FindPaged
        ( 
            DicomDataset request, 
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
            DicomDataset request,
            ConditionFactory condFactory
        )
        {
            return condFactory.ProcessDataSet ( request ) ;
        }

        protected abstract IEnumerable<DicomDataset> DoFind
        (
            DicomDataset request,
            IQueryOptions options, 
            string queryLevel,
            IEnumerable<IMatchingCondition> conditions
        ) ;

        protected abstract PagedResult<DicomDataset> DoFindPaged
        (
            DicomDataset request,
            IQueryOptions options, 
            string queryLevel,
            IEnumerable<IMatchingCondition> conditions
        ) ;
    }
}
