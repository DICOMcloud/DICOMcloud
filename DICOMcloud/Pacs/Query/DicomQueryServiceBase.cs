using DICOMcloud.DataAccess;
using DICOMcloud.DataAccess.Matching;
using System.Collections.Generic;
using fo = Dicom;

namespace DICOMcloud.Pacs
{

    //TODO: base class for query services
    public abstract class DicomQueryServiceBase : IDicomQueryService
    {
        public IObjectStorageDataAccess QueryDataAccess { get; protected set; }
        //public DbSchemaProvider             SchemaProvider  { get; protected set; }
        
        //public DicomQueryServiceBase ( IDicomStorageQueryDataAccess queryDataAccess )
        //: this ( queryDataAccess, new StorageDbSchemaProvider ( )*/ )
        //{
        //}

        public DicomQueryServiceBase ( IObjectStorageDataAccess queryDataAccess/*, DbSchemaProvider schemaProvider*/ )
        {
            QueryDataAccess = queryDataAccess ;
            //SchemaProvider  = schemaProvider ;
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
    }
}
