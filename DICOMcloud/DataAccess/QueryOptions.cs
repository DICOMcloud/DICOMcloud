using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DICOMcloud.DataAccess
{
    public interface IQueryOptions
    {
        int? Limit { get; set; }

        int? Offset { get; set; }
    }

    public class QueryOptions : IQueryOptions
    {
        public int? Limit { get; set; }

        public int? Offset { get; set; }
    }
}
