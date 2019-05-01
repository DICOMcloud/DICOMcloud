using DICOMcloud.DataAccess.Database.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DICOMcloud.DataAccess.Database.SQL
{
    public abstract class InsertStatementsProvider
    {
        public InsertStatementsProvider(IGeneralStatementsProvider generalStatementsProvider)
        {
            GeneralStatementsProvider = generalStatementsProvider;
        }

        public string GetTableKeyForeign(TableKey table)
        {
            return GeneralStatementsProvider.GetVariableName("New" + table.KeyColumn.Name);
        }

        public string GetDeclareForeignStatement ( TableKey table )
        {
            var varName = GetTableKeyForeign (table);
            
            return GeneralStatementsProvider.GetDeclareVariable (varName, "BIGINT");
        }

        /*
        {0} = @NewPatientKey
        {1} = [PatientKey]
        {2} = [Patient]
        {3} = [PatientId]
        {4} = @PatientId
        {5} = columns
        {6} = values
         */

        public string GetInsertIntoTableFormatted
        ( 
            TableKey table
        )
        {
            IList<ColumnInfo> whereColumns = table.ModelKeyColumns ;
            string [] conditions           = new string [ whereColumns.Count ] ;
            string    newPrimaryParam      = GeneralStatementsProvider.GetVariableName("New" + table.KeyColumn.Name) ;
            string    primaryColumn        = GeneralStatementsProvider.WrapColumn(table.KeyColumn.Name) ;
            string    tableName            = GeneralStatementsProvider.WrapTable(table.Name) ;
            string    whereColumnsString   = "" ;
            string    columns              = "{0}" ;
            string    values               = "{1}" ;
            
            if ( table.ForeignColumn != null )
            { 
                columns = GeneralStatementsProvider.WrapColumn(table.ForeignColumn.Name) + ", " + columns ;
                values  = GeneralStatementsProvider.GetVariableName ("New" + table.Parent.KeyColumn.Name) + ", " + values ;
            }

            for ( int index = 0; index < whereColumns.Count; index++ )
            { 
                ColumnInfo column = whereColumns[index] ;

                if ( column.IsForeign )
                {
                    conditions [index] = GeneralStatementsProvider.WrapColumn(column.Name) + " = " + GeneralStatementsProvider.GetVariableName("New" + table.Parent.KeyColumn.Name) ;
                }
                else
                {
                    conditions [index] = GeneralStatementsProvider.WrapColumn(column.Name) + " = " + GeneralStatementsProvider.GetParameterName(column.Name) ;
                }
            }

            whereColumnsString = string.Join ( " AND ", conditions ) ;

            if ( string.IsNullOrEmpty ( whereColumnsString ))
            {
                return GetInsertTableFormattedNoPrimaryKey (newPrimaryParam, tableName, columns, values );
            }
            else
            {
                return GetInsertTableFormatted (newPrimaryParam, primaryColumn, tableName, whereColumnsString, columns, values );
            }

        }

        public abstract string FormatInsertIntoTable
        (
            string formatedInsert,
            IEnumerable<string> columns,
            IEnumerable<string> values
        );

        protected abstract string GetInsertTableFormatted 
        (
            string newPrimaryParam, 
            string primaryColumn, 
            string tableName, 
            string whereColumnsString, 
            string columns, 
            string values 
        );

        protected abstract string GetInsertTableFormattedNoPrimaryKey 
        ( 
            string newPrimaryParam, 
            string tableName, 
            string columns, 
            string values 
        );

        public IGeneralStatementsProvider GeneralStatementsProvider { get; }
    }
}
