using DICOMcloud.DataAccess;
using System.Collections.Generic;
using Dicom;

namespace DICOMcloud.DataAccess.Database
{
    public partial class QueryResponseBuilder
    { 
        private class EntityReadData
        {
            public DicomDataset                     CurrentDs                 = new DicomDataset ( ) { AutoValidate = false };
            public Dictionary<uint, PersonNameData> CurrentPersonNames        = new Dictionary<uint,PersonNameData> ( )  ; 
            public PersonNameData                   CurrentPersonNameData     = null ;
            public uint                             CurrentPersonNameTagValue = 0 ;
            public string                           KeyValue                  = null ;

        
            public bool  IsCurrentDsSequence   = false ;
            public bool  IsCurrentDsMultiValue = false ;
            public uint  ForeignTagValue       = 0 ;
            public DicomDataset ForeignDs = null ;
        }
    }
}
