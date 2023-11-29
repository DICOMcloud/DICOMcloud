using DICOMcloud.DataAccess;
using DICOMcloud.Media;
using DICOMcloud.Pacs;
using DICOMcloud.Wado.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using DICOMcloud.IO;

using System;
using DICOMcloud.Wado.Core.WadoResponse;
using FellowOakDicom;
using DICOMcloud.Wado.Core.Types;

namespace DICOMcloud.Wado
{
    public class QidoRsService : IQidoRsService
    {
        public int MaximumResultsLimit { get; set; }
        protected IObjectArchieveQueryService QueryService { get; set; }
        protected IDicomMediaIdFactory MediaIdFactory { get; set; }
        protected IMediaStorageService StorageService { get; set; }
        protected QidoRsServiceConfig Config { get; set; }

        public QidoRsService 
        ( 
            IObjectArchieveQueryService queryService, 
            IDicomMediaIdFactory mediaIdFactory, 
            IMediaStorageService storageService,
            QidoRsServiceConfig config
        )
        {
            QueryService   = queryService ;
            MediaIdFactory = mediaIdFactory;
            StorageService = storageService ;
            Config         = config;

            MaximumResultsLimit = config.MaxResultLimit?? 12 ;
        }

        public virtual QidoResponse SearchForStudies
        (
            IQidoRequestModel request
        )
        {
            return SearchForDicomEntity ( request, 
            DefaultDicomQueryElements.GetDefaultStudyQuery(),
            delegate
            ( 
                IObjectArchieveQueryService queryService, 
                DicomDataset dicomRequest, 
                IQidoRequestModel qidoRequest 
            )
            {
                IQueryOptions queryOptions = GetQueryOptions ( qidoRequest ) ;

                return queryService.FindStudiesPaged ( dicomRequest, queryOptions ) ;
            }  ) ;
        }

        public virtual QidoResponse SearchForSeries(IQidoRequestModel request)
        {
            return SearchForDicomEntity ( request, 
            DefaultDicomQueryElements.GetDefaultSeriesQuery ( ),
            delegate 
            ( 
                IObjectArchieveQueryService queryService, 
                DicomDataset dicomRequest, 
                IQidoRequestModel qidoResult
            )
            {
                return queryService.FindSeriesPaged ( dicomRequest, GetQueryOptions ( qidoResult ) ) ;
            }  ) ;
        }

        public virtual QidoResponse SearchForInstances(IQidoRequestModel request)
        {
            return SearchForDicomEntity ( request,
            DefaultDicomQueryElements.GetDefaultInstanceQuery ( ),
            delegate 
            ( 
                IObjectArchieveQueryService queryService, 
                DicomDataset dicomRequest, 
                IQidoRequestModel qidoResult
            )
            {
                return queryService.FindObjectInstancesPaged ( dicomRequest, GetQueryOptions ( qidoResult ) ) ;
            }  ) ;
        }

        protected virtual IQueryOptions CreateNewQueryOptions ( ) 
        {
            return new QueryOptions ( ) ;
        }

        protected virtual IQueryOptions GetQueryOptions ( IQidoRequestModel qidoRequest )
        {
            var queryOptions = CreateNewQueryOptions ( ) ;
            
            queryOptions.Limit = Math.Min ( MaximumResultsLimit, qidoRequest.Limit.HasValue ? qidoRequest.Limit.Value : MaximumResultsLimit ) ;
            queryOptions.Offset = Math.Max ( 0, qidoRequest.Offset.HasValue ? qidoRequest.Offset.Value : 0 ) ;
            
            return queryOptions ;
        }
        
        private QidoResponse SearchForDicomEntity 
        ( 
            IQidoRequestModel request, 
            DicomDataset dicomSource,
            DoQueryDelegate doQuery 
        )
        {
            if ( null != request.Query )
            {
                PagedResult<DicomDataset> result;
                var matchingParams = request.Query.MatchingElements;
                var includeParams = request.Query.IncludeElements;

                foreach (var returnParam in includeParams)
                {
                    InsertDicomElement(dicomSource, returnParam, "");
                }

                foreach (var queryParam in matchingParams)
                {
                    string paramValue = queryParam.Value;


                    InsertDicomElement(dicomSource, queryParam.Key, paramValue);
                }

                
                result = doQuery(QueryService, dicomSource, request);
                
                return new QidoResponse(request, dicomSource, result);
            }

            return null;
        }

        private void InsertDicomElement(DicomDataset dicomRequest, string paramKey, string paramValue)
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

        private void CreateElement(string tagString, DicomDataset dicomRequest, string value)
        {
            // special include fields. Server include all by default.
            if (tagString.ToLower() == "all")
            {
                return;
            }

            uint tag = GetTagValue (tagString);

            var entry = DicomDictionary.Default[(DicomTag)tag];

            if (entry.ValueRepresentations.Where( n => n.Name == DicomVR.UI.Name).FirstOrDefault ( ) != null)
            {
                value = value.Replace (",", "\\");
            }

            if (entry.ValueRepresentations.Contains(DicomVR.SQ))
            {
                dicomRequest.AddOrUpdate(new DicomSequence(tag));
            }
            else
            {
                dicomRequest.AddOrUpdate(tag, value);
            }
        }

        private void CreateSequence(List<string> elements, int currentElementIndex, DicomDataset dicomRequest, string value)
        {
            uint tag = GetTagValue ( elements[currentElementIndex] ) ;
            var dicEntry = DicomDictionary.Default[tag] ;
            DicomSequence sequence ;
            DicomDataset  item ;
            
            dicomRequest.AddOrUpdate ( new DicomSequence ( dicEntry.Tag ) ) ;
            sequence = dicomRequest.GetSequence(dicEntry.Tag);

            item = new DicomDataset ( ).NotValidated();

            sequence.Items.Add ( item ) ;
            
            
            for ( int index = (currentElementIndex+1); index < elements.Count; index++  )
            {
                tag = GetTagValue ( elements[index] ) ;
                
                dicEntry = DicomDictionary.Default[tag] ;

                if (  dicEntry.ValueRepresentations.Contains (DicomVR.SQ) )
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

        private static uint GetTagValue (string tagString)
        {
            uint tag ;


            if ( !Char.IsDigit (tagString[0]))
            {
                var element = FellowOakDicom.DicomDictionary.Default.Where ( n=>n.Keyword.ToLower( ) == tagString.ToLower()).FirstOrDefault ( ) ;

                if ( null == element )
                {
                    throw new DICOMcloud.DCloudException ( "Invalid matching parameter: " + tagString ) ;
                }

                tag = (uint) element.Tag.DictionaryEntry.Tag; 
            }
            else
            {
                tag = uint.Parse (tagString, System.Globalization.NumberStyles.HexNumber) ;
            }

            return tag;
        }

        private delegate PagedResult<DicomDataset> DoQueryDelegate 
        ( 
            IObjectArchieveQueryService queryService, 
            DicomDataset dicomRequest, 
            IQidoRequestModel request
        ) ;
    }
}
