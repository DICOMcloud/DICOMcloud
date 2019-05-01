using DICOMcloud.DataAccess.Database.Schema;
using DICOMcloud.DataAccess.Database.SQL;
using DICOMcloud.DataAccess.Matching;
using System;
using System.Collections.Generic;
using System.Text;

namespace DICOMcloud.DataAccess.Database
{
    public partial class QueryBuilder
    {
        public SortedDictionary<TableKey, List<string>> ProcessedColumns {  get { return _processedColumns; } }

        public QueryBuilder (SelectStatementsProvider selectStatementsProvider) 
        {
            SelectStatementsProvider = selectStatementsProvider;

            Returns           = new List<string>   ( ) ;
            Conditions        = new List<string>   ( ) ;
            ColumnDefenitions = new List<string>   ( ) ;
            Joins             = new SqlJoinBuilder        ( selectStatementsProvider ) ;
            ConditionBuilder  = new DicomConditionBuilder (selectStatementsProvider.GeneralStatementsProvider) ;
        }

        public virtual string GetQueryText 
        ( 
            TableKey sourceTable, 
            IQueryOptions options = null
        )
        {
            if ( (Returns == null || Returns.Count == 0 ) )
            {
                throw new InvalidOperationException ( "No columns has been processed." ) ;
            }

            string selectText = string.Join ( ",", Returns  ) ;
            string joinsText  = string.Join ( " ", Joins.ToString ( ) ) ;
            string whereText  = string.Join ( " AND ", Conditions ) ; 

            if ( string.IsNullOrWhiteSpace ( joinsText ))
            {
                joinsText = "" ;
            }

            if ( string.IsNullOrWhiteSpace ( whereText))
            {
                whereText = "" ;
            }
            else
            {
                whereText = " AND " + whereText ;
            }

            StringBuilder queryBuilder = new StringBuilder ( ) ;

            
            queryBuilder.AppendFormat (SelectStatementsProvider.GetSelectStatement(selectText, sourceTable, joinsText , whereText) ) ;


            
            return queryBuilder.ToString ( ) ;
        }


        public virtual void ProcessColumn
        (   
            TableKey sourceTable,
            ColumnInfo column, 
            IQueryInfo queryInfo = null,
            IList<string> columnValues = null
        )
        {
            string whereCondition ;
            
                
            FillReturns ( column ) ;
            FillJoins ( sourceTable, column ) ;

            whereCondition = ConditionBuilder.CreateMatching ( sourceTable, column, queryInfo, columnValues ) ;
        
            if ( !string.IsNullOrWhiteSpace ( whereCondition ) )
            { 
                Conditions.Add ( whereCondition ) ;
            }
        }

        protected virtual void FillReturns(ColumnInfo column )
        {
            if ( !_processedColumns.ContainsKey ( column.Table ) )
            {
                _processedColumns.Add ( column.Table, new List<string> ( ) ) ;
                
                FillReturns ( column.Table.KeyColumn ) ;

                if ( null!= column.Table.ForeignColumn )
                { 
                    FillReturns ( column.Table.ForeignColumn ) ;
                }
            }

            
            if ( !_processedColumns[column.Table].Contains ( column.Name ) )
            {
                //always return any matching
                Returns.Add (SelectStatementsProvider.GeneralStatementsProvider.WrapColumn (column.Table.Name, column.Name )) ;
            
                _processedColumns[column.Table].Add ( column.Name ) ;
            
                ColumnDefenitions.Add ( column.Defenition ) ;
            }
        }

        protected virtual void FillJoins ( TableKey sourceTable, ColumnInfo column )
        {
            if ( !column.Table.Name.Equals (sourceTable, StringComparison.InvariantCultureIgnoreCase ) &&
                 !column.IsData )
            {
                Joins.AddJoins ( sourceTable, column.Table ) ;
                
                //string joinKey = GetJoinKey ( sourceTable, column.Table.Name ) ;

                //if ( !_joins.ContainsKey ( joinKey ))
                //{
                //    _joins.Add (joinKey, _cachedJoins[joinKey] ) ;
                //}
            }
        }

        public SelectStatementsProvider SelectStatementsProvider { get; private set; }
        protected virtual List<string>          Returns           { get; set; }
        protected virtual List<string>          Conditions        { get; set; }
        protected virtual List<string>          ColumnDefenitions { get; set; }
        protected virtual SqlJoinBuilder        Joins             { get; set; }
        protected virtual DicomConditionBuilder ConditionBuilder  { get; set; } 

        private string GetJoinKey(string sourceTable, string destTable)
        {
            return SelectStatementsProvider.GeneralStatementsProvider.WrapColumn (sourceTable, destTable) ;
        }

        private SortedDictionary<TableKey, List<string>> _processedColumns = new SortedDictionary<TableKey, List<string>>();
    }

    public partial class QueryBuilder
    {
        public class DicomConditionBuilder
        {
            public DicomConditionBuilder (IGeneralStatementsProvider generalStatementsProvider)
            { 
                GeneralStatementsProvider = generalStatementsProvider;
            }

            public IGeneralStatementsProvider GeneralStatementsProvider { get; }

            public virtual string CreateMatching
            ( 
                string sourceTable, 
                ColumnInfo column, 
                IQueryInfo queryInfo, 
                IList<string> matchValues
            )
            {
                if ( (null!= matchValues) && (matchValues.Count != 0) )
                {
                    MatchBuilder matchBuilder = new MatchBuilder (GeneralStatementsProvider) ;
                
                    if ( column.IsDateTime && matchValues.Count >= 2 )
                    {
                        matchBuilder.Column ( column ).GreaterThanOrEqual ( ).Value ( matchValues [ 0 ] ).And ( ).
                                     Column ( column ).LessThanOrEqual ( ).Value ( matchValues [ 1]  ) ;
                    }
                    else
                    {
                        for ( int valueIndex = 0; valueIndex < matchValues.Count; valueIndex++ )
                        {
                            string stringValue = matchValues[valueIndex] ;
                    
                            if ( string.IsNullOrWhiteSpace (stringValue) )
                            { 
                                continue ;
                            }
                    
                            matchBuilder.Column ( column ) ;
                    
                            //TODO:??
                            //if ( queryInfo.)
                            if ( queryInfo.ExactMatch )
                            { 
                                matchBuilder.Equals ( ) ;
                            }
                            else
                            { 
                                matchBuilder.Like ( ) ;
                            }

                            matchBuilder.Value ( stringValue) ;

                            if ( valueIndex != matchValues.Count -1 )
                            { 
                                matchBuilder.Or ( ) ;
                            }
                        }
                    }

                    return matchBuilder.Match.ToString ( )  ;
                }

                return "" ;
            }
        }
    }
}
