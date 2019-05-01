using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DICOMcloud.DataAccess.Database.SQL
{
    public class MySQLGeneralStatementsProvider : IGeneralStatementsProvider
    {
        public string WrapTable(string tableName)
        {
            return $"`{tableName}`";
        }

        public string WrapColumn(string tableName, string columnName)
        { 
            return $"`{tableName}`.`{columnName}`" ;
        }

        public string WrapColumn(string columnName)
        {
            return $"`{columnName}`";
        }

        public string GetParameterName(string name)
        {
            return "@" + name;
        }

        public string GetVariableName(string variable)
        {
            return "@" + variable;
        }

        public string GetDeclareVariable(string variableName, string type = "")
        {
            return "";
        }

        public string BeginTransaction
        {
            get { return "start transaction;"; }
        }

        public string CommitTransaction
        {
            get { return "commit;"; }
        }

        public bool SupportDeclareVariableInScript { get { return true;} }
    }
}
