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
        public string GetDeleteStudyCommandText 
        ( 
            long studyInstanceKey
        ) 
        {
            return DeleteStudy.DeleteText ( 
                                   SchemaProvider.StudyTable.KeyColumn.Name, 
                                   SchemaProvider.PatientTable.KeyColumn.Name,
                                   SchemaProvider.StudyTable.Name, 
                                   SchemaProvider.PatientTable.Name,
                                   SchemaProvider.StudyTable.ForeignColumn,
                                   studyInstanceKey ) ;
        }


        static class DeleteStudy
        {
            static Dictionary<int, string> StudyStatementAliasNames = new Dictionary<int, string> ( ) { 
                { 0, "studyKey" }, 
                { 1, "patientKey" }, 
                { 2, "studyTableName" }, 
                { 3, "patientTableName" }, 
                { 4, "studyRefPatientColName" }, 
                } ;


            static DeleteStudy ( )
            {
                foreach ( var alias in StudyStatementAliasNames )
                {
                    string stringReplaced = "{" + alias.Value + ":" + alias.Key + "}" ;
                    string indexReplacing = "{" + alias.Key + "}" ;
                     
                    Delete_Study_Command_Formatted =  Delete_Study_Command_Formatted.Replace ( stringReplaced, indexReplacing ) ;
                }

            }

            public static string DeleteText
            ( 
                string studyTableKeyColumnName, 
                string patientTableKeyColumnName,
                string studyTableName,
                string patientTableName,
                ColumnInfo studyTableForeignColumn,
                long studyInstanceKey
            ) 
            {
                return string.Format ( DeleteStudy.Delete_Study_Command_Formatted,
                                       studyTableKeyColumnName, 
                                       patientTableKeyColumnName,
                                       studyTableName,
                                       patientTableName,
                                       studyTableForeignColumn,
                                       studyInstanceKey ) ;
            }

            public static readonly string Delete_Study_Command_Formatted =
            @"
declare @study bigint
declare @patient bigint
declare @studyCount int

SELECT @study = stud.{studyKey:0}, @patient = p.{patientKey:1}
FROM {studyTableName:2} stud, {patientTableName:3} p
where 
    stud.{studyRefPatientColName:4} = p.{patientKey:1} AND
    stud.{studyKey:0} = {5}
    
Delete from {studyTableName:2} where {studyTableName:2}.{studyKey:0} = @study

SELECT  @studyCount = COUNT(*) FROM {studyTableName:2} stud
WHERE stud.{studyRefPatientColName:4} = @patient

/* if 0 entries, remove orphaned patient record */
IF (@studyCount = 0) 
DELETE FROM {patientTableName:3} WHERE (@patient = {patientTableName:3}.{patientKey:1});
            ";
        }
    }
}