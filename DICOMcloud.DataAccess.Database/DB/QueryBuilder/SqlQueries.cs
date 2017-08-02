using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DICOMcloud.DataAccess.Database
{
    public partial class QueryBuilder
    { 
        private void AppendSelectGroupBy ( string from, string returns, StringBuilder sb )
        {
            sb.AppendFormat ( SqlQueries.Select_GroupBy, returns, from ) ;
        }

        private void AppendSelectKeyColumnRange ( string keyColumn, string from, string returns, StringBuilder sb  )
        {
            sb.AppendFormat ( SqlQueries.Select_KeyColumn, returns, from, keyColumn ) ;
        }

        private void AppendDeclareTableParam ( string paramName, string columns, StringBuilder sb )
        {
            sb.AppendFormat ( SqlQueries.DeclareTableParam_Formatted, paramName, columns ) ;
        }
    }
    
    class SqlQueries
    {
        public static readonly string Table_Column_Formatted   = "[{0}].[{1}]" ;
        public static readonly string Select_Command_Formatted = "SELECT {0} FROM {1} {2} WHERE 1=1 {3}" ;
        public static readonly string Select_GroupBy = "SELECT {0} FROM {1} GROUP BY {0}\n\r" ;
        public static readonly string Select_KeyColumn = "SELECT {0} FROM {1} WHERE {2} in (SELECT Distinct({2}) {2} FROM {1})\n\r" ;
        public static readonly string Table_Column_NumberCondition_Formatted = "[{0}].[{1}]={2}" ;
        public static readonly string Table_Column_TextCondition_Formatted = "[{0}].[{1}]='{2}'" ;

        public static readonly string DeclareTableParam_Formatted = "DECLARE {0} Table ( {1} );" ;


        public class Joins
        {
            //public const string StudyToPatient = @"INNER JOIN [Patient] on [Study].[Study_PatientKey] = [Patient].[PatientKey]";
            //public const string SeriesToStudy  = @"INNER JOIN [Study] on [Series].[Series_StudyKey] = [Study].[StudyKey]";
            //public const string ObjectToSeries = @"INNER JOIN [Series] on [ObjectInstance].[ObjectInstance_SeriesKey] = [Series].[SeriesKey]";
            
            //{0}=Patient (parent/destination)
            //{1}=Study (child/source)
            //{2}=Study_PatientKey (child foriegn)
            //{3}=PatientKey (parent foriegn)

            public const string JoinFormattedTemplate = @"INNER JOIN [{0}] on [{1}].[{2}] = [{0}].[{3}]";

            public const string OuterJoinFormattedTemplate = @"LEFT OUTER JOIN [{0}] on [{1}].[{2}] = [{0}].[{3}]";
        }
    }
}
