using DICOMcloud.DataAccess;
using System.Collections.Generic;
using fo = Dicom;

namespace DICOMcloud.DataAccess.Database
{
    public partial class QueryResponseBuilder
    { 
        private class EntityReadData
        { 
            public fo.DicomDataset                  CurrentDs                 = new fo.DicomDataset ( ) ;
            public Dictionary<uint, PersonNameData> CurrentPersonNames        = new Dictionary<uint,PersonNameData> ( )  ; 
            public PersonNameData                   CurrentPersonNameData     = null ;
            public uint                             CurrentPersonNameTagValue = 0 ;
            public string                           KeyValue                  = null ;

        
            public bool  IsCurrentDsSequence   = false ;
            public bool  IsCurrentDsMultiValue = false ;
            public uint  ForeignTagValue       = 0 ;
            public fo.DicomDataset ForeignDs = null ;             
            
            //public void ResetCurrentRead ( )
            //{
            //    CurrentDs                 = null ;
            //    CurrentPersonNames        = new Dictionary<uint,PersonNameData> ( )  ; 
            //    CurrentPersonNameData     = null ;
            //    CurrentPersonNameTagValue = 0 ;
            //    KeyValue                  = null ;
            //    IsCurrentDsSequence   = false ;
            //    IsCurrentDsMultiValue = false ;
            //    ForeignTagValue       = 0 ;
            //    ForeignDs              = null ; 

            //}
        }
    }
}
