namespace DICOMcloud.DataAccess.Database.SQL
{
    public interface IGeneralStatementsProvider
    {
        string WrapTable(string tableName);
        string WrapColumn (string tableName, string columnName);
        string WrapColumn(string columnName);

        string GetParameterName(string name);
        string GetVariableName (string variable);
        string GetDeclareVariable(string variableName, string type = "");

        string BeginTransaction { get; }
        string CommitTransaction { get; }
    }
}