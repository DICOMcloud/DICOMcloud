using DICOMcloud.DataAccess.Database.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DICOMcloud.DataAccess.Database.SQL
{
    public class MSInsertStatementsProvider : InsertStatementsProvider
    {
        public MSInsertStatementsProvider(IGeneralStatementsProvider generalStatementsProvider)
        : base ( generalStatementsProvider)
        {
        }

        public override string FormatInsertIntoTable
        (
            string formattedInsert,
            IEnumerable<string> columnNames, 
            IEnumerable<string> parametersValueNames
        )
        {
            StringBuilder result = new StringBuilder();

            string columns = string.Join(", ", columnNames);
            string values = string.Join(", ", parametersValueNames);

            return string.Format (formattedInsert, columns, values);
        }

        protected override string GetInsertTableFormatted 
        (
            string newPrimaryParam, 
            string primaryColumn, 
            string tableName, 
            string whereColumnsString, 
            string columns, 
            string values 
        )
        {
            return string.Format(InsertTableFormatted, newPrimaryParam, primaryColumn, tableName, whereColumnsString, columns, values);
        }

        protected override string GetInsertTableFormattedNoPrimaryKey 
        ( 
            string newPrimaryParam, 
            string tableName, 
            string columns, 
            string values 
        )
        {
            return string.Format(InsertTableFormattedNoPrimaryKey, newPrimaryParam, tableName, columns, values);
        }


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
    }
}
