using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using fo = Dicom ;

namespace DICOMcloud
{
    public class ObjectId : IObjectId
    {

        public ObjectId ( ) 
        {}

        public ObjectId ( fo.DicomDataset dataset )
        {
            StudyInstanceUID  = dataset.Get<string> (fo.DicomTag.StudyInstanceUID, 0, "" ) ;
            SeriesInstanceUID = dataset.Get<string> (fo.DicomTag.SeriesInstanceUID, 0, "" ) ;
            SOPInstanceUID    = dataset.Get<string> (fo.DicomTag.SOPInstanceUID, 0, "" ) ;
        }

        public ObjectId ( IStudyId study )
        {
            StudyInstanceUID  = study.StudyInstanceUID;
        }

        public ObjectId ( ISeriesId series )
        {
            StudyInstanceUID  = series.StudyInstanceUID;
            SeriesInstanceUID = series.SeriesInstanceUID;
        }

        //public ObjectId

        public string SeriesInstanceUID
        {
            get ;
            set ;
        }

        public string SOPInstanceUID
        {
            get ;
            set ;
        }

        public string StudyInstanceUID
        {
            get ;
            set ;
        }
    
        public int? Frame { get; set; }    
    }
}
