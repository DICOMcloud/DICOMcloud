using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DICOMcloud;
using DICOMcloud.Wado.Core.WadoResponse;
using Microsoft.Extensions.Configuration;

namespace DICOMcloud.Wado
{
    public class RetrieveUrlProvider : IRetrieveUrlProvider
    {
        public RetrieveUrlProvider (IConfiguration config)
        {
            string wadoRsUrl = config[config_WadoRs_API_URL];
            string wadoUriUrl = config[config_WadoUri_API_URL];


            wadoRsUrl = wadoRsUrl ?? "";
            wadoUriUrl = wadoUriUrl ?? "";

            Init(wadoRsUrl, wadoUriUrl);
        }

        public static string config_WadoRs_API_URL = "app:WadoRsUrl" ;
        public static string config_WadoUri_API_URL = "app:WadoUriUrl" ;


        public string GetStudyUrl(IStudyId study)
        {
            return GetStudyUrl (study.StudyInstanceUID);
        }

        private string GetStudyUrl (string studyInstanceUID)
        {
            return string.Format("{0}/{1}/studies/{2}", BaseWadoRsUrl, "wadors", studyInstanceUID);
        }

        public virtual string GetInstanceUrl ( IObjectId instance )
        {
            return GetInstanceUrl ( instance.StudyInstanceUID, instance.SeriesInstanceUID, instance.SOPInstanceUID ) ;
        }
        
        private string GetInstanceUrl 
        (
            string studyInstanceUID, 
            string seriesInstanceUID, 
            string sopInstanceUID
        )
        {
            if ( PreferWadoUri )
            {
                return BaseWadoUriUrl + GenerateWadoUriPart (studyInstanceUID, seriesInstanceUID, sopInstanceUID );
            }
            else
            {
                return BaseWadoRsUrl + GenerateWadoRsPart (studyInstanceUID, seriesInstanceUID, sopInstanceUID);
            }
        }

        protected virtual string GenerateWadoUriPart
        ( 
            string studyInstanceUID, 
            string seriesInstanceUID, 
            string sopInstanceUID
        )
        {
            return string.Format("?RequestType=wado&studyUID={0}&seriesUID={1}&objectUID={2}&&contentType=application/dicom", studyInstanceUID, seriesInstanceUID, sopInstanceUID);
        }

        protected virtual string GenerateWadoRsPart
        (
            string studyInstanceUID,
            string seriesInstanceUID,
            string sopInstanceUID
        )
        {
            return string.Format("/studies/{0}/series/{1}/instances/{2}", studyInstanceUID, seriesInstanceUID, sopInstanceUID);
        }


        public virtual string BaseWadoRsUrl  { get; set; }
        public virtual string BaseWadoUriUrl { get; set; }
        public virtual bool PreferWadoUri    { get; set; }


        private void Init ( string wadoRsUrl, string wadoUriUrl ) 
        {
            BaseWadoRsUrl  = wadoRsUrl ;
            BaseWadoUriUrl = wadoUriUrl ;
            PreferWadoUri  = true ;
        }
    }
}
