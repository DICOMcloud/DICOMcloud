using DICOMcloud.DataAccess.Database.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DICOMcloud.DataAccess.Database.SQL
{
    public class MySQLInsertStatementsProvider : InsertStatementsProvider
    {
        public MySQLInsertStatementsProvider(IGeneralStatementsProvider generalStatementsProvider)
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
            string values = string.Join(", ", parametersValueNames.Select((col) => 
            { 
                return col + " as " + GeneralStatementsProvider.WrapColumn(col.TrimStart('@')); 
            }));

            return string.Format(formattedInsert, columns, values);
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
            values = string.Join(",", values.Split(',').Select((valueText) =>
            {
                return (valueText.StartsWith("@") ?
                valueText +
                " as " +
                GeneralStatementsProvider.WrapColumn(valueText.TrimStart('@'))
                : valueText);
            }));

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
	INSERT INTO {2} ({4})
	SELECT * FROM (SELECT {5}) AS Tmp
	WHERE NOT EXISTS(SELECT {1} FROM {2} where {3}) LIMIT 1;

    SET {0} = (SELECT {1} FROM {2} where {3});
";

            const string InsertTableFormattedNoPrimaryKey =
@"
	INSERT INTO {1} ({2})
	VALUES ( {3} );

	SET {0} = LAST_INSERT_ID();
";
    }
}
