namespace DICOMcloud.DataAccess.Database.SQL
{
    public class SelectStatementsProvider : ISelectStatementsProvider
    {
        public SelectStatementsProvider (IGeneralStatementsProvider generalStatementsProvider )
        {
            GeneralStatementsProvider = generalStatementsProvider;
        }

        public IGeneralStatementsProvider GeneralStatementsProvider { get; private set; }

        public string GetSelectStatement
        ( 
            string columnsSelectPart, 
            string tableFromPart, 
            string joinsPart, 
            string conditionsPart
        )
        { 
            if (!string.IsNullOrWhiteSpace (conditionsPart) && conditionsPart.TrimStart().IndexOf("and", System.StringComparison.InvariantCultureIgnoreCase) != 0)
            { 
                conditionsPart = "AND " + conditionsPart;
            }

            return $"SELECT {columnsSelectPart} FROM {tableFromPart} {joinsPart} WHERE 1=1 {conditionsPart}";
        }

        /// <summary>
        /// {0}=Patient (parent/destination)
        /// {1}=Study (child/source)
        /// {2}=Study_PatientKey (child foriegn)
        /// {3}=PatientKey (parent foriegn)
        /// "INNER JOIN {0} on {1}.{2} = {0}.{3}"
        /// </summary>
        /// <param name="parentTableName">Patient (parent/destination)</param>
        /// <param name="childTableName">Study (child/source)</param>
        /// <param name="childColumnName">Study_PatientKey (child foriegn)</param>
        /// <param name="parentColumnName">PatientKey (parent foriegn)</param>
        /// <returns></returns>
        public string GetInnerJoinStatement 
        (
            string parentTableName, 
            string childTableName, 
            string childColumnName, 
            string parentColumnName
        )
        { 
            return $"INNER JOIN {GeneralStatementsProvider.WrapTable(parentTableName)} on " +
            $"{GeneralStatementsProvider.WrapColumn(childTableName, childColumnName)} = " +
            $"{GeneralStatementsProvider.WrapColumn(parentTableName, parentColumnName)}";
        }

        /// <summary>
        /// {0}=Study (child/source)
        /// {1}=Patient (parent/destination)
        /// {2}=PatientKey (parent foriegn)
        /// {3}=Study_PatientKey (child foriegn)
        /// "LEFT OUTER JOIN {0} on {1}.{2} = {0}.{3}"
        /// </summary>
        /// <param name="parentTableName">Study (child/source)</param>
        /// <param name="childTableName">Patient (parent/destination)</param>
        /// <param name="childColumnName">PatientKey (parent foriegn)</param>
        /// <param name="parentColumnName">Study_PatientKey (child foriegn)</param>
        /// <returns></returns>
        public string GetLeftOuterJoinStatement
        (
            string parentTableName, 
            string childTableName, 
            string childColumnName, 
            string parentColumnName
        )
        {
            return $"LEFT OUTER JOIN {GeneralStatementsProvider.WrapTable(parentTableName)} on " +
            $"{GeneralStatementsProvider.WrapColumn(childTableName, childColumnName)} = " +
            $"{GeneralStatementsProvider.WrapColumn(parentTableName, parentColumnName)}";
        }
    }
}