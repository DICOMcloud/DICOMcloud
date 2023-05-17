using System.Collections.Generic;
using System.Data;
using System.Linq;
using DICOMcloud.DataAccess.Database.Schema;
using Dicom;

namespace DICOMcloud.DataAccess.Database.Commands
{
    public class DicomDsQueryCommand : IPagedDataAdapterCommand<DicomDataset>
    {
        public IDbCommand Command                    { get; set; }
        public QueryBuilder QueryBuilder             { get; set; }
        public IQueryResponseBuilder ResponseBuilder { get; set; }
        public IQueryOptions QueryOptions            { get; set; }


        public DicomDsQueryCommand
        (
            IDbCommand command,
            QueryBuilder queryBuilder,
            IQueryResponseBuilder responseBuilder
        ) : this ( command, queryBuilder, responseBuilder, null)
        {
        }

        public DicomDsQueryCommand
        (
            IDbCommand command,
            QueryBuilder queryBuilder,
            IQueryResponseBuilder responseBuilder,
            IQueryOptions options
        )
        {
            Command         = command;
            QueryBuilder    = queryBuilder;
            ResponseBuilder = responseBuilder;
            QueryOptions    = options;
            ApplyPagination = false;
        }

        public IEnumerable<DicomDataset> Result => ResponseBuilder.GetResponse ( ) ;

        public bool Execute()
        {
            Command.Connection.Open();

            try
            {
                
                using (var reader = Command.ExecuteReader())
                {
                    bool maxReached = false ;
                    List<object> queryLevelResultsKey = new List<object> ();

                    while (reader.Read())
                    {
                        foreach (var table in QueryBuilder.ProcessedColumns)
                        {
                            object keyValue = reader.GetValue(reader.GetOrdinal ( table.Key.KeyColumn.Name ) );
                            

                            if (ResponseBuilder.ResultExists ( table.Key.Name, keyValue )) 
                            { 
                                continue ; 
                            }

                            if (ShouldPaginate ( ) && table.Key.Name == ResponseBuilder.QueryLevelTableName)
                            {
                                // if this is true then we alerady have a full page 
                                // and we are skipping adding to ResponseBuilder, 
                                // we should also skip here too
                                if ( queryLevelResultsKey.Contains(keyValue))
                                {
                                    break;
                                }

                                queryLevelResultsKey.Add(keyValue);

                                if (QueryOptions.Offset >= queryLevelResultsKey.Count)
                                { 
                                    break;
                                }

                                // Now we should have enough responses
                                if (queryLevelResultsKey.Count > (QueryOptions.Offset + QueryOptions.Limit))
                                {
                                    System.Diagnostics.Debug.Assert (QueryOptions.Limit == ResponseBuilder.GetResponse().Count());

                                    break;
                                }
                            }

                            ResponseBuilder.BeginResultSet ( table.Key.Name ) ;
                            ResponseBuilder.BeginRead();

                            foreach ( var column in table.Value )
                            {
                                object value = reader.GetValue(reader.GetOrdinal ( column ) );

                                ResponseBuilder.ReadData(table.Key.Name, column, value);
                            }
                             
                            if ( null != CountColumnName && null != CountColumnTable && 
                                 string.Compare (CountColumnTable, table.Key.Name, true) == 0 )
                            {
                                TotalCount = reader.GetInt32 (reader.GetOrdinal (CountColumnName)) ;
                            }

                            ResponseBuilder.EndRead ( ) ;
                            ResponseBuilder.EndResultSet ( ) ;
                        }

                        if (ShouldPaginate() && maxReached)
                        {
                            break;
                        }
                    }
                    
                    if (ApplyPagination)
                    {
                        TotalCount = queryLevelResultsKey.Count;
                    }
                }

                return true ;
            }
            finally
            {
                if (Command.Connection.State == System.Data.ConnectionState.Open)
                {
                    Command.Connection.Close();
                }
            }
        }
        
        private bool ShouldPaginate ( )
        { 
            return (ApplyPagination && QueryOptions != null && 
                    QueryOptions .Limit.HasValue && QueryOptions.Offset.HasValue  && QueryOptions.Limit > 0 );
        }
        
        public string CountColumnTable { get; set ; }
        public string CountColumnName { get; set ; }
        public bool ApplyPagination { get; set; }

        public int? TotalCount { get; private set ; }

        public void SetCountColumn ( string tableName, string columnName )
        {
            CountColumnTable = tableName ;
            CountColumnName  = columnName ;
        }
    }
}


