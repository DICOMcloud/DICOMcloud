namespace DICOMcloud.DataAccess.Database.SQL
{
    public interface ISelectStatementsProvider
    {
        string GetSelectStatement
        ( 
            string columnsSelectPart, 
            string tableFromPart, 
            string joinsPart, 
            string conditionsPart
        );

        string GetInnerJoinStatement
        (
            string parentTableName,
            string childTableName,
            string childColumnName,
            string parentColumnName
        );

        string GetLeftOuterJoinStatement
        (
            string parentTableName,
            string childTableName,
            string childColumnName,
            string parentColumnName
        );
    }
}