using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dicom;

namespace DICOMcloud.DataAccess.Database
{
    public interface IQueryResponseBuilder
    {
        string QueryLevelTableName { get; set; }
        
        void BeginResultSet ( string name ) ;
        void EndResultSet   ( ) ;

        void BeginRead  ( ) ;
        void EndRead    ( ) ;
        void ReadData   ( string tableName, string columnName, object value ) ;
    
        bool ResultExists ( string table, object keyValue ) ;

        IEnumerable<DicomDataset> GetResponse ( ) ;

        IEnumerable<DicomDataset> GetResults(string resultName);
    }
}
