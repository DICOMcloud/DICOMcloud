using DICOMcloud.DataAccess.Database.Schema;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DICOMcloud.DataAccess.Database
{
    public class SqlJoinBuilder
    {
        private static ConcurrentDictionary<DbJoin, ICollection<string>> _cachedJoins = new ConcurrentDictionary<DbJoin,ICollection<string>> ( ) ;
        private static Random _aliasGenerator = new Random ( ) ;
        private Dictionary<int,string> _generatedJoins = new Dictionary<int,string> ( ) ;

        public void AddJoins ( TableKey source, TableKey destination )
        { 
            if ( source == destination ) return ;

            DbJoin join = new DbJoin ( source, destination ) ;

            CreateJoin ( join ) ;
        }

        public override string ToString()
        {
            return string.Join ( " ", _generatedJoins.Values ) ;
        }

        private void CreateJoin ( DbJoin arg )
        {
            if ( !AddChildToParentJoins ( arg ) )
            {
                AddParentToChildJoins ( arg ) ; 
            }
        }

        private bool AddChildToParentJoins ( DbJoin arg )
        {
            TableKey childTable = arg.Source;
            TableKey parent = null;

            while ( ( parent = childTable.Parent ) != null && ( arg.Destination != childTable ) )
            {
                DbJoin join = new DbJoin ( childTable, parent );

                if ( !_generatedJoins.ContainsKey ( join ) )
                {
                    var joins = _cachedJoins.GetOrAdd ( join, GenerateAndAddNewJoin );
                    _generatedJoins.Add ( join, joins.First ( ) );
                }

                childTable = parent;
            }

            return childTable == arg.Destination ;
        }

        private void AddParentToChildJoins ( DbJoin arg )
        {
            TableKey childTable = arg.Destination;
            TableKey parent = null;
            Dictionary<int,string> localGeneratedJoins = new Dictionary<int, string> ( ) ;


            while ( ( parent = childTable.Parent ) != null && ( arg.Source != childTable ) )
            {
                DbJoin join = new DbJoin ( parent, childTable );

                if ( !_generatedJoins.ContainsKey ( join ) )
                {
                    var joins = _cachedJoins.GetOrAdd ( join, GenerateAndAddJoinWithChild );
                    localGeneratedJoins.Add ( join, joins.First ( ) );
                }

                childTable = parent;
            }

            if ( arg.Source == childTable )
            {
                Merge ( _generatedJoins, localGeneratedJoins );
            }
        }

        private List<string> GenerateAndAddNewJoin(DbJoin arg )
        {
            string joinText = GetJoinWithParent ( arg.Source ) ;
        
            return new List<string> ( new string[]{joinText} ) ;
        }

        private List<string> GenerateAndAddJoinWithChild ( DbJoin arg )
        {
            string joinText = GetJoinWithChild ( arg.Destination ) ;
        
            return new List<string> ( new string[]{joinText} ) ;
        }

        private string GetJoinWithParent(TableKey table )
        {
            //{0}=Patient (parent/destination)
            //{1}=Study (child/source)
            //{2}=Study_PatientKey (child foriegn)
            //{3}=PatientKey (parent foriegn)

            return string.Format ( SqlQueries.Joins.JoinFormattedTemplate,
                                                table.Parent.Name, 
                                                table.Name, 
                                                table.ForeignColumn.Name, 
                                                table.Parent.KeyColumn.Name ) ;

        }

        private string GetJoinWithChild ( TableKey child )
        {
            return string.Format ( SqlQueries.Joins.OuterJoinFormattedTemplate,
                                                child.Name, 
                                                child.Parent.Name, 
                                                child.Parent.KeyColumn.Name,
                                                child.ForeignColumn.Name ) ;
                                                //child.Name + _aliasGenerator.Next ( 1000 ) ) ;
        }

        public static void Merge (Dictionary<int, string> source, Dictionary<int, string> collection)
        {
            if (collection == null)
            {
                throw new ArgumentNullException("Collection is null");
            }

            foreach (var item in collection)
            {
                if(!source.ContainsKey(item.Key)){ 
                    source.Add(item.Key, item.Value);
                }
            } 
        }
    }

        

    
    class DbJoin : IComparable<DbJoin>
    { 
        public DbJoin ( TableKey source, TableKey destination )
        { 
            Source      = source ;
            Destination = destination ;

            _key = GenerateKey ( source, destination ) ;
        }

        public static int GenerateKey(TableKey source, TableKey destination )
        {
            return (int) (source.OrderValue << 16 | (destination.OrderValue) );
        }

        int _key ;
        public TableKey Source      {get; set;}
        public TableKey Destination {get; set;}
    
        public static implicit operator int(DbJoin join )
        {
            return join._key ;
        }

        public override int GetHashCode()
        {
            return _key ;
        }

        public int CompareTo(DbJoin obj)
        {
            return _key.CompareTo ( obj._key ) ;
        }

        public override bool Equals(object obj)
        {
            if (null == obj) {  return false ;}

            if (!(obj is DbJoin)) {  return false ; }

            return ((DbJoin) obj)._key == _key ;
        }
    }
}
