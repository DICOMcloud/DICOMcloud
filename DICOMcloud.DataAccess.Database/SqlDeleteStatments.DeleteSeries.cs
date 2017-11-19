using DICOMcloud.DataAccess.Database.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DICOMcloud.DataAccess.Database
{
    public partial class SqlDeleteStatments
    {
        public string GetDeleteSeriesCommandText 
        ( 
            long sopInstanceKey
        ) 
        {
            return DeleteSeries.DeleteText ( 
                                   SchemaProvider.SeriesTable.KeyColumn.Name, 
                                   SchemaProvider.StudyTable.KeyColumn.Name, 
                                   SchemaProvider.PatientTable.KeyColumn.Name,
                                   SchemaProvider.SeriesTable.Name, 
                                   SchemaProvider.StudyTable.Name, 
                                   SchemaProvider.PatientTable.Name,
                                   SchemaProvider.SeriesTable.ForeignColumn,
                                   SchemaProvider.StudyTable.ForeignColumn,
                                   sopInstanceKey ) ;
        }


        //public static string GetDeleteSeriesCommandText ( long seriesKey )
        //{
        //    throw new NotImplementedException ( );
        //}

        //public static string GetDeleteStudyCommandText ( long studyKey )
        //{
        //    throw new NotImplementedException ( );
        //}

        static class DeleteSeries
        {
            static Dictionary<int, string> SeriesStatementAliasNames = new Dictionary<int, string> ( ) { 
                { 0, "seriesKey" }, 
                { 1, "studyKey" }, 
                { 2, "patientKey" }, 
                { 3, "seriesTableName" }, 
                { 4, "studyTableName" }, 
                { 5, "patientTableName" }, 
                { 6, "seriesRefStudyColName" }, 
                { 7, "studyRefPatientColName" }, 
                } ;


            static DeleteSeries ( )
            {
                foreach ( var alias in SeriesStatementAliasNames )
                {
                    string stringReplaced = "{" + alias.Value + ":" + alias.Key + "}" ;
                    string indexReplacing = "{" + alias.Key + "}" ;
                     
                    Delete_Series_Command_Formatted =  Delete_Series_Command_Formatted.Replace ( stringReplaced, indexReplacing ) ;
                }

            }

            public static string DeleteText
            ( 
                string seriesTableKeyColumnName, 
                string studyTableKeyColumnName, 
                string patientTableKeyColumnName,
                string seriesTableName,
                string studyTableName,
                string patientTableName,
                ColumnInfo seriesTableForeignColumn,
                ColumnInfo studyTableForeignColumn,
                long seriesInstanceKey
            ) 
            {
                return string.Format ( DeleteSeries.Delete_Series_Command_Formatted,
                                       seriesTableKeyColumnName, 
                                       studyTableKeyColumnName, 
                                       patientTableKeyColumnName,
                                       seriesTableName, 
                                       studyTableName,
                                       patientTableName,
                                       seriesTableForeignColumn,
                                       studyTableForeignColumn,
                                       seriesInstanceKey ) ;
            }

            public static readonly string Delete_Series_Command_Formatted =
            @"
declare @series bigint
declare @study bigint
declare @patient bigint
declare @seriesCount int
declare @studyCount int

SELECT @series = ser.{seriesKey:0}, @study = stud.{studyKey:1}, @patient = p.{patientKey:2}
FROM {seriesTableName:3} ser, {studyTableName:4} stud, {patientTableName:5} p
where 
    ser.{seriesRefStudyColName:6} = stud.{studyKey:1} AND
    stud.{studyRefPatientColName:7} = p.{patientKey:2} AND
    ser.{seriesKey:0} = {8}

Delete from {seriesTableName:3} where {seriesTableName:3}.{seriesKey:0} = @series

SELECT  @seriesCount = COUNT(*) FROM {seriesTableName:3} ser
WHERE ser.{seriesRefStudyColName:6} = @study

/* if 0 entries, remove orphaned study record */
IF (@seriesCount = 0) 
DELETE FROM {studyTableName:4} WHERE (@study = {studyTableName:4}.{studyKey:1});

SELECT  @studyCount = COUNT(*) FROM {studyTableName:4} stud
WHERE stud.{studyRefPatientColName:7} = @patient

/* if 0 entries, remove orphaned patient record */
IF (@studyCount = 0) 
DELETE FROM {patientTableName:5} WHERE (@patient = {patientTableName:5}.{patientKey:2});
            ";
        }
    }
}