using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DICOMcloud.DataAccess.Database
{
    public interface ISortingStrategyFactory
    {
        ISortingStrategy Create ( ) ;
    }
}
