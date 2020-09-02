using DICOMcloud.Wado.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net.Http;
using DICOMcloud.Media;
using DICOMcloud.IO;

namespace DICOMcloud.Wado
{
    public abstract class ObjectHandlerBase : IMimeResponseHandler
    {
        public virtual IMediaStorageService MediaStorage { get; protected set ; }
   
        public abstract bool CanProcess(string mimeType);
      
        public ObjectHandlerBase ( IMediaStorageService mediaStorage, IDicomMediaIdFactory mediaFactory )
        {
            MediaStorage = mediaStorage ;
            MediaFactory = mediaFactory ;
        }

        public virtual IDicomMediaIdFactory MediaFactory
        {
            get; protected set;
        }

        public virtual async Task<IWadoRsResponse> Process (IWadoUriRequest request, string mimeType)
        {
            Location = MediaStorage.GetLocation ( MediaFactory.Create (request, 
                                                                       GetMediaProperties ( request, mimeType, 
                                                                                            GetTransferSyntax ( request ) ) ) ) ;
         
            if ( Location != null && Location.Exists ( ) )
            {
                WadoResponse response = new WadoResponse ( await Location.GetReadStream ( ), mimeType ) ;
                
                return response ;
            }
            else
            {
                //TODO: in case mime not storedmethod to create on 
                return await DoProcess(request, mimeType);
            }
        }


        protected virtual string GetTransferSyntax ( IWadoUriRequest request )
        {
            return (request.ImageRequestInfo != null) ? request.ImageRequestInfo.TransferSyntax : "" ;
        }

        protected virtual DicomMediaProperties GetMediaProperties ( IWadoUriRequest request, string mimeType, string transferSyntax )
        {
            return new DicomMediaProperties {  MediaType = mimeType, TransferSyntax = transferSyntax } ;
        }

      protected abstract Task<WadoResponse> DoProcess(IWadoUriRequest request, string mimeType);

       protected IStorageLocation Location { get; set; }

   }
}
