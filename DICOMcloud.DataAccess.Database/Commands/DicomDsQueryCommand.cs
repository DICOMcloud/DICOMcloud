using System.Collections.Generic;
using System.Data;
using DICOMcloud.DataAccess.Database.Schema;
using Dicom;

namespace DICOMcloud.DataAccess.Database.Commands
{
    public class DicomDsQueryCommand : IPagedDataAdapterCommand<DicomDataset>
    {
        public IDbCommand Command { get; set; }
        public QueryBuilder QueryBuilder { get; set; }
        public IQueryResponseBuilder ResponseBuilder { get; set; }

        public DicomDsQueryCommand
        (
            IDbCommand command,
            QueryBuilder queryBuilder,
            IQueryResponseBuilder responseBuilder
        )
        {
            Command         = command;
            QueryBuilder    = queryBuilder ;
            ResponseBuilder = responseBuilder ;
        }

        public IEnumerable<DicomDataset> Result => ResponseBuilder.GetResponse ( ) ;

        public bool Execute()
        {
            Command.Connection.Open();

            try
            {
                
                using (var reader = Command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        foreach (var table in QueryBuilder.ProcessedColumns)
                        {
                            object keyValue = reader.GetValue(reader.GetOrdinal ( table.Key.KeyColumn.Name ) );
                            

                            if ( ResponseBuilder.ResultExists ( table.Key.Name, keyValue ) ) 
                            { 
                                continue ; 
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
        
        public string CountColumnTable { get; set ; }
        public string CountColumnName { get; set ; }

        public int? TotalCount { get; private set ; }

        public void SetCountColumn ( string tableName, string columnName )
        {
            CountColumnTable = tableName ;
            CountColumnName  = columnName ;
        }
    }
}


