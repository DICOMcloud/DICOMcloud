
using Dicom;
using DICOMcloud.Wado.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Net.Http;

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
                    result.Dataset.Add ( DicomTag.StudyInstanceUID, studyParam.FirstValue ) ; 
                    
                    result.DeleteLevel = ObjectQueryLevel.Study ;
                }

                if ( null != seriesParam  ) 
                { 
                    result.Dataset.Add ( DicomTag.StudyInstanceUID, seriesParam.FirstValue ) ;

                    result.DeleteLevel = ObjectQueryLevel.Series ;
                }
                
                if ( null != instanceParam ) 
                { 
                    result.Dataset.Add ( DicomTag.StudyInstanceUID, instanceParam.FirstValue) ; 

                    result.DeleteLevel = ObjectQueryLevel.Instance ;
                }
            }

            return result.DeleteLevel != ObjectQueryLevel.Unknown ;
        }
   }
}
