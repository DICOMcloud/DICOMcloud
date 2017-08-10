using DICOMcloud.DataAccess;
using DICOMcloud.Media;
using DICOMcloud.Pacs;
using DICOMcloud.Wado.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using fo = Dicom;

namespace DICOMcloud.Wado
{
    public class QidoRsService : IQidoRsService
    {
        protected IObjectArchieveQueryService QueryService { get; set; }

        public QidoRsService ( IObjectArchieveQueryService queryService )
        {
            QueryService = queryService ;
        }

        public HttpResponseMessage SearchForStudies
        (
            IQidoRequestModel request
        )
        {
            return SearchForDicomEntity ( request, 
            DefaultDicomQueryElements.GetDefaultStudyQuery(),
            delegate 
            ( 
                IObjectArchieveQueryService queryService, 
                fo.DicomDataset dicomRequest, 
                IQidoRequestModel qidoRequest 
            )
            {
                IQueryOptions queryOptions = GetQueryOptions ( qidoRequest ) ;

                return queryService.FindStudies ( dicomRequest, queryOptions ) ;
            }  ) ;
        }

        public HttpResponseMessage SearchForSeries(IQidoRequestModel request)
        {
            return SearchForDicomEntity ( request, 
            DefaultDicomQueryElements.GetDefaultSeriesQuery ( ),
            delegate 
            ( 
                IObjectArchieveQueryService queryService, 
                fo.DicomDataset dicomRequest, 
                IQidoRequestModel qidoResult
            )
            {
                return queryService.FindSeries ( dicomRequest, GetQueryOptions ( qidoResult ) ) ;
            }  ) ;
        }

        public HttpResponseMessage SearchForInstances(IQidoRequestModel request)
        {
            return SearchForDicomEntity ( request,
            DefaultDicomQueryElements.GetDefaultInstanceQuery ( ),
            delegate 
            ( 
                IObjectArchieveQueryService queryService, 
                fo.DicomDataset dicomRequest, 
                IQidoRequestModel qidoResult
            )
            {
                return queryService.FindObjectInstances ( dicomRequest, GetQueryOptions ( qidoResult ) ) ;
            }  ) ;
        }

        protected virtual IQueryOptions CreateNewQueryOptions ( ) 
        {
            return new QueryOptions ( ) ;
        }

        protected virtual IQueryOptions GetQueryOptions ( IQidoRequestModel qidoRequest )
        {
            var queryOptions = CreateNewQueryOptions ( ) ;
            
            queryOptions.Limit = qidoRequest.Limit ;
            queryOptions.Offset = qidoRequest.Offset ;

            return queryOptions ;
        }
        
        private HttpResponseMessage SearchForDicomEntity 
        ( 
            IQidoRequestModel request, 
            fo.DicomDataset dicomSource,
            DoQueryDelegate doQuery 
        )
        {
            if ( null != request.Query )
            {
                var matchingParams = request.Query.MatchingElements ;
                var includeParams = request.Query.IncludeElements ;

                foreach ( var queryParam in  matchingParams )
                {
                    string paramValue = queryParam.Value;


                    InsertDicomElement ( dicomSource, queryParam.Key, paramValue);
                }

                foreach ( var returnParam in includeParams )
                {
                    InsertDicomElement ( dicomSource,  returnParam, "" );
                }

                ICollection<fo.DicomDataset> results = doQuery (QueryService, dicomSource, request) ; //TODO: move configuration params into their own object

                if ( MultipartResponseHelper.IsMultiPartRequest ( request ) )
                {
                    if ( MultipartResponseHelper.GetSubMediaType ( request.AcceptHeader.FirstOrDefault ( ) ) == MimeMediaTypes.xmlDicom )
                    {
                        HttpResponseMessage response ;
                        MultipartContent multiContent ;
                        

                        response        = new HttpResponseMessage ( ) ;
                        multiContent    = new MultipartContent ( "related", MultipartResponseHelper.DicomDataBoundary ) ;           
                        
                        response.Content = multiContent ;

                        foreach ( var result in results)
                        {
                            XmlDicomConverter converter = new XmlDicomConverter ( ) ;

                            MultipartResponseHelper.AddMultipartContent ( multiContent, 
                                                                          new WadoResponse ( new MemoryStream ( Encoding.ASCII.GetBytes ( converter.Convert (result) )), 
                                                                                             MimeMediaTypes.xmlDicom ) ) ;
                        }

                        multiContent.Headers.ContentType.Parameters.Add ( new System.Net.Http.Headers.NameValueHeaderValue ( "type", 
                                                                                                    "\"" + MimeMediaTypes.xmlDicom + "\"" ) ) ;
                    
                        return response ;                                                                                
                    }
                    else
                    {
                        return new HttpResponseMessage ( System.Net.HttpStatusCode.BadRequest ) ;
                    }
                }
                else
                {
                    StringBuilder jsonReturn = new StringBuilder ( "[" ) ;

                    JsonDicomConverter converter = new JsonDicomConverter ( ) { IncludeEmptyElements = true } ;

                    foreach ( var response in results )
                    {
                    
                        jsonReturn.AppendLine (converter.Convert ( response )) ;

                        jsonReturn.Append(",") ;
                    }

                    if ( results.Count > 0 )
                    {
                        jsonReturn.Remove ( jsonReturn.Length -1, 1 ) ;
                    }

                    jsonReturn.Append("]") ;
                
                    return new HttpResponseMessage (System.Net.HttpStatusCode.OK )  { 
                                                    Content = new StringContent ( jsonReturn.ToString ( ), 
                                                    Encoding.UTF8, 
                                                    MimeMediaTypes.Json) } ;    
                }
            }

            return null;
        }

        private void InsertDicomElement(fo.DicomDataset dicomRequest, string paramKey, string paramValue)
        {
            List<string> elements = new List<string>();

            elements.AddRange(paramKey.Split('.'));

            if(elements.Count > 1)
            {
                CreateSequence(elements, 0, dicomRequest, paramValue);
            }
            else
            {
                CreateElement(elements[0], dicomRequest, paramValue);
            }
        }

        private void CreateElement(string tagString, fo.DicomDataset dicomRequest, string value)
        {
            uint tag = uint.Parse (tagString, System.Globalization.NumberStyles.HexNumber) ;

            dicomRequest.AddOrUpdate(tag, value ) ;
        }

        private void CreateSequence(List<string> elements, int currentElementIndex, fo.DicomDataset dicomRequest, string value)
        {
            uint tag = uint.Parse ( elements[currentElementIndex], System.Globalization.NumberStyles.HexNumber) ;//TODO: need to handle the case of keywords
            var dicEntry = fo.DicomDictionary.Default[tag] ;
            fo.DicomSequence sequence ;
            fo.DicomDataset  item ;
            
            dicomRequest.AddOrUpdate ( new fo.DicomSequence ( dicEntry.Tag ) ) ;
            sequence = dicomRequest.Get<fo.DicomSequence>(dicEntry.Tag);


            item = new fo.DicomDataset ( ) ;

            sequence.Items.Add ( item ) ;
            
            
            for ( int index = (currentElementIndex+1); index < elements.Count; index++  )
            {
                tag = uint.Parse ( elements[index], System.Globalization.NumberStyles.HexNumber) ;
                
                dicEntry = fo.DicomDictionary.Default[tag] ;

                if (  dicEntry.ValueRepresentations.Contains (fo.DicomVR.SQ) )
                {
                    CreateSequence ( elements, index, item, value) ;

                    break ;
                }
                else
                {
                    item.AddOrUpdate<string> ( tag, value) ;
                }
            }
        }
    
        private delegate ICollection<fo.DicomDataset> DoQueryDelegate 
        ( 
            IObjectArchieveQueryService queryService, 
            fo.DicomDataset dicomRequest, 
            IQidoRequestModel request
        ) ;
    }
}
