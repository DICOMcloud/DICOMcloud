using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DICOMcloud.DataAccess.Database.Commands
{
    public interface IDataAdapterCommand<T>
    {
        bool Execute ( ) ;

        T Result { get; }
    }

    public interface IPagedDataAdapterCommand<T> : IDataAdapterCommand<IEnumerable<T>>
    {
        int? TotalCount { get; }
    }
}
