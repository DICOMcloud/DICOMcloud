using DICOMcloud;
using DICOMcloud.DataAccess.Database;
using DICOMcloud.DataAccess.Database.Schema;
using DICOMcloud.DataAccess.Matching;
using System;
using System.Collections.Generic;
using fo = Dicom;

namespace DICOMcloud.DataAccess
{
    public class ObjectArchieveDataAccess : IObjectArchieveDataAccess
    {
        public string                    ConnectionString { get; set ; }
        public ObjectArchieveDataAdapter DataAdapter      { get; private set; }
        public DbSchemaProvider          SchemaProvider   { get; private set; }

        public ObjectArchieveDataAccess
        ( 
            IConnectionStringProvider connectionStringProvier, 
            DbSchemaProvider schemaProvier,
            ObjectArchieveDataAdapter dataAdapter
        ) : this ( connectionStringProvier.ConnectionString, schemaProvier, dataAdapter)
        { }

        public ObjectArchieveDataAccess
        ( 
            string connectionString, 
            DbSchemaProvider schemaProvier,
            ObjectArchieveDataAdapter dataAdapter
        )
        { 
            ConnectionString = connectionString ;
            SchemaProvider   = schemaProvier ;
            DataAdapter      = dataAdapter ;
        }

        public virtual IEnumerable<fo.DicomDataset> Search
        (
            IEnumerable<IMatchingCondition> conditions, 
            IQueryOptions options,
            string queryLevel
        )
        {

            IQueryResponseBuilder responseBuilder;

            responseBuilder = CreateResponseBuilder ( queryLevel ) ;

            var cmd = DataAdapter.CreateSelectCommand ( SchemaProvider.GetQueryTable ( queryLevel ), 
                                                        conditions, 
                                                        options, 
                                                        responseBuilder);

            cmd.Execute();

            return responseBuilder.GetResponse();
        }

        public virtual void StoreInstance 
        ( 
            IObjectId objectId,  
            IEnumerable<IDicomDataParameter> parameters, 
            InstanceMetadata data = null
        )
        {
            //TODO: use transation
            //dbAdapter.CreateTransation ( ) 

            var cmd = DataAdapter.CreateInsertCommand ( parameters, data );

            cmd.Connection.Open ( );

            try
            {
                int rowsInserted = cmd.ExecuteNonQuery ( );

                if ( rowsInserted <= 0 )
                {
                    //return duplicate instance?!!!
                }

                if ( null != data )
                {
                    StoreInstanceMetadata ( objectId, data );
                }
            }
            finally
            {
                cmd.Connection.Close ( );
            }
        }

        public virtual void StoreInstanceMetadata ( IObjectId objectId, InstanceMetadata data )
        {
            StoreInstanceMetadata ( objectId, data, DataAdapter );
        }

        public virtual IEnumerable<InstanceMetadata> GetStudyMetadata ( IStudyId study ) 
        {
            var command = DataAdapter.CreateGetMetadataCommand ( study ) ;
        

            command.Execute ( ) ;

            return command.Result ;
            //return GetInstanceMetadata ( DataAdapter, command ) ;
        }
        
        public virtual IEnumerable<InstanceMetadata> GetSeriesMetadata ( ISeriesId series ) 
        {
            var command = DataAdapter.CreateGetMetadataCommand ( series ) ;
        
            command.Execute ( ) ;

            return command.Result ;

            //return GetInstanceMetadata ( DataAdapter, command ) ;
        }

        public virtual InstanceMetadata GetInstanceMetadata ( IObjectId instance ) 
        {
            var command = DataAdapter.CreateGetMetadataCommand ( instance ) ;
        

            command.Execute ( ) ;
            return command.Result ; //GetInstanceMetadata ( DataAdapter, command ).FirstOrDefault ( ) ;
        }

        public virtual bool Exists ( IObjectId instance )
        {
            var command = DataAdapter.CreateSelectInstanceKeyCommand ( instance ) ;


            command.Execute ( ) ;

            return command.Result > 0 ; 
        }

        public virtual bool DeleteStudy ( IStudyId study )
        {
            long studyKey  = GetStudyKey ( DataAdapter, study ) ;
            
            
            return DataAdapter.CreateDeleteStudyCommand ( studyKey ).Execute ( ) ;
        }

        public virtual bool DeleteSeries ( ISeriesId series )
        {
            long seriesKey = GetSeriesKey ( DataAdapter, series ) ;
            
            
            return DataAdapter.CreateDeleteSeriesCommand ( seriesKey ).Execute ( ) ;
        }

        public virtual bool DeleteInstance ( IObjectId instance )
        {
            long instanceKey = GetInstanceKey ( DataAdapter, instance ) ;
            
            
            return DataAdapter.CreateDeleteInstancCommand ( instanceKey ).Execute ( ) ;
        }

        protected virtual bool StoreInstanceMetadata 
        ( 
            IObjectId objectId,
            InstanceMetadata data, 
            ObjectArchieveDataAdapter dbAdapter 
        )
        {
            return dbAdapter.CreateUpdateMetadataCommand ( objectId, data ).Execute ( ) ;
        }

        protected virtual IQueryResponseBuilder CreateResponseBuilder(string queryLevel)
        {
            return new QueryResponseBuilder ( SchemaProvider, queryLevel ) ;
        }

        protected virtual long GetStudyKey ( ObjectArchieveDataAdapter adapter, IStudyId study )
        {
            var cmd = adapter.CreateSelectStudyKeyCommand ( study ) ;


            if ( cmd.Execute ( ) )
            {
                return cmd.Result ;
            }
            else
            {
                throw new DCloudNotFoundException ( "study is not found." ) ;
            }
        }

        protected virtual long GetSeriesKey ( ObjectArchieveDataAdapter adapter, ISeriesId series )
        {
            var cmd = adapter.CreateSelectSeriesKeyCommand ( series ) ;


            if ( cmd.Execute ( ) )
            {
                return cmd.Result ;
            }
            else
            {
                throw new DCloudNotFoundException ( "series is not found." ) ;
            }
        }

        protected virtual long GetInstanceKey ( ObjectArchieveDataAdapter adapter, IObjectId instance )
        {
            var cmd = adapter.CreateSelectInstanceKeyCommand ( instance ) ;


            if ( cmd.Execute ( ) )
            {
                return cmd.Result ;
            }
            else
            {
                throw new DCloudNotFoundException ( "Instance is not found." ) ;
            }
            
        }

        private static T GetDbScalarValue<T> ( object result, T defaultValue )
        {
            if ( result != null && result != DBNull.Value )
            {
                return (T) result;
            }
            else
            {
                return defaultValue;
            }
        }
    }
}
