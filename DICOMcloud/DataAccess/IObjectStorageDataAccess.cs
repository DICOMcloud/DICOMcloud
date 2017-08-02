using System.Collections.Generic;
using DICOMcloud;

namespace DICOMcloud.DataAccess
{
    public interface IObjectStorageDataAccess
    {
        void StoreInstance        ( IObjectId objectId, IEnumerable<IDicomDataParameter> parameters, InstanceMetadata data ) ;
        void StoreInstanceMetadata( IObjectId objectId, InstanceMetadata data ) ;
        
        bool DeleteInstance ( IObjectId instance );
        bool DeleteStudy    ( IStudyId  study    );
        bool DeleteSeries   ( ISeriesId series   );
        
        IEnumerable<InstanceMetadata> GetStudyMetadata    ( IStudyId study );
        IEnumerable<InstanceMetadata> GetSeriesMetadata   ( ISeriesId series );
        InstanceMetadata              GetInstanceMetadata ( IObjectId instance ) ;

    }
}