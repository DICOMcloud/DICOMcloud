
using System;
using System.Net.Http;
using DICOMcloud.Wado.Models;
using System.Web.Http.ModelBinding;
using System.Web.Http.ValueProviders;
using fo = Dicom;

namespace DICOMcloud.Wado
{
    public class QidoRequestModelConverter 
    {
        public QidoRequestModelConverter ( )
        { }

        public virtual bool TryParse ( HttpRequestMessage request, ModelBindingContext bindingContext, out IQidoRequestModel result )
        {
            IQidoRequestModel wadoReq = CreateModel ( );

            wadoReq.AcceptHeader = request.Headers.Accept;
            wadoReq.AcceptCharsetHeader = request.Headers.AcceptCharset;

            var query = request.RequestUri.ParseQueryString();

            foreach (var key in query)
            {
                string queryKey = ((string)key).Trim().ToLower();

                if (queryKey == "") { continue; }

                switch (queryKey)
                {
                    case QidoRequestKeys.FuzzyMatching:
                        {
                            bool fuzzy;

                            if (bool.TryParse(query[QidoRequestKeys.FuzzyMatching], out fuzzy))
                            {
                                wadoReq.FuzzyMatching = fuzzy;
                            }
                        }
                        break;

                    case QidoRequestKeys.Limit:
                        {
                            int limit;

                            if (int.TryParse(query[QidoRequestKeys.Limit], out limit))
                            {
                                wadoReq.Limit = limit;
                            }
                        }
                        break;

                    case QidoRequestKeys.Offset:
                        {
                            int offset;

                            if (int.TryParse(query[QidoRequestKeys.Offset], out offset))
                            {
                                wadoReq.Offset = offset;
                            }
                        }
                        break;

                    case QidoRequestKeys.IncludeField:
                        {
                            string includeFields = query[QidoRequestKeys.IncludeField];

                            if (!string.IsNullOrWhiteSpace(includeFields))
                            {
                                wadoReq.Query.IncludeElements.AddRange(includeFields.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries));
                            }
                        }
                        break;

                    default:
                        {
                            string queryValue = query[queryKey].Trim();


                            if (queryKey.StartsWith("_"))
                            {
                                wadoReq.Query.CustomParameters.Add(queryKey, queryValue);
                            }
                            else
                            {
                                wadoReq.Query.MatchingElements.Add(queryKey, queryValue);
                            }
                        }
                        break;
                }
            }

            CheckAndFillUids(bindingContext.ValueProvider, wadoReq);
            
            result = wadoReq;

            return true;
        }
        
        /// <summary>
        /// If access Qido interface with URL http://localhost:44301/qidors/studies/1.3.12.2.1107.5.3.4.2373.1.20171103124622/series
        /// will get a 404 page, this method will check if the URL contains uid information.
        /// </summary>
        /// <param name="valueProvider"></param>
        /// <param name="wadoReq"></param>
        private void CheckAndFillUids(IValueProvider valueProvider, IQidoRequestModel wadoReq)
        {
            string studyInstanceUidKey = fo.DicomTag.StudyInstanceUID.DictionaryEntry.Keyword.ToLower();
            string seriesInstanceUidKey = fo.DicomTag.SeriesInstanceUID.DictionaryEntry.Keyword.ToLower();
            string sopInstanceUidKey = fo.DicomTag.SOPInstanceUID.DictionaryEntry.Keyword.ToLower();

            if (!wadoReq.Query.MatchingElements.ContainsKey(studyInstanceUidKey))
            {
                ValueProviderResult valueResult = valueProvider.GetValue(studyInstanceUidKey);
                if (valueResult != null)
                {
                    string studyInstanceUid = valueResult.RawValue as string;
                    if (!string.IsNullOrEmpty(studyInstanceUid))
                        wadoReq.Query.MatchingElements.Add(studyInstanceUidKey, studyInstanceUid);
                }
            }

            if (!wadoReq.Query.MatchingElements.ContainsKey(seriesInstanceUidKey))
            {
                ValueProviderResult valueResult = valueProvider.GetValue(seriesInstanceUidKey);
                if (valueResult != null)
                {
                    string seriesInstanceUid = valueResult.RawValue as string;
                    if (!string.IsNullOrEmpty(seriesInstanceUid))
                        wadoReq.Query.MatchingElements.Add(seriesInstanceUidKey, seriesInstanceUid);
                }
            }

            if (!wadoReq.Query.MatchingElements.ContainsKey(sopInstanceUidKey))
            {
                ValueProviderResult valueResult = valueProvider.GetValue(sopInstanceUidKey);
                if (valueResult != null)
                {
                    string sopInstanceUid = valueResult.RawValue as string;
                    if (!string.IsNullOrEmpty(sopInstanceUid))
                        wadoReq.Query.MatchingElements.Add(sopInstanceUidKey, sopInstanceUid);
                }
            }
        }

        protected virtual IQidoRequestModel CreateModel ( )
        {
            IQidoRequestModel queryModel = new QidoRequestModel();

            queryModel.Query = new QidoQuery();

            return queryModel;
        }

        private WadoBurnAnnotation ParseAnnotation ( string annotationString)
        {
            WadoBurnAnnotation annotation = WadoBurnAnnotation.None ;

            if ( !string.IsNullOrWhiteSpace ( annotationString ) )
            { 
            string[] parts = annotationString.Trim().Split(new string[]{","}, StringSplitOptions.RemoveEmptyEntries) ;
         
            foreach(string part in parts)
            {
                WadoBurnAnnotation tempAnn ; 
               
                if ( Enum.TryParse<WadoBurnAnnotation>(part.Trim(), true, out tempAnn ) )
                { 
                    annotation |= tempAnn ;
                }
            }
            }

            return annotation ;
        }

      private int? GetIntValue ( string stringValue )
      {
         if ( string.IsNullOrWhiteSpace(stringValue))
         {
            return null ;
         }
         else
         { 
            int parsedVal ;

            if ( int.TryParse (stringValue.Trim(), out parsedVal))
            { 
               return parsedVal ;
            }
            else
            { 
               return null ;
            }
         }
      }
   }

   public abstract class QidoRequestKeys
   {
      private QidoRequestKeys (){} 

      public const string FuzzyMatching = "fuzzymatching" ;
      public const string Limit         = "limit" ;
      public const string Offset        = "offset" ;
      public const string IncludeField  = "includefield" ;
   }
}
