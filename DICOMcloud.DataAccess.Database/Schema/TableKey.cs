using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DICOMcloud.DataAccess.Database.Schema
{
    public class TableKey : IComparable<TableKey>
    {
        public TableKey ( ) : this ( "", 0 ) {}

        public TableKey ( string name, byte order )
        {
            Name            = name ;
            OrderValue      = order ;
            ModelKeyColumns = new List<ColumnInfo> ( ) ;
            Columns         = new List<ColumnInfo> ( ) ;
        }

        public ushort OrderValue
        {
            get; set;
        }

        public string Name
        {
            get; set;
        }

        public TableKey Parent { get; set; }
        
        public ColumnInfo KeyColumn {  get; set; }
        
        public ColumnInfo ForeignColumn {  get; set; }

        public int CompareTo(TableKey other)
        {
            if ( null == other )
            {
                return 1 ;
            }

            if ( OrderValue > other.OrderValue )
            {
                return 1 ;
            }
            else if ( OrderValue < other.OrderValue )
            {
                return -1 ;
            }

            return 0 ;
        }

        public static implicit operator int(TableKey table)
        {
            return table.OrderValue ;
        }

        public static implicit operator string(TableKey table)
        {
            return table.Name ;
        }

        public bool IsSequence { get; set; }

        public bool IsMultiValue { get; set; }

        public uint ParentElement { get; set; }

        public IList<ColumnInfo> ModelKeyColumns { get; set; }
        
        public IList<ColumnInfo> Columns { get; set; }

        public override string ToString()
        {
           return Name ;
        }
    }
}
