
using DICOMcloud.Wado.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.ModelBinding;
using System.Web.Http.ValueProviders;
using DICOMcloud.Pacs;
using Dicom;


namespace DICOMcloud.Wado
{
    public class DeleteRsRequestModelConverter : RsRequestModelConverter<WebDeleteRequest>
    {
        public DeleteRsRequestModelConverter ( )
        { }


        public override bool TryParse ( HttpRequestMessage request, ModelBindingContext bindingContext, out WebDeleteRequest result )
        {
            var studyParam    = bindingContext.ValueProvider.GetValue ("studyInstanceUID") ;
            var seriesParam   = bindingContext.ValueProvider.GetValue ("seriesInstanceUID") ;
            var instanceParam = bindingContext.ValueProvider.GetValue ("sopInstanceUID" ) ;


            result = null ;

            if ( null == studyParam  && 
                 null == seriesParam  && 
                 null == instanceParam  )
            {
                return false ;
            }
            else
            {
                result = new WebDeleteRequest ( ) 
                { 
                    Dataset     = new DicomDataset ( ).NotValidated(),
                    DeleteLevel = ObjectQueryLevel.Unknown 
                } ;

                if ( null != studyParam ) 
                { 
                    result.Dataset.Add ( DicomTag.StudyInstanceUID, studyParam.AttemptedValue ) ; 
                    
                    result.DeleteLevel = ObjectQueryLevel.Study ;
                }

                if ( null != seriesParam  ) 
                { 
                    result.Dataset.Add ( DicomTag.StudyInstanceUID, seriesParam.AttemptedValue ) ;

                    result.DeleteLevel = ObjectQueryLevel.Series ;
                }
                
                if ( null != instanceParam ) 
                { 
                    result.Dataset.Add ( DicomTag.StudyInstanceUID, instanceParam.AttemptedValue ) ; 

                    result.DeleteLevel = ObjectQueryLevel.Instance ;
                }
            }

            return result.DeleteLevel != ObjectQueryLevel.Unknown ;
        }
   }
}
