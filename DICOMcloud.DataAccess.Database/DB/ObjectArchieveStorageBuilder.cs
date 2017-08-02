using DICOMcloud;
using DICOMcloud.DataAccess.Database.Schema;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace DICOMcloud.DataAccess.Database
{
    public class ObjectArchieveStorageBuilder
    {
        public IList<System.Data.IDbDataParameter> Parameters { get; protected set; }
        public string InsertString {  get ; protected set ; }

        public ObjectArchieveStorageBuilder ( ) 
        {
            Parameters = new List<System.Data.IDbDataParameter> ( ) ;
        }
        
        public virtual string GetInsertText()
        {
            StringBuilder result = new StringBuilder ( SqlInsertStatments.BeginTransaction ) ;
            
            result.AppendLine ( ) ;

            foreach ( var insertKeyValue in _tableToInsertStatments )
            {
                var insert = insertKeyValue.Value ;

                string columns = string.Join ( ", ", insert.ColumnNames ) ;
                string values  = string.Join ( ", ", insert.ParametersValueNames ) ;
            
                result.AppendLine ( SqlInsertStatments.GetTablesKey ( insertKeyValue.Key ) ) ;
                result.AppendFormat ( insert.InsertTemplate, columns, values ) ;
                result.AppendLine ( ) ;
            }

            result.AppendLine ( SqlInsertStatments.CommitTransaction ) ;

            return result.ToString ( ) ;
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
            InsertSections insert = GetTableInsert ( column.Table ) ;

            insert.ColumnNames.Add ( column.Name ) ;
            insert.ParametersValueNames.Add ( "@" + column.Name ) ; //TODO: add a parameter name to the column class
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
            
            param = parameterFactory ( "@" + column.Name, value ) ;
            
            Parameters.Add ( param ) ;
            
            if ( null != insertCommand )
            { 
                insertCommand.Parameters.Add ( param ) ;
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

            result.InsertTemplate = SqlInsertStatments.GetInsertIntoTable ( table ) ;
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
