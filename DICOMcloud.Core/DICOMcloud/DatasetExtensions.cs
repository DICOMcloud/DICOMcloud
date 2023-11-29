using FellowOakDicom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using fo = Dicom ;

namespace DICOMcloud
{
    public static class DatasetExtensions
    {
        public static void Merge ( this DicomDataset source, DicomDataset destination )
        { 
            
            foreach ( var element in source )
            { 
                destination.AddOrUpdate ( element ) ;
            }
        }
    }
}
