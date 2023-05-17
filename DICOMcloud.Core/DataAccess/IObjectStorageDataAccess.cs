using System.Collections.Generic;
using Dicom;
using DICOMcloud;
using DICOMcloud.DataAccess.Matching;

namespace DICOMcloud.DataAccess
{
    public interface IObjectArchieveDataAccess
    {
        IEnumerable<DicomDataset> Search
        ( 
            IEnumerable<IMatchingCondition> conditions, 
            IQueryOptions options,
            string queryLevel
        ) ;

        PagedResult<DicomDataset> SearchPaged
        ( 
            IEnumerable<IMatchingCondition> conditions, 
            IQueryOptions options,
            string queryLevel
        ) ;

        void StoreInstance        ( IObjectId objectId, IEnumerable<IDicomDataParameter> parameters, InstanceMetadata data ) ;
        void StoreInstanceMetadata( IObjectId objectId, InstanceMetadata data ) ;
        
        bool Exists ( IObjectId instance );

        bool DeleteInstance ( IObjectId instance );
        bool DeleteStudy    ( IStudyId  study    );
        bool DeleteSeries   ( ISeriesId series   );
        
        IEnumerable<InstanceMetadata> GetStudyMetadata    ( IStudyId study );
        IEnumerable<InstanceMetadata> GetSeriesMetadata   ( ISeriesId series );
        InstanceMetadata              GetInstanceMetadata ( IObjectId instance ) ;

    }
}