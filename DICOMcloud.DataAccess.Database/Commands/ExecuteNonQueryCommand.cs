using System.Data;

namespace DICOMcloud.DataAccess.Database.Commands
{
    public class ExecuteNonQueryCommand : IDataAdapterCommand<int>
    {
        public ExecuteNonQueryCommand ( IDbCommand command )
        {
            Command = command ;
        }

        public virtual bool Execute ( ) 
        {
            Command.Connection.Open ( );

            try
            {
                Result = Command.ExecuteNonQuery ( ) ;

                if ( Result > 0 )
                {
                    return true ;
                }
                else
                {
                    return false ;
                }
            }
            finally
            {
                Command.Connection.Close ( );
            }
        }

        public int Result 
        { 
            get; 
            protected set;
        }

        public IDbCommand Command 
        {
            get;
            private set ;
        }
    }
}
