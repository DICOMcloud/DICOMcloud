using DICOMcloud.DataAccess.Database.Schema;
using DICOMcloud.DataAccess.Database.SQL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DICOMcloud.DataAccess.Database
{
    public partial class QueryBuilder
    { 
        class MatchBuilder
        {
            public IGeneralStatementsProvider GeneralStatementsProvider { get; }

            private ColumnInfo _lastColumn ;
            private bool       _inOR ;
            private int        _lastColumnStartIndex = 0 ;
            public StringBuilder Match { get;set; }

            public MatchBuilder (IGeneralStatementsProvider generalStatementsProvider)
            {
                GeneralStatementsProvider = generalStatementsProvider;
                
                _lastColumn = null ; 

                Match = new StringBuilder ( ) ;
            }

            public MatchBuilder Column ( ColumnInfo column )
            {
                _lastColumn = column ;

                _lastColumnStartIndex = Match.Length + 1 ;

                Match.Append ( ( _inOR ? "" : "(" ) + GeneralStatementsProvider.WrapColumn (column.Table.Name, column.Name ) ) ;
            
                return this ;
            }

            public MatchBuilder Equals ( )
            { 
                Match.Append ( OperationEqual ) ;

                return this ;
            }

            public MatchBuilder Like ( )
            { 
                Match.Append ( OperationLike ) ;

                return this ;
            }

            public MatchBuilder Value ( string value )
            {
                if (_lastColumn == null) {  throw new InvalidOperationException ( "Column must be called first" ) ; }

                if (!_lastColumn.IsNumber)
                { 
                    value = "'" + value.Replace ( "*", "%" ) + "'" ;
                }

                Match.Append ( value ) ;
                Match.Append ( ")" ) ;

                return this ;
            }

            public MatchBuilder Or ( )
            { 
                if ( _inOR )
                { 
                    Match.Remove ( _lastColumnStartIndex, 1 ) ; //removing the (
                }

                Match.Remove ( Match.Length-1, 1 ) ; // removing the )
                
                
                Match.Append ( OperationOr ) ;

                _inOR = true ;

                return this ;
            }

            public MatchBuilder And()
            {
                Match.Append ( OperationAnd ) ;

                return this ;
            }

            public MatchBuilder GreaterThanOrEqual()
            {
                Match.Append ( OperationGreaterThanOrEqual ) ;

                return this ;
            }

            public MatchBuilder LessThanOrEqual()
            {
                Match.Append ( OperationLessThanOrEqual ) ;

                return this ;
            }

            private const string OperationEqual = " = " ;
            private const string OperationLike = " LIKE " ;
            private const string OperationOr = " OR " ;
            private const string OperationAnd = " AND " ;
            private const string OperationGreaterThanOrEqual = " >= " ;
            private const string OperationLessThanOrEqual = " <= " ;
        }
    }
}
