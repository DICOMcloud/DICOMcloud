using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DICOMcloud.DataAccess.Database.Schema;

namespace DICOMcloud.DataAccess.Database
{
    public class SortingStrategyFactory : ISortingStrategyFactory
    {
        public DbSchemaProvider SchemaProvider { get; protected set ; }

        public SortingStrategyFactory ( DbSchemaProvider schemaProvier )
        {
            SchemaProvider = schemaProvier ;
        }

        public virtual ISortingStrategy Create ( ) 
        {
            return new ObjectArchieveSortingStrategy ( SchemaProvider ) ;
        }
    }
}
