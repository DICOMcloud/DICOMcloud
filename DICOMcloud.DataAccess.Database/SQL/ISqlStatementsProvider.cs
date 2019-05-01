using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DICOMcloud.DataAccess.Database.SQL
{
    public interface ISQLStatementsProvider
    {
        IGeneralStatementsProvider GeneralStatementsProvider { get;}

        SelectStatementsProvider SelectStatementsProvider { get; }

        InsertStatementsProvider InsertUpdateStatementsProvider { get; }

        void GetDeleteStatementsProvider ( ) ;
    }
}
