using DICOMcloud.DataAccess.Database.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DICOMcloud.DataAccess.Database
{
    static class SqlInsertStatments
    {
        public static string GetTablesKey ( TableKey table )
        { 
            return string.Format ( TablesKeyFormatted, table.KeyColumn.Name ) ;
        }

        public static string GetTablesKeysParams ( )
        { 
            return TablesKeysParams ;
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
         /// <summary>
         /// This method is a replacment for the hard coded InsertIntoPatient/InsertInfoStudy... methods!
         /// </summary>
         /// <param name="table"></param>
         /// <returns></returns>
        public static string GetInsertIntoTable 
        ( 
            TableKey table
        )
        { 
            IList<ColumnInfo> whereColumns = table.ModelKeyColumns ;
            string [] conditions           = new string [ whereColumns.Count ] ;
            string    newPrimaryParam      = "@New" + table.KeyColumn.Name ;
            string    primaryColumn        =  wrap (table.KeyColumn.Name) ;
            string    tableName            = wrap (table.Name) ;
            string    whereColumnsString   = "" ;
            string    columns              = "{0}" ;
            string    values               = "{1}" ;
            
            if ( table.ForeignColumn != null )
            { 
                columns = table.ForeignColumn.Name + ", " + columns ;
                values  = "@New" + table.Parent.KeyColumn.Name + ", " + values ;
            }

            for ( int index = 0; index < whereColumns.Count; index++ )
            { 
                ColumnInfo column = whereColumns[index] ;

                if ( column.IsForeign )
                {
                    conditions [index] = wrap(column.Name) + " = @New" + table.Parent.KeyColumn.Name ;
                }
                else
                {
                    conditions [index] = wrap(column.Name) + " = @" + column.Name ;
                }
            }

            whereColumnsString = string.Join ( " AND ", conditions ) ;

            if ( string.IsNullOrEmpty ( whereColumnsString ))
            {
                return string.Format ( InsertTableFormattedNoPrimaryKey, newPrimaryParam, tableName, columns, values )  ;
            }
            else
            {
                return string.Format ( InsertTableFormatted, newPrimaryParam, primaryColumn, tableName, whereColumnsString, columns, values )  ;            
            }

        }

        private static string wrap(string entityName)
        {
            return "[" + entityName + "]" ;
        }
        
        
        public static string GetInsertIntoTable ( string tableName )
        {
            switch ( tableName )
            { 
                case PatientTableName:
                {
                    return GetInsertIntoPatient ( ) ;
                }

                case StudyTableName:
                {
                    return GetInsertIntoStudy ( ) ;
                }

                case SeriesTableName:
                {
                    return GetInsertIntoSeries ( ) ;
                }

                case ObjectInstanceTableName:
                {
                    return GetInsertIntoObjectInstance ( ) ;
                }
            }

            return "" ; //TODO: throw exception!
        }

        public static string GetInsertIntoPatient ( )
        { 
            return InsertPatientFormatted ;
        }

        public static string GetInsertIntoStudy ( )
        { 
            return InsertStudyFormatted ;
        }

        public static string GetInsertIntoSeries ( )
        { 
            return InsertSeriesFormatted ;
        }

        public static string GetInsertIntoObjectInstance ( )
        { 
            return InsertObjectInstanceFormatted ;
        }

        //These has been replaced now by the method GetInsertIntoTable with TableKey :)
        const string PatientTableName        = "Patient" ;
        const string StudyTableName          = "Study" ;
        const string SeriesTableName         = "Series" ;
        const string ObjectInstanceTableName = "ObjectInstance" ;

        const string TablesKeyFormatted = 
@"DECLARE @New{0} BIGINT" ;

        const string TablesKeysParams = 
@"
DECLARE @NewPatientKey BIGINT
DECLARE @NewStudyKey BIGINT
DECLARE @NewSeriesKey BIGINT
DECLARE @NewInstanceKey BIGINT
" ;


            const string InsertTableFormatted = 
@"
SET {0} = (SELECT {1} FROM {2} where {3} )
IF {0} is NULL
BEGIN
	INSERT INTO {2} ({4})
	VALUES ( {5} )

	Select {0} = SCOPE_IDENTITY()
END
" ;

            const string InsertTableFormattedNoPrimaryKey = 
@"
BEGIN
	INSERT INTO {1} ({2})
	VALUES ( {3} )

	Select {0} = SCOPE_IDENTITY()
END
" ;


            const string InsertPatientFormatted = 
@"
SET @NewPatientKey = (SELECT [PatientKey] FROM [Patient] where [PatientId] = @PatientId)
IF @NewPatientKey is NULL
BEGIN
	INSERT INTO [Patient] ({0})
	VALUES ( {1} )

	Select @NewPatientKey = SCOPE_IDENTITY()
END
" ;
    
        const string InsertStudyFormatted = 
@"
SET @NewStudyKey = (SELECT [StudyKey] from  [Study] where [StudyInstanceUid] = @StudyInstanceUid)
IF (@NewStudyKey is NULL)
BEGIN
	INSERT INTO [Study] ([Study_PatientKey],{0})
	VALUES ( @NewPatientKey,{1} )

	SELECT @NewStudyKey = SCOPE_IDENTITY()
END
" ;

        const string InsertSeriesFormatted = 
@"
SET @NewSeriesKey = (SELECT [SeriesKey] from  [Series] where [SeriesInstanceUid] = @SeriesInstanceUid)
IF (@NewSeriesKey is NULL)
BEGIN
	INSERT INTO [Series] ([Series_StudyKey], {0})
	VALUES ( @NewStudyKey, {1} )

	SELECT @NewSeriesKey = SCOPE_IDENTITY()
END
" ;
        const string InsertObjectInstanceFormatted = 
@"
SET @NewInstanceKey = (SELECT [ObjectInstanceKey] from  [ObjectInstance] where [SopInstanceUid] = @SopInstanceUid)
IF (@NewInstanceKey is NULL)
BEGIN
    INSERT INTO [ObjectInstance] ( [ObjectInstance_SeriesKey], {0} )
    VALUES (@NewSeriesKey, {1})

    SELECT @NewInstanceKey = SCOPE_IDENTITY()
END
" ;

        public const string BeginTransaction = "begin transaction" ;
        public const string CommitTransaction = "commit transaction" ;
    }
}
