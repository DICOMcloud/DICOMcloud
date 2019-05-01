using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DICOMcloud.DataAccess.Database.SQL
{
    public class MSStatementsProvider : ISQLStatementsProvider
    {
        public MSStatementsProvider ( )
        { 
            GeneralStatementsProvider = new MSGeneralStatementsProvider ( );
            SelectStatementsProvider = new SelectStatementsProvider (GeneralStatementsProvider);
            InsertUpdateStatementsProvider = new MSInsertStatementsProvider (GeneralStatementsProvider);
        }

        public IGeneralStatementsProvider GeneralStatementsProvider { get; private set; }

        public SelectStatementsProvider SelectStatementsProvider { get; private set; }

        public InsertStatementsProvider InsertUpdateStatementsProvider { get; private set; }

        public void GetDeleteStatementsProvider ( ) {}
    }
}
