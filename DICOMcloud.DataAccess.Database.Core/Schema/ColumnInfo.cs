using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DICOMcloud.DataAccess.Database.Schema
{
    public class ColumnInfo
    {
        private DataColumn _column ;

        public ColumnInfo ( )
        {}

        public ColumnInfo ( DataColumn column )
        {
            _column = column ;
            Name = column.ColumnName ;
            IsNumber = column.DataType == typeof(int) ;
            IsDateTime = column.DataType == typeof(DateTime) ;
        }

        public string Name
        {
            get;
            set; 
        }

        public TableKey Table
        {
            get ;
            set ;
        }

        public bool IsNumber 
        {
            get; set ;
        }

        public bool IsDateTime 
        {
            get; set ;
        }

        public IList<string> Values {  get; set; }
        
        public uint[] Tags
        {
            get;
            set;
        }
        
        public string Defenition
        {
            get;
            set;
        }

        public override string ToString()
        {
            return this.Name ;
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        public bool IsKey { get; set; }

        public bool IsForeign { get; set; }

        public bool IsModelKey { get; set; }

        public bool IsData
        {
            get;
            set;
        }

        public static implicit operator string(ColumnInfo column)
        {
            return column.Name ;
        }
    }
}
