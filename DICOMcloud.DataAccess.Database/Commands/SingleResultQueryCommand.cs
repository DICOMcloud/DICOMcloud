using System;
using System.Data;
using System.Linq;

namespace DICOMcloud.DataAccess.Database.Commands
{
    public class SingleResultQueryCommand<T> : IDataAdapterCommand<T>
    {
        public SingleResultQueryCommand 
        ( 
            IDbCommand command, 
            string table, 
            string column,
            SetValueCallback<T> setValueCallback = null
        )
        {
            Command = command ;
            Table   = table ;
            Column  = column ;
            SetValueCallback = setValueCallback ;
        }

        public virtual bool Execute ( ) 
        {
            Command.Connection.Open ( );

            using ( var reader = Command.ExecuteReader ( CommandBehavior.CloseConnection | CommandBehavior.KeyInfo ) )
            {
                do
                {
                    while ( reader.Read ( ) )
                    {
                        //table name is not availabile in GetSchemaTable to compare. depend on the Column name to be unique across tables.
                        if ( null != reader.GetSchemaTable ( ).Select ( "ColumnName ='" + Column + "'"  ).FirstOrDefault ( ) )
                        {
                            int colIndex = reader.GetOrdinal ( Column ) ;
                            
                            if ( colIndex > -1 )
                            {
                                var value = reader.GetValue ( colIndex ) ;
                                
                                if ( SetValueCallback != null )
                                {
                                    Result = SetValueCallback (Column, value) ;

                                    return true ;
                                }
                                if ( value is T )
                                {
                                    Result = (T) value ;
                                    
                                    return true ;
                                }
                                else
                                {
                                    throw new InvalidOperationException ( "Generic type T is not supported for this Command value." ) ;
                                }
                            }
                        }
                        else
                        {
                            break ;
                        }
                    }
                } while ( reader.NextResult ( ) ) ;
            }

            return false ;
        }

        public T Result 
        { 
            get; 
            protected set;
        }

        public IDbCommand Command 
        {
            get;
            private set ;
        }

        private string Table  { get; set; }
        private string Column { get; set; }
        private SetValueCallback<T> SetValueCallback { get; set; }
    }
}
