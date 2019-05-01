using DICOMcloud;
using DICOMcloud.DataAccess.Database.Schema;
using DICOMcloud.DataAccess.Database.SQL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace DICOMcloud.DataAccess.Database
{
    public class ObjectArchieveStorageBuilder
    {
        public ISQLStatementsProvider SQLStatementsProvider { get; private set; }
        public IList<System.Data.IDbDataParameter> Parameters { get; protected set; }
        public string InsertString {  get ; protected set ; }

        public ObjectArchieveStorageBuilder (ISQLStatementsProvider sqlStatementsProvider) 
        {
            SQLStatementsProvider = sqlStatementsProvider;
            Parameters = new List<System.Data.IDbDataParameter> ( ) ;
        }
        
        public virtual void SetInsertText ( IDbCommand cmd )
        {
            StringBuilder result = new StringBuilder () ;
            
            result.AppendLine (SQLStatementsProvider.GeneralStatementsProvider.BeginTransaction);

            result.AppendLine ( ) ;

            foreach ( var insertKeyValue in _tableToInsertStatments )
            {
                var insert = insertKeyValue.Value ;

                result.AppendLine (SQLStatementsProvider.InsertUpdateStatementsProvider.GetDeclareForeignStatement( insertKeyValue.Key ) ) ;
                
                result.AppendLine(SQLStatementsProvider.InsertUpdateStatementsProvider.FormatInsertIntoTable(insert.InsertTemplate, insert.ColumnNames, insert.ParametersValueNames)) ;
            }

            result.AppendLine (SQLStatementsProvider.GeneralStatementsProvider.CommitTransaction ) ;

            cmd.CommandText = result.ToString ( ) ;
        }

        public void BuildInsertOrUpdateMetadata ( ObjectId instance, IDbCommand insertCommand )
        {
            
        }

        public virtual void ProcessColumn
        (
            ColumnInfo column, 
            IDbCommand insertCommand,
            DataParamFactory parameterFactory
        )
        {
            if (null == column.Values || column.Values.Count == 0 || null == column.Values[0]) return;

            InsertSections insert = GetTableInsert ( column.Table ) ;

            insert.ColumnNames.Add ( SQLStatementsProvider.GeneralStatementsProvider.WrapColumn(column.Name) ) ;
            insert.ParametersValueNames.Add (SQLStatementsProvider.GeneralStatementsProvider.GetParameterName(column.Name) ) ; 
            //TODO: add a parameter name to the column class
            //insert.ParametersValueNames.Add ( column.Values[0] ) ;


            System.Data.IDbDataParameter param ;
            
            object value = DBNull.Value ;
            
            if ( null != column.Values && column.Values.Count != 0 )
            {
                //TODO: multivalue must be stored in their own table where each value is a row
                //in order to support proper query
                value = column.Values[0] ;
                            
                if ( null == value )
                { 
                    value = DBNull.Value ;
                }
            }
            
            param = parameterFactory ( SQLStatementsProvider.GeneralStatementsProvider.GetParameterName(column.Name), value ) ;
            
            Parameters.Add ( param ) ;
            
            if ( null != insertCommand )
            { 
                if (insertCommand is MySql.Data.MySqlClient.MySqlCommand)
                {
                    ((MySql.Data.MySqlClient.MySqlCommand)insertCommand).Parameters.AddWithValue(SQLStatementsProvider.GeneralStatementsProvider.GetParameterName(column.Name), value);
                }
                else
                { 
                    insertCommand.Parameters.Add ( param ) ;
                }
            }
        }

        public delegate IDbDataParameter DataParamFactory ( string columnName, object Value ) ; 
        
        protected InsertSections GetTableInsert ( TableKey table )
        { 
            InsertSections result = null ;

            if ( _tableToInsertStatments.TryGetValue ( table, out result) )
            { 
                return result ;
            }


            result = new InsertSections ( ) ;

            result.InsertTemplate = SQLStatementsProvider.InsertUpdateStatementsProvider.GetInsertIntoTableFormatted ( table ) ;
            result.TableName      = table.Name ;

            _tableToInsertStatments.Add ( table, result ) ;
        
            return result ;
        }


        SortedDictionary<TableKey,InsertSections> _tableToInsertStatments = new SortedDictionary<TableKey,InsertSections> ( ) ;
        

    }

    public class InsertSections
    {
        public InsertSections ( )
        { 
            ColumnNames = new List<string> (  ) ;
            ParametersValueNames = new List<string> ( ) ;
        }

        public string TableName                  { get; set;  }
        public List<string> ColumnNames          { get; set; }
        public List<string> ParametersValueNames { get; set; }
        public string       InsertTemplate       { get; set; }
    }
}
