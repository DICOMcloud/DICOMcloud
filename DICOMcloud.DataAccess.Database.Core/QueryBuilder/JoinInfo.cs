using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DICOMcloud.DataAccess.Database
{
    public partial class QueryBuilder
    { 
        class JoinInfo
        {
            public string SourceTable { get; set; }
            public string DestinationTable { get; set; }

            public string JoinText { get; set; }
        }
    }
}
