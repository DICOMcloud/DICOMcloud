using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DICOMcloud.Wado.Models
{
    public class OHIFViewerModel
    {
        public OHIFViewerModel ( ) 
        {
            Studies = new List<OHIFStudy> ( ) ;
        }

        public string TransactionId { get; set; }

        public List<OHIFStudy> Studies { get; set; }
    }

    public class OHIFStudy
    {
        public OHIFStudy ( ) 
        {
            SeriesList = new List<OHIFSeries> ( ) ;
        }

        public string StudyInstanceUid { get; set; }

        public string PatientName { get; set; }

        public List<OHIFSeries> SeriesList { get; set; }
    }

    public class OHIFSeries
    {
        public OHIFSeries ( ) 
        {
            Instances = new List<OHIFInstance> ( ) ;
        }

        public string SeriesInstanceUid { get; set; }

        public string SeriesDescription { get; set; }

        public List<OHIFInstance> Instances { get; set; }
    }

    public class OHIFInstance
    {
        public string SopInstanceUid { get; set; }

        public int Rows { get; set; }

        public int? NumberOfFrames { get; set;}

        public string Url { get; set; }
    }
}
