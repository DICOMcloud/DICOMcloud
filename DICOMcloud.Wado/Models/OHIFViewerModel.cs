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
            studies = new List<OHIFStudy> ( ) ;
        }

        public string TransactionId { get; set; }

        public List<OHIFStudy> studies { get; set; }
    }

    public class OHIFStudy
    {
        public OHIFStudy ( ) 
        {
            series = new List<OHIFSeries> ( ) ;
        }


        public string StudyInstanceUID { get; set; }

        public string StudyDescription { get; set; }

        public string StudyDate { get; set; }

        public string StudyTime { get; set; }

        public string PatientName { get; set; }

        public string PatientId { get; set; }

        public List<OHIFSeries> series { get; set; }

    }

    public class OHIFSeries
    {
        public OHIFSeries ( ) 
        {
            instances = new List<OHIFInstance> ( ) ;
        }


        public string SeriesDescription { get; set; }

        public string SeriesInstanceUID { get; set; }

        public int SeriesNumber { get; set; }

        public string SeriesDate { get; set; }

        public string SeriesTime { get; set; }

        public string Modality { get; set; }

        public List<OHIFInstance> instances { get; set; }
    }



    public class OHIFMetadata
    {
        public int Columns { get; set; }

        public int Rows { get; set; }

        public int InstanceNumber { get; set; }

        public int AcquisitionNumber { get; set; }

        public string PhotometricInterpretation { get; set; }

        public int BitsAllocated { get; set; }

        public int BitsStored { get; set; }

        public int PixelRepresentation { get; set; }

        public int SamplesPerPixel { get; set; }

        public List<double> PixelSpacing { get; set; }

        public int HighBit { get; set; }

        public List<int> ImageOrientationPatient { get; set; }

        public List<double> ImagePositionPatient { get; set; }

        public string FrameOfReferenceUID { get; set; }

        public List<string> ImageType { get; set; }

        public string Modality { get; set; }

        public string SOPInstanceUID { get; set; }

        public string SeriesInstanceUID { get; set; }

        public string StudyInstanceUID { get; set; }
    }


    public class OHIFInstance
    {
        public OHIFMetadata metadata { get; set; }

        public string url { get; set; }

    }

}
