using DICOMcloud;
using DICOMcloud.DataAccess.Database.Commands;
using DICOMcloud.DataAccess.Database.Schema;
using DICOMcloud.DataAccess.Matching;
using DICOMcloud.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using fo = Dicom;
using Dicom;

namespace DICOMcloud.DataAccess.Database
{
    public partial class ObjectArchieveDataAdapter
    {
        #region Public

        public ObjectArchieveDataAdapter 
        ( 
            DbSchemaProvider schemaProvider, 
            IDatabaseFactory database 
        ) : this ( schemaProvider, database, null)
        {
        }

        public ObjectArchieveDataAdapter 
        ( 
            DbSchemaProvider schemaProvider, 
            IDatabaseFactory database,
            ISortingStrategyFactory sortingStrategyFactory = null
        )
        {
            SchemaProvider          = schemaProvider ;
            Database                = database ;
            SortingStrategyFactory  = sortingStrategyFactory ?? new SortingStrategyFactory ( schemaProvider ) ;
        }

        public DbSchemaProvider SchemaProvider
        {
            get;
            protected set;
        }

        public IDatabaseFactory Database
        {
            get ;
            protected set ;
        }

        public ISortingStrategyFactory SortingStrategyFactory { get; set; }

        public IDbCommand CreateCommand ( string commandText )
        {
            var command = Database.CreateCommand ( );

            SetConnectionIfNull ( command );

            command.CommandText = commandText ;

            return command;
        }

        public virtual IPagedDataAdapterCommand<DicomDataset> CreateSelectCommand 
        ( 
            string queryLevel, 
            IEnumerable<IMatchingCondition> conditions, 
            IQueryOptions options,
            IQueryResponseBuilder responseBuilder
        )
        {
            var queryLevelTable = SchemaProvider.GetTableInfo(SchemaProvider.GetQueryTable(queryLevel));

            if (queryLevelTable == null)
            {
                throw new ArgumentException("querylevel not supported");
            }
            
            var queryBuilder = BuildQuery(conditions, options, queryLevelTable);

            var sorting = SortingStrategyFactory.Create ( ) ;

            sorting.ApplyPagination = sorting.CanPaginate (queryBuilder, options, queryLevelTable);

            var sortedQuery = sorting.Sort ( queryBuilder, 
                                             options, 
                                             queryLevelTable ) ;

            var selectCommand = new DicomDsQueryCommand(CreateCommand(sortedQuery), queryBuilder, responseBuilder, options);

            // if the database strategy can't paginate then we'll do pagination in code.
            selectCommand.ApplyPagination = !sorting.ApplyPagination;

            if (!string.IsNullOrEmpty (sorting.CountColumn))
            {
                selectCommand.SetCountColumn ( queryLevelTable, sorting.CountColumn);
            }

            return selectCommand;
        }

        public virtual IDataAdapterCommand<long> CreateSelectStudyKeyCommand ( IStudyId study )
        {
            TableKey                   studyTable   = SchemaProvider.GetTableInfo ( StorageDbSchemaProvider.StudyTableName );
            QueryBuilder queryBuilder = CreateQueryBuilder ( ) ;
            
            ProcessSelectStudy ( study, queryBuilder, studyTable, studyTable ) ;

            return new SingleResultQueryCommand<long> ( CreateCommand ( queryBuilder.GetQueryText ( studyTable ) ), 
                                                        studyTable.Name,
                                                        studyTable.KeyColumn.Name ) ;
        }

        public virtual IDataAdapterCommand<long> CreateSelectSeriesKeyCommand ( ISeriesId series )
        {
            TableKey                   studyTable   = SchemaProvider.GetTableInfo ( StorageDbSchemaProvider.StudyTableName );
            TableKey                   seriesTable  = SchemaProvider.GetTableInfo ( StorageDbSchemaProvider.SeriesTableName );
            QueryBuilder queryBuilder = CreateQueryBuilder ( ) ;


            ProcessSelectStudy ( series, queryBuilder, studyTable, seriesTable ) ;
            ProcessSelectSeries ( series, queryBuilder, seriesTable, seriesTable ) ;

            return new SingleResultQueryCommand<long> ( CreateCommand ( queryBuilder.GetQueryText ( seriesTable ) ),
                                                        seriesTable.Name,
                                                        seriesTable.KeyColumn.Name ) ;
        }

        public virtual IDataAdapterCommand<long> CreateSelectInstanceKeyCommand ( IObjectId instance ) 
        {
            QueryBuilder queryBuilder = CreateQueryBuilder ( ) ;
            TableKey studyTable       = SchemaProvider.GetTableInfo ( StorageDbSchemaProvider.StudyTableName );
            TableKey seriesTable      = SchemaProvider.GetTableInfo ( StorageDbSchemaProvider.SeriesTableName );
            TableKey instanceTable    = SchemaProvider.GetTableInfo ( StorageDbSchemaProvider.ObjectInstanceTableName ) ;

           
            ProcessSelectObjectInstance ( instance, queryBuilder, instanceTable, instanceTable) ;
            ProcessSelectSeries         ( instance, queryBuilder, seriesTable, instanceTable ) ;
            ProcessSelectStudy          ( instance, queryBuilder, studyTable, seriesTable ) ;

            return new SingleResultQueryCommand<long> ( CreateCommand ( queryBuilder.GetQueryText ( instanceTable ) ),
                                                        instanceTable.Name, 
                                                        instanceTable.KeyColumn.Name ) ;                       
        }

        public virtual IDbCommand CreateInsertCommand 
        ( 
            IObjectId objectId,
            IEnumerable<IDicomDataParameter> conditions,
            InstanceMetadata data = null
        )
        {
            IDbCommand insertCommand = Database.CreateCommand ( ) ;

            BuildInsert (objectId, conditions, data, insertCommand ) ;

            SetConnectionIfNull ( insertCommand ) ;
            
            return insertCommand ;
        
        }
        
        public virtual IDataAdapterCommand<int> CreateDeleteStudyCommand ( long studyKey )
        {
            return new ExecuteNonQueryCommand ( CreateCommand ( new SqlDeleteStatments ( )
                                                              .GetDeleteStudyCommandText ( studyKey ) ) ) ;
        }
        
        public virtual IDataAdapterCommand<int> CreateDeleteSeriesCommand ( long seriesKey )
        {
            return new ExecuteNonQueryCommand ( CreateCommand ( new SqlDeleteStatments ( )
                                                               .GetDeleteSeriesCommandText ( seriesKey ) ) ) ;
        }
        
        public virtual IDataAdapterCommand<int> CreateDeleteInstancCommand ( long instanceKey )
        {
            return new ExecuteNonQueryCommand ( CreateCommand ( new SqlDeleteStatments ( ).
                                                               GetDeleteInstanceCommandText ( instanceKey ) ) ) ;
        }

        public IDataAdapterCommand<int> CreateUpdateMetadataCommand ( IObjectId objectId, InstanceMetadata data )
        {
            IDbCommand insertCommand = Database.CreateCommand ( ) ;
            var instance             = objectId ;
            

            insertCommand = Database.CreateCommand ( ) ;

            insertCommand.CommandText = string.Format ( @"
UPDATE {0} SET {2}=@{2}, {3}=@{3} WHERE {1}=@{1}

IF @@ROWCOUNT = 0
   INSERT INTO {0} ({2}, {3}) VALUES (@{2}, @{3})
", 
StorageDbSchemaProvider.MetadataTable.TableName, 
StorageDbSchemaProvider.MetadataTable.SopInstanceColumn, 
StorageDbSchemaProvider.MetadataTable.MetadataColumn,
StorageDbSchemaProvider.MetadataTable.OwnerColumn ) ;

             var sopParam   = Database.CreateParameter ( "@" + StorageDbSchemaProvider.MetadataTable.SopInstanceColumn, instance.SOPInstanceUID ) ;
             var metaParam  = Database.CreateParameter ( "@" + StorageDbSchemaProvider.MetadataTable.MetadataColumn, data.ToJson ( ) ) ;
             var ownerParam = Database.CreateParameter ( "@" + StorageDbSchemaProvider.MetadataTable.OwnerColumn, data.Owner ) ;
            
            insertCommand.Parameters.Add ( sopParam ) ;
            insertCommand.Parameters.Add ( metaParam ) ;
            insertCommand.Parameters.Add ( ownerParam ) ;

            SetConnectionIfNull ( insertCommand ) ;        
            
            return new ExecuteNonQueryCommand ( insertCommand ) ; 
        }

        public ResultSetQueryCommand<InstanceMetadata> CreateGetMetadataCommand ( IStudyId study )
        {
            TableKey                   studyTable     = SchemaProvider.GetTableInfo ( StorageDbSchemaProvider.StudyTableName );
            TableKey                   sourceTable    = SchemaProvider.GetTableInfo ( StorageDbSchemaProvider.ObjectInstanceTableName );
            QueryBuilder queryBuilder   = CreateQueryBuilder          ( );
            ColumnInfo                 metaDataColumn = SchemaProvider.GetColumn ( sourceTable.Name,
                                                                                   StorageDbSchemaProvider.MetadataTable.MetadataColumn ) ;

            ProcessSelectStudy ( study, queryBuilder, studyTable, sourceTable ) ;

            queryBuilder.ProcessColumn ( sourceTable, metaDataColumn, null, null );

            return new ResultSetQueryCommand<InstanceMetadata> ( CreateCommand ( queryBuilder.GetQueryText ( sourceTable ) ), 
                                                      sourceTable, 
                                                      new string [] { metaDataColumn.ToString ( ) },
                                                      CreateMetadata );
        }

        public ResultSetQueryCommand<InstanceMetadata> CreateGetMetadataCommand ( ISeriesId series )
        {
            TableKey                   studyTable   = SchemaProvider.GetTableInfo ( StorageDbSchemaProvider.StudyTableName );
            TableKey                   seriesTable  = SchemaProvider.GetTableInfo ( StorageDbSchemaProvider.SeriesTableName );
            TableKey                   sourceTable  = SchemaProvider.GetTableInfo ( StorageDbSchemaProvider.ObjectInstanceTableName );
            QueryBuilder queryBuilder = CreateQueryBuilder          ( );
            ColumnInfo                 metadataColumn = SchemaProvider.GetColumn ( sourceTable.Name,
                                                                                   StorageDbSchemaProvider.MetadataTable.MetadataColumn ) ;

            ProcessSelectStudy ( series, queryBuilder, studyTable, sourceTable ) ;
            ProcessSelectSeries ( series, queryBuilder, seriesTable, sourceTable ) ;

            queryBuilder.ProcessColumn ( sourceTable, metadataColumn  );

            return new ResultSetQueryCommand<InstanceMetadata> ( CreateCommand ( queryBuilder.GetQueryText ( sourceTable ) ),
                                                                 sourceTable,
                                                                 new string [] { metadataColumn.ToString ( ) },
                                                                 CreateMetadata ) ;
        }

        public SingleResultQueryCommand<InstanceMetadata> CreateGetMetadataCommand ( IObjectId instance )
        {
            TableKey                   studyTable    = SchemaProvider.GetTableInfo ( StorageDbSchemaProvider.StudyTableName );
            TableKey                   seriesTable   = SchemaProvider.GetTableInfo ( StorageDbSchemaProvider.SeriesTableName );
            TableKey                   instanceTable = SchemaProvider.GetTableInfo ( StorageDbSchemaProvider.ObjectInstanceTableName );
            QueryBuilder queryBuilder  = CreateQueryBuilder          ( );
            ColumnInfo                 metadataColumn = SchemaProvider.GetColumn ( instanceTable.Name,
                                                                                   StorageDbSchemaProvider.MetadataTable.MetadataColumn ) ;

            ProcessSelectStudy  ( instance, queryBuilder, studyTable, instanceTable ) ;
            ProcessSelectSeries ( instance, queryBuilder, seriesTable, instanceTable ) ;
            ProcessSelectObjectInstance ( instance, queryBuilder, instanceTable, instanceTable) ;

            queryBuilder.ProcessColumn ( instanceTable, metadataColumn  );

            return new SingleResultQueryCommand<InstanceMetadata> ( CreateCommand ( queryBuilder.GetQueryText ( instanceTable ) ),
                                                                    instanceTable,
                                                                    metadataColumn.ToString ( ),
                                                                    CreateMetadata ) ;
        }

        #endregion

        #region Protected
        
        protected virtual QueryBuilder BuildQuery 
        ( 
            IEnumerable<IMatchingCondition> conditions, 
            IQueryOptions options,
            TableKey queryLeveTable
        )
        {
            QueryBuilder queryBuilder = CreateQueryBuilder ( ) ;


            if ( null != conditions && conditions.Count ( ) > 0 )
            {
                foreach ( var condition in conditions )
                {
                    if ( condition.VR == fo.DicomVR.PN )
                    { 
                        List<PersonNameData> pnValues = new List<PersonNameData> ( ) ;

                         
                        pnValues = condition.GetPNValues ( ) ;
                        
                        foreach ( var values in pnValues )
                        {
                            int          index = -1 ;
                            string[]     stringValues = values.ToArray ( ) ;
                            List<string> pnConditions = new List<string> ( ) ;

                            foreach ( var column in SchemaProvider.GetColumnInfo ( condition.KeyTag ) )
                            { 
                                var columnValues = new string [] { stringValues[++index]} ;
                                
                                queryBuilder.ProcessColumn ( queryLeveTable, column, condition, columnValues ) ;
                            }
                        }
                    }
                    else
                    { 
                        IList<string> columnValues = GetValues ( condition )  ;

                        foreach ( var column in SchemaProvider.GetColumnInfo ( condition.KeyTag ) )
                        { 
                            queryBuilder.ProcessColumn ( queryLeveTable, column, condition, columnValues ) ;
                        }
                    }
                }
            }
            else
            {
                foreach ( var column in SchemaProvider.GetTableInfo( queryLeveTable ).Columns ) 
                {
                    queryBuilder.ProcessColumn ( queryLeveTable, column ) ;
                }
            }
        
            return queryBuilder ;
        }

        protected virtual void BuildInsert 
        ( 
            IObjectId objectId,
            IEnumerable<IDicomDataParameter> conditions, 
            InstanceMetadata data, 
            IDbCommand insertCommand 
        )
        {
            if ( null == conditions ) throw new ArgumentNullException ( ) ;

            var stroageBuilder = CreateStorageBuilder ( ) ;
            
            FillInsertParameters ( conditions, data, insertCommand, stroageBuilder ) ;
            
            insertCommand.CommandText = stroageBuilder.GetInsertText ( ) ;
        }
        
        protected virtual void ProcessSelectStudy
        (
            IStudyId study, 
            QueryBuilder queryBuilder, 
            TableKey studyTable,
            TableKey sourceTable
        )
        {
            SingleValueMatching uidMatching  = new SingleValueMatching ( ) ;


            queryBuilder.ProcessColumn ( sourceTable, 
                                         studyTable.ModelKeyColumns [0], 
                                         uidMatching, 
                                         new string[] { study.StudyInstanceUID } );
        }

        protected virtual void ProcessSelectSeries
        (
            ISeriesId series, 
            QueryBuilder queryBuilder, 
            TableKey seriesTable,
            TableKey sourceTable
        )
        {
            SingleValueMatching uidMatching  = new SingleValueMatching ( ) ;


            queryBuilder.ProcessColumn ( sourceTable, 
                                         seriesTable.ModelKeyColumns [0], 
                                         uidMatching, 
                                         new string[] { series.SeriesInstanceUID } );
        }

        protected virtual void ProcessSelectObjectInstance
        (
            IObjectId objectInstance, 
            QueryBuilder queryBuilder, 
            TableKey objectInstanceTable,
            TableKey sourceTable
        )
        {
            SingleValueMatching uidMatching  = new SingleValueMatching ( ) ;


            queryBuilder.ProcessColumn ( sourceTable, 
                                         objectInstanceTable.ModelKeyColumns [0], 
                                         uidMatching, 
                                         new string[] { objectInstance.SOPInstanceUID } );
        }

        protected virtual void SetConnectionIfNull ( IDbCommand command )
        {
            if (command !=null && command.Connection == null)
            {
                command.Connection = Database.CreateConnection ( ) ;
            }
        }

        protected virtual void FillInsertParameters
        (
            IEnumerable<IDicomDataParameter> dicomParameters,
            InstanceMetadata data, 
            IDbCommand insertCommad,
            ObjectArchieveStorageBuilder stroageBuilder
        )
        {
            foreach ( var dicomParam in dicomParameters )
            {
                if ( dicomParam.VR == fo.DicomVR.PN )
                { 
                    List<PersonNameData> pnValues ;

                         
                    pnValues = dicomParam.GetPNValues ( ) ;
                        
                    foreach ( var values in pnValues )
                    {
                        string[] stringValues = values.ToArray ( ) ;
                        int index = -1 ;
                        List<string> pnConditions = new List<string> ( ) ;

                        foreach ( var column in SchemaProvider.GetColumnInfo ( dicomParam.KeyTag ) )
                        { 
                            column.Values = new string [] { stringValues[++index]} ;
                                
                            stroageBuilder.ProcessColumn ( column, insertCommad, Database.CreateParameter ) ;
                        }
                    }
                    
                    continue ;
                }

                
                foreach ( var column in SchemaProvider.GetColumnInfo ( dicomParam.KeyTag ) )
                { 
                    column.Values = GetValues ( dicomParam ) ;
                        
                    stroageBuilder.ProcessColumn ( column, insertCommad, Database.CreateParameter ) ;
                }
            }
        }

        protected virtual QueryBuilder CreateQueryBuilder ( ) 
        {
            return new QueryBuilder ( ) ;
        }

        protected virtual ObjectArchieveStorageBuilder CreateStorageBuilder ( ) 
        {
            return new ObjectArchieveStorageBuilder ( ) ;
        }

        protected virtual IList<string> GetValues ( IDicomDataParameter condition )
        {
            if ( condition is RangeMatching )
            {
                RangeMatching  rangeCondition  = (RangeMatching) condition ;
                fo.DicomItem dateElement     = rangeCondition.DateElement ;
                fo.DicomItem timeElement     = rangeCondition.TimeElement ;
                
                
                return GetDateTimeValues ( (fo.DicomElement) dateElement, (fo.DicomElement) timeElement ) ;
            }
            else if ( condition.VR.Equals ( fo.DicomVR.DA ) || condition.VR.Equals ( fo.DicomVR.DT ) )
            {
                fo.DicomElement dateElement = null ;
                fo.DicomElement timeElement = null ;

                foreach ( var element in condition.Elements )
                {
                    if ( element.ValueRepresentation.Equals ( fo.DicomVR.DA ) )
                    {
                        dateElement = (fo.DicomElement) element ;
                        continue ;
                    }

                    if ( element.ValueRepresentation.Equals ( fo.DicomVR.TM ) )
                    { 
                        timeElement = (fo.DicomElement) element ;
                    }
                }

                return GetDateTimeValues ( dateElement, timeElement ) ;
            }
            else
            { 
                return condition.GetValues ( ) ;
            }
        }

        private IList<string> GetDateTimeValues ( fo.DicomElement dateElement, fo.DicomElement timeElement )
        {
            List<string> values = new List<string> ( ) ; 
            int dateValuesCount = dateElement == null ? 0 : (int)dateElement.Count;
            int timeValuesCount = timeElement == null ? 0 : (int)timeElement.Count;
            int dateTimeIndex = 0;

            for (; dateTimeIndex < dateValuesCount || dateTimeIndex < timeValuesCount; dateTimeIndex++)
            {
                string dateString = null;
                string timeString = null;

                if (dateTimeIndex < dateValuesCount)
                {
                    dateString = dateElement == null || dateElement.Count == 0 ? null : dateElement.Get<string>(0); //TODO: test - original code returns "" as default
                }

                if (dateTimeIndex < dateValuesCount)
                {
                    timeString = timeElement == null || timeElement.Count == 0 ? null : timeElement.Get<string>(0); //TODO: test - original code returns "" as default
                }

                values.AddRange(GetDateTimeValues(dateString, timeString));
            }

            return values;
        }

        protected virtual IList<string> GetDateTimeValues ( string dateString, string timeString )
        {
            string date1String = null ;
            string time1String = null ;
            string date2String = null ;
            string time2String = null ;

            if ( dateString != null )
            {
                dateString = dateString.Trim();

                if (!string.IsNullOrWhiteSpace(dateString) )
                {
                    string[] dateRange = dateString.Split('-');

                    if (dateRange.Length > 0)
                    {
                        date1String = dateRange[0];
                        time1String = "";
                    }

                    if (dateRange.Length == 2)
                    {
                        date2String = dateRange[1];
                        time2String = "";
                    }
                }
            }


            if ( timeString != null )
            { 
                timeString = timeString.Trim ( ) ;

                if ( !string.IsNullOrWhiteSpace ( timeString ) )
                { 
                    string[] timeRange = timeString.Split ( '-' ) ;

                    if ( timeRange.Length > 0 )
                    { 
                        date1String = date1String ?? "" ;
                        time1String = timeRange [0 ] ; 
                    }

                    if ( timeRange.Length == 2 )
                    { 
                        date2String = date2String ?? "" ;
                        time2String = timeRange [ 1 ] ;
                    }
                }
            }
        
            return GetDateTimeQueryValues ( date1String, time1String, date2String, time2String ) ;
        }

        protected virtual IList<string> GetDateTimeQueryValues
        (
            string date1String, 
            string time1String, 
            string date2String, 
            string time2String
        )
        {
            List<string> values = new List<string> ( ) ;
            
            
            SanitizeDate ( ref date1String ) ;
            SanitizeDate ( ref date2String ) ;
            SanitizeTime ( ref time1String, true ) ;
            SanitizeTime ( ref time2String, false ) ;

            if ( string.IsNullOrEmpty (date1String) && string.IsNullOrEmpty(date2String) &&
                 string.IsNullOrEmpty (time1String) && string.IsNullOrEmpty(time2String) )
            {
                return values ;
            }

            if ( string.IsNullOrEmpty(date1String) ) 
            {
                //date should be either min or same as second
                date1String = string.IsNullOrEmpty ( date2String ) ? SqlConstants.MinDate : date2String  ;
            }

            if ( string.IsNullOrEmpty (time1String) )
            {
                time1String = string.IsNullOrEmpty ( time2String ) ? SqlConstants.MinTime : time2String ;
            }

            if ( string.IsNullOrEmpty(date2String) ) 
            {
                //date should be either min or same as second
                date2String = ( SqlConstants.MinDate == date1String ) ? SqlConstants.MaxDate : date1String ;
            }

            if ( string.IsNullOrEmpty (time2String) )
            {
                time2String = ( SqlConstants.MinTime == time1String ) ? SqlConstants.MaxTime : time1String ;
            } 

            values.Add ( date1String + " " + time1String ) ;
            values.Add ( date2String + " " + time2String ) ;
            
            return values ;
        }

        protected virtual InstanceMetadata CreateMetadata ( string columnName, object metaValue )
        {
            if( null != metaValue )
            {
                return metaValue.ToString ( ).FromJson<InstanceMetadata> ( ) ;
            }

            return null ; 
        }
                
        //TODO: currently not used any more
        protected virtual string CombineDateTime(string dateString, string timeString, bool secondInRange )
        {
            if ( string.IsNullOrWhiteSpace ( timeString ) && string.IsNullOrWhiteSpace ( dateString ) )
            {
                return ( secondInRange ) ? SqlConstants.MaxDateTime : SqlConstants.MinDateTime ;
            }

            if ( string.IsNullOrEmpty ( timeString ) )
            {
                timeString = ( secondInRange ) ? SqlConstants.MaxTime : SqlConstants.MinTime ;
            }

            if ( string.IsNullOrEmpty ( dateString ) )
            {
                dateString = ( secondInRange ) ? SqlConstants.MaxDate : SqlConstants.MinDate ;
            }
            

            return dateString + " " + timeString ;
        }

        protected virtual void SanitizeTime(ref string timeString, bool startTime )
        {
            if (null == timeString) { return ;}

            if ( string.IsNullOrEmpty ( timeString ) )
            { 
                timeString = "" ;

                return ;
            }

            if ( true )//TODO: add to config
            {
                timeString = timeString.Replace (":", "");
            }

            int length = timeString.Length ;

            if (length > "hhmm".Length) 
            {  
                timeString = timeString.Insert (4, ":") ; 
            }
            else if ( length == 4 )
            { 
                if ( startTime )
                {
                    timeString   += ":00" ;
                }
                else
                { 
                    timeString += ":59" ;
                }
            }
            
            if (timeString.Length > "hh".Length) 
            {  
                timeString = timeString.Insert (2, ":") ; 
            }
            else //it must equal
            { 
                if ( startTime )
                {
                    timeString   += ":00:00" ;
                }
                else
                { 
                    timeString += ":59:59" ;
                }
            }
            
            {//TODO: no support for fractions 
                int fractionsIndex ;

                if( ( fractionsIndex= timeString.LastIndexOf (".") ) > -1 )
                {
                    timeString = timeString.Substring ( 0, fractionsIndex ) ;
                }
            } 
        }

        protected virtual void SanitizeDate(ref string dateString )
        {
            if (null == dateString) { return ;}

            //TODO: make it a configuration option
            //a lot of dataset samples do not follow dicom standard
            //must be caled prior to IsMinDate
            if (true)
            {   
                dateString = dateString.Replace ( ".", "" ).Replace ( "-", "") ;
            }
            
            if ( !string.IsNullOrWhiteSpace ( dateString ) && !IsValidDicomDate ( dateString) )
            {
                throw new DCloudException ( "Invalid DICOM date format. Format must by yyyymmdd" ) ; 
            }

            if ( string.IsNullOrEmpty ( dateString) || IsMinDate ( dateString ) )
            { 
                dateString = "" ;

                return ;
            }

            dateString = dateString.Insert ( 6, "-") ;

            dateString = dateString.Insert ( 4, "-") ;
        }

        #endregion

        #region Private 
        private static bool IsMinDate(string dateString)
        {
            return ( DateTime.MinValue.ToShortDateString() == 
                     DateTime.ParseExact(dateString, "yyyymmdd", System.Globalization.CultureInfo.InvariantCulture).ToShortDateString());
        }

        private bool IsValidDicomDate(string dateString)
        {
            int length = dateString.Length ;
            DateTime date ;
            
            
            if (length != 8) {  return false ; }
            
            return DateTime.TryParseExact( dateString, 
                                           "yyyymmdd", 
                                           System.Globalization.CultureInfo.InvariantCulture, 
                                           System.Globalization.DateTimeStyles.None,
                                           out date ) ;
        }

        #endregion
    }
}
