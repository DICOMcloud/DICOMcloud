using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DICOMcloud.DataAccess.Database
{
    public partial class QueryBuilder
    { 
    }
    
    class SqlQueries
    {
        public static readonly string Table_Column_Formatted   = "[{0}].[{1}]" ;
        public static readonly string Select_Command_Formatted = "SELECT {0} FROM {1} {2} WHERE 1=1 {3}" ;
        
        public class Joins
        {
            //{0}=Patient (parent/destination)
            //{1}=Study (child/source)
            //{2}=Study_PatientKey (child foriegn)
            //{3}=PatientKey (parent foriegn)

            public const string JoinFormattedTemplate = @"INNER JOIN [{0}] on [{1}].[{2}] = [{0}].[{3}]";

            public const string OuterJoinFormattedTemplate = @"LEFT OUTER JOIN [{0}] on [{1}].[{2}] = [{0}].[{3}]";
        }
    }
}
