using DICOMcloud.DataAccess.Database.Schema;
using System.Collections.Generic;

namespace DICOMcloud.DataAccess.Database
{
    public partial class SqlDeleteStatments
    {
        public string GetDeleteInstanceCommandText 
        ( 
            long sopInstanceKey
        ) 
        {
            return DeleteInstance.DeleteText ( 
                                   SchemaProvider.ObjectInstanceTable.KeyColumn.Name, 
                                   SchemaProvider.SeriesTable.KeyColumn.Name, 
                                   SchemaProvider.StudyTable.KeyColumn.Name, 
                                   SchemaProvider.PatientTable.KeyColumn.Name,
                                   SchemaProvider.ObjectInstanceTable.Name, 
                                   SchemaProvider.SeriesTable.Name, 
                                   SchemaProvider.StudyTable.Name, 
                                   SchemaProvider.PatientTable.Name,
                                   SchemaProvider.ObjectInstanceTable.ForeignColumn.Name,
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

        static class DeleteInstance
        {
            static Dictionary<int, string> InstanceStatementAliasNames = new Dictionary<int, string> ( ) { 
                { 0, "sopKey" }, //0
                { 1, "seriesKey" }, //1
                { 2, "studyKey" }, //2
                { 3, "patientKey" }, //3
                { 4, "sopTableName" }, //4
                { 5, "seriesTableName" }, //5
                { 6, "studyTableName" }, //6
                { 7, "patientTableName" }, //7
                { 8, "sopInstanceRefSeriesColName" }, //8
                { 9, "seriesRefStudyColName" }, //9
                { 10, "studyRefPatientColName" }, //10
                } ;


            static DeleteInstance ( )
            {
                Delete_Instance_Command_Formatted = __delete_Instance_Command ;

                foreach ( var alias in InstanceStatementAliasNames )
                {
                    string stringReplaced = "{" + alias.Value + "}" ;
                    string indexReplacing = "{" + alias.Key + "}" ;
                     
                    Delete_Instance_Command_Formatted =  Delete_Instance_Command_Formatted.Replace ( stringReplaced, indexReplacing ) ;
                }

            }

            public static string DeleteText
            ( 
                string objectInstanceTableKeyColumnName, 
                string seriesTableKeyColumnName, 
                string studyTableKeyColumnName, 
                string patientTableKeyColumnName,
                string objectInstanceTableName,
                string seriesTableName,
                string studyTableName,
                string patientTableName,
                string objectInstanceTableForeignColumnName,
                ColumnInfo seriesTableForeignColumn,
                ColumnInfo studyTableForeignColumn,
                long sopInstanceKey
            ) 
            {
                return string.Format ( DeleteInstance.Delete_Instance_Command_Formatted,
                                       objectInstanceTableKeyColumnName, 
                                       seriesTableKeyColumnName, 
                                       studyTableKeyColumnName, 
                                       patientTableKeyColumnName,
                                       objectInstanceTableName,
                                       seriesTableName, 
                                       studyTableName,
                                       patientTableName,
                                       objectInstanceTableForeignColumnName,
                                       seriesTableForeignColumn,
                                       studyTableForeignColumn,
                                       sopInstanceKey ) ;
            }

            public static readonly string Delete_Instance_Command_Formatted ;
            private static readonly string __delete_Instance_Command =            
            @"
declare @sop bigint
declare @series bigint
declare @study bigint
declare @patient bigint
declare @sopCount int
declare @seriesCount int
declare @studyCount int

SELECT @sop = instance.{sopKey}, @series = ser.{seriesKey}, @study = stud.{studyKey}, @patient = p.{patientKey}
FROM {sopTableName} instance, {seriesTableName} ser, {studyTableName} stud, {patientTableName} p
where 
    instance.{sopInstanceRefSeriesColName} = ser.{seriesKey} AND 
    Ser.{seriesRefStudyColName} = stud.{studyKey} AND
    stud.{studyRefPatientColName} = p.{patientKey} AND
    instance.{sopKey} = {11}

Delete from {sopTableName} where {sopTableName}.{sopKey} = @sop

SELECT @sopCount = COUNT(*)
FROM {sopTableName} instance
WHERE instance.{sopInstanceRefSeriesColName} = @series

/* if 0 entries, remove orphaned' series record */
IF (@sopCount = 0) 
DELETE FROM {seriesTableName} WHERE (@series = {seriesTableName}.{seriesKey});

SELECT  @seriesCount = COUNT(*) FROM {seriesTableName} ser
WHERE ser.{seriesRefStudyColName} = @study

/* if 0 entries, remove orphaned study record */
IF (@seriesCount = 0) 
DELETE FROM {studyTableName} WHERE (@study = {studyTableName}.{studyKey});

SELECT  @studyCount = COUNT(*) FROM {studyTableName} stud
WHERE stud.{studyRefPatientColName} = @patient

/* if 0 entries, remove orphaned patient record */
IF (@studyCount = 0) 
DELETE FROM {patientTableName} WHERE (@patient = {patientTableName}.{patientKey});
            ";
        }
    }
}
