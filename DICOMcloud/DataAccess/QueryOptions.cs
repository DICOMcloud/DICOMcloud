using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DICOMcloud.DataAccess
{
    public interface IQueryOptions
    {
        //string QueryLevel { get; set; }

        int? Limit { get; set; }

        int? Offset { get; set; }
    }

    public class QueryOptions : IQueryOptions
    {
        //public string QueryLevel { get; set; }

        public int? Limit { get; set; }

        public int? Offset { get; set; }
    }
}
