using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dicom;

namespace DICOMcloud
{
    public class DicomObjectIdFactory
    {
        private static DicomObjectIdFactory _instance = new DicomObjectIdFactory ( ) ;

        public static void RegisterInstance ( DicomObjectIdFactory instance )
        {
            _instance = instance ;
        }

        public static DicomObjectIdFactory Instance
        {
            get
            {
                return _instance ;
            }
        }

        public virtual IObjectId CreateObjectId ( DicomDataset dataset )
        {
            return new ObjectId ( ) {
                StudyInstanceUID = dataset.Get<string>(DicomTag.StudyInstanceUID, 0, ""),
                SeriesInstanceUID = dataset.Get<string>(DicomTag.SeriesInstanceUID, 0, ""),
                SOPInstanceUID = dataset.Get<string>(DicomTag.SOPInstanceUID, 0, "")
            } ;
        }
    }
}
