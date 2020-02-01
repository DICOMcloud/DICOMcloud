using DICOMcloud;
using DICOMcloud.DataAccess.Database;
using DICOMcloud.DataAccess.Database.Schema;
using DICOMcloud.DataAccess.Matching;
using System;
using System.Collections.Generic;
using Dicom;
using System.Linq;

namespace DICOMcloud.DataAccess
{
    public class ObjectArchieveDataAccess : IObjectArchieveDataAccess
    {
        public ObjectArchieveDataAdapter DataAdapter      { get; private set; }
        public DbSchemaProvider          SchemaProvider   { get; private set; }

        public ObjectArchieveDataAccess
        ( 
            DbSchemaProvider schemaProvier,
            ObjectArchieveDataAdapter dataAdapter
        )
        { 
            SchemaProvider   = schemaProvier ;
            DataAdapter      = dataAdapter ;
        }

        public virtual IEnumerable<DicomDataset> Search
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

            return cmd.Result;
        }

        public virtual PagedResult<DicomDataset> SearchPaged
        ( 
            IEnumerable<IMatchingCondition> conditions, 
            IQueryOptions options,
            string queryLevel
        )
        {
            IQueryResponseBuilder responseBuilder;
            PagedResult<DicomDataset> result ;


            if ( null == options || !options.Limit.HasValue || !options.Offset.HasValue )
            {
                throw new ArgumentNullException ("options", "Query options must have a value for paged result") ;
            }

            if ( options.Limit == 0 )
            {
                throw new IndexOutOfRangeException ( "Invalid query limit for paged result") ;
            }

            responseBuilder = CreateResponseBuilder ( queryLevel ) ;

            conditions = AddAdditionalQueryParameters(conditions, options, queryLevel);

            var cmd = DataAdapter.CreateSelectCommand ( SchemaProvider.GetQueryTable ( queryLevel ), 
                                                        conditions, 
                                                        options, 
                                                        responseBuilder);

            cmd.Execute();

            FillAdditionalQueryParameters(conditions, options, responseBuilder, queryLevel);

            result = new PagedResult<DicomDataset> ( cmd.Result, 
                                                     options.Offset.Value,  
                                                     options.Limit.Value,
                                                     cmd.TotalCount.HasValue ? cmd.TotalCount.Value : cmd.Result.Count()) ;

            return result ;
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

            var cmd = DataAdapter.CreateInsertCommand (objectId, parameters, data );

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

        /// <summary>
        /// Add additional query parameters to the <paramref name="conditions"/> collection 
        /// Study Level: (ModalitiesInStudy, NumberOfStudyRelatedSeries, NumberOfStudyRelatedInstances...)
        /// Series Level:
        /// Instance Level:
        /// </summary>
        /// <param name="conditions">
        /// A collection of <see cref="IEnumerable<IMatchingCondition>"/> that contains the conditions for the query.
        /// </param>
        /// <param name="options">
        /// An object of <see cref="IQueryOptions"/> with query options.
        /// </param>
        /// <param name="queryLevel">
        /// A <see cref="String"/> representing the DICOM query level (e.g. Study, Series, Instance)
        /// </param>
        protected virtual IEnumerable<IMatchingCondition> AddAdditionalQueryParameters
        (
            IEnumerable<IMatchingCondition> conditions, 
            IQueryOptions options, 
            string queryLevel
        )
        {
            List< IMatchingCondition > matchingConditions = new List<IMatchingCondition> (conditions);

            if (queryLevel == ObjectQueryLevelConstants.Study)
            {
                if (conditions.FirstOrDefault ( n=> n.KeyTag == DicomTag.ModalitiesInStudy) != null &&
                    conditions.FirstOrDefault (n => n.KeyTag == DicomTag.Modality) == null )
                {
                    var condition  = new SingleValueMatching ( );

                    condition.KeyTag = (uint) DicomTag.Modality;
                    condition.VR = DicomTag.Modality.DictionaryEntry.ValueRepresentations.First ( );

                    matchingConditions.Add (condition);
                }

                if (conditions.FirstOrDefault(n => n.KeyTag == DicomTag.NumberOfStudyRelatedSeries) != null &&
                    conditions.FirstOrDefault(n => n.KeyTag == DicomTag.SeriesInstanceUID) == null)
                { 
                    var condition  = new SingleValueMatching ( );

                    condition.KeyTag = (uint) DicomTag.SeriesInstanceUID;
                    condition.VR = DicomTag.SeriesInstanceUID.DictionaryEntry.ValueRepresentations.First ( );

                    matchingConditions.Add (condition);                
                }

                if (conditions.FirstOrDefault(n => n.KeyTag == DicomTag.NumberOfStudyRelatedInstances) != null &&
                    conditions.FirstOrDefault(n => n.KeyTag == DicomTag.SOPInstanceUID) == null)
                { 
                    var condition  = new SingleValueMatching ( );

                    condition.KeyTag = (uint) DicomTag.SOPInstanceUID;
                    condition.VR = DicomTag.SOPInstanceUID.DictionaryEntry.ValueRepresentations.First ( );

                    matchingConditions.Add (condition);
                }
            }
            else if (queryLevel == ObjectQueryLevelConstants.Series)
            {
                if (conditions.FirstOrDefault(n => n.KeyTag == DicomTag.NumberOfSeriesRelatedInstances) != null &&
                    conditions.FirstOrDefault(n => n.KeyTag == DicomTag.SOPInstanceUID) == null)
                {
                    var condition = new SingleValueMatching();

                    condition.KeyTag = (uint)DicomTag.SOPInstanceUID;
                    condition.VR = DicomTag.SOPInstanceUID.DictionaryEntry.ValueRepresentations.First();

                    matchingConditions.Add(condition);
                }
            }

            return matchingConditions;
        }

        protected virtual void FillAdditionalQueryParameters
        (
            IEnumerable<IMatchingCondition> conditions, 
            IQueryOptions options, 
            IQueryResponseBuilder responseBuilder, 
            string queryLevel
        )
        {
            if (queryLevel == ObjectQueryLevelConstants.Study)
            {
                Dictionary<string, StudyAdditionalParams> studyKeyValuePairs = new Dictionary<string, StudyAdditionalParams>();
                var studies = responseBuilder.GetResults(ObjectQueryLevelConstants.Study);


                FillStudyRelatedSeriesParameters   (responseBuilder, studyKeyValuePairs);
                FillStudyRelatedInstancesParameters(responseBuilder, studyKeyValuePairs);

                foreach (var studyDs in studies)
                {
                    string studyUid = studyDs.GetSingleValueOrDefault (DicomTag.StudyInstanceUID, "");
                    
                    if (studyKeyValuePairs.ContainsKey (studyUid))
                    { 
                        var studyParams = studyKeyValuePairs[studyUid];


                        studyDs.AddOrUpdate (DicomTag.ModalitiesInStudy, studyParams.Modality.ToArray());
                        studyDs.AddOrUpdate(DicomTag.NumberOfStudyRelatedSeries, studyParams.NumberOfSeries);
                        studyDs.AddOrUpdate(DicomTag.NumberOfStudyRelatedInstances, studyParams.NumberOfInstances);
                    }
                }
            }
            else if (queryLevel == ObjectQueryLevelConstants.Series)
            {
                Dictionary<string, SeriesAdditionalParams> seriesKeyValuePairs = new Dictionary<string, SeriesAdditionalParams>();
                var series = responseBuilder.GetResults(ObjectQueryLevelConstants.Series);


                FillSeriesRelatedInstancesParameters(responseBuilder, seriesKeyValuePairs);

                foreach (var seriesDs in series)
                {
                    var seriesUid = seriesDs.GetSingleValueOrDefault (DicomTag.SeriesInstanceUID, "");
                    
                    if ( seriesKeyValuePairs.ContainsKey (seriesUid))
                    { 
                        var seriesParams = seriesKeyValuePairs[seriesUid];


                        seriesDs.AddOrUpdate(DicomTag.NumberOfSeriesRelatedInstances, seriesParams.NumberOfInstances);
                    }
                }
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

        private static void FillStudyRelatedSeriesParameters
        (
            IQueryResponseBuilder responseBuilder, 
            Dictionary<string, StudyAdditionalParams> studyKeyValuePairs
        )
        {
            var series = responseBuilder.GetResults(ObjectQueryLevelConstants.Series);

            foreach (var seriesDs in series)
            {
                var studyKey  = seriesDs.GetSingleValueOrDefault (DicomTag.StudyInstanceUID, "");
                var seriesKey = seriesDs.GetSingleValueOrDefault (DicomTag.SeriesInstanceUID, "");
                var modality  = seriesDs.GetSingleValueOrDefault (DicomTag.Modality, "");
                StudyAdditionalParams studyParams = null;


                if (!studyKeyValuePairs.TryGetValue(studyKey, out studyParams))
                {
                    studyParams = new StudyAdditionalParams();

                    studyKeyValuePairs.Add(studyKey, studyParams);
                }

                studyParams.NumberOfSeries++;

                if (!string.IsNullOrEmpty(modality) && !studyParams.Modality.Contains (modality))
                {
                    studyParams.Modality.Add (modality);
                }
            }
        }

        private static void FillStudyRelatedInstancesParameters
        (
            IQueryResponseBuilder responseBuilder, 
            Dictionary<string, StudyAdditionalParams> studyKeyValuePairs
        )
        {
            var instances = responseBuilder.GetResults(ObjectQueryLevelConstants.Instance);

            foreach (var instanceDs in instances)
            {
                var studyKey  = instanceDs.GetSingleValueOrDefault (DicomTag.StudyInstanceUID, "");
                StudyAdditionalParams studyParams = null;


                if (!studyKeyValuePairs.TryGetValue(studyKey, out studyParams))
                {
                    studyParams = new StudyAdditionalParams();

                    studyKeyValuePairs.Add(studyKey, studyParams);
                }

                studyParams.NumberOfInstances++;
            }
        }

        private static void FillSeriesRelatedInstancesParameters
        (
            IQueryResponseBuilder responseBuilder,
            Dictionary<string, SeriesAdditionalParams> seriesKeyValuePairs
        )
        {
            var instances = responseBuilder.GetResults(ObjectQueryLevelConstants.Instance);

            foreach (var instanceDs in instances)
            {
                var seriesKey = instanceDs.GetSingleValueOrDefault(DicomTag.SeriesInstanceUID, "");
                SeriesAdditionalParams seriesParams = null;


                if (!seriesKeyValuePairs.TryGetValue(seriesKey, out seriesParams))
                {
                    seriesParams = new SeriesAdditionalParams();

                    seriesKeyValuePairs.Add(seriesKey, seriesParams);
                }

                seriesParams.NumberOfInstances++;
            }
        }

        private sealed class StudyAdditionalParams
        { 
            public List<string> Modality = new List<string>();
            public int NumberOfSeries = 0;
            public int NumberOfInstances = 0;

        }

        private sealed class SeriesAdditionalParams
        {
            public int NumberOfInstances = 0;
        }
    }
}
