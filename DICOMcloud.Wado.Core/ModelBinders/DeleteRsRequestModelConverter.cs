
using DICOMcloud.Wado.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using DICOMcloud.Pacs;
using Dicom;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Http;
using DICOMcloud.Extensions;

namespace DICOMcloud.Wado
{
    public class DeleteRsRequestModelConverter : RsRequestModelConverter<WebDeleteRequest>
    {
        public DeleteRsRequestModelConverter ( )
        { }


        public override bool TryParse (ModelBindingContext bindingContext, out WebDeleteRequest result )
        {
            var studyParam    = bindingContext.ValueProvider.GetValue("studyInstanceUID");
            var seriesParam   = bindingContext.ValueProvider.GetValue("seriesInstanceUID");
            var instanceParam = bindingContext.ValueProvider.GetValue("sopInstanceUID");


            studyParam.IsNullOrEmpty();
            result = null ;

            if ( studyParam.IsNullOrEmpty() && 
                 seriesParam.IsNullOrEmpty() && 
                 instanceParam.IsNullOrEmpty())
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

                if ( !studyParam.IsNullOrEmpty()) 
                { 
                    result.Dataset.Add ( DicomTag.StudyInstanceUID, studyParam.FirstOrDefault()) ; 
                    
                    result.DeleteLevel = ObjectQueryLevel.Study ;
                }

                if ( !seriesParam.IsNullOrEmpty()) 
                { 
                    result.Dataset.Add ( DicomTag.StudyInstanceUID, seriesParam.FirstOrDefault()) ;

                    result.DeleteLevel = ObjectQueryLevel.Series ;
                }
                
                if ( !instanceParam.IsNullOrEmpty()) 
                { 
                    result.Dataset.Add ( DicomTag.StudyInstanceUID, instanceParam.FirstOrDefault()) ; 

                    result.DeleteLevel = ObjectQueryLevel.Instance ;
                }
            }

            return result.DeleteLevel != ObjectQueryLevel.Unknown ;
        }
   }
}
