using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DICOMcloud;

namespace DICOMcloud.Wado
{
    public class RetieveUrlProvider
    {
        public static string config_WadoRs_API_URL = "app:WadoRsUrl" ;
        public RetieveUrlProvider ( )
        {
            string wadoRsUrl = System.Configuration.ConfigurationManager.AppSettings["wadoRsUrl"] ;
        
            wadoRsUrl = wadoRsUrl ?? "" ;

            BaseUrl = wadoRsUrl ;
        }

        public RetieveUrlProvider ( string baseUrl )
        {
            BaseUrl = baseUrl ;
        }

        public string GetStudyUrl ( string studyInstanceUID )
        {
            return string.Format ( "{0}/{1}/studies/{2}", BaseUrl, "wadors", studyInstanceUID )  ;
        }

        public string GetInstanceUrl ( ObjectId instance )
        {
            return GetInstanceUrl ( instance.StudyInstanceUID, instance.SeriesInstanceUID, instance.SOPInstanceUID ) ;
        }
        public string GetInstanceUrl ( string studyInstanceUID, string seriesInstanceUID, string sopInstanceUID )
        {
            return string.Format ( "{0}/{1}/studies/{2}/series/{3}/instances/{4}", BaseUrl, "wadors", studyInstanceUID, seriesInstanceUID, sopInstanceUID )  ;
        }

        public string BaseUrl 
        { 
            get; 
            set; 
        }
    }
}
