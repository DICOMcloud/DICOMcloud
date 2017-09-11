using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using fo = Dicom;

using DICOMcloud;
using DICOMcloud.Pacs;
using DICOMcloud.Pacs.Commands;
using System.Net;
using Dicom;

namespace DICOMcloud.Wado
{
    public class WadoStoreResponse
    {
        private fo.DicomDataset _dataset ;
        public IRetieveUrlProvider UrlProvider { get; set; }
        public string StudyInstanceUID         { get; private set; }
        public HttpStatusCode HttpStatus       { get; private set ; }
        public string StatusMessage            { get; private set;}

        private bool _successAdded = false ;
        private bool _failureAdded = false ;

        public WadoStoreResponse ( )
        : this ( "", null )
        {

        }

        public WadoStoreResponse ( string studyInstanceUID, IRetieveUrlProvider urlProvider )
        {
            _dataset         = new fo.DicomDataset ( ) ;
            UrlProvider      = urlProvider?? new RetieveUrlProvider ( ) ;
            StudyInstanceUID = studyInstanceUID ;
            HttpStatus       = HttpStatusCode.Unused ;
            StatusMessage    = "" ;
        }

        public fo.DicomDataset GetResponseContent ( )
        {
            _dataset.AddOrUpdate<string>(fo.DicomTag.RetrieveURI, UrlProvider.GetStudyUrl ( StudyInstanceUID ?? "" ) ) ;
        
            return _dataset ;
        }

        public void AddResult ( DicomDataset ds, Exception ex )
        {
            var referencedInstance = GetReferencedInstsance ( ds ) ;
            var failedSeq          = new DicomSequence ( fo.DicomTag.FailedSOPSequence ) ;
            var item               = new DicomDataset ( ) ;


            referencedInstance.Merge ( item ) ;

            _dataset.AddOrUpdate (failedSeq);
            
            failedSeq.Items.Add ( item ) ;
            
            if ( _successAdded )
            {
                HttpStatus = HttpStatusCode.Accepted ;
            }
            else
            {
                SetError ( ex, item ) ;
            }

            _failureAdded = true ;
        }

        
        public void AddResult ( DicomDataset ds )
        {
            var referencedInstance = GetReferencedInstsance ( ds ) ;
            var referencedSeq      = new fo.DicomSequence ( fo.DicomTag.ReferencedInstanceSequence ) ;
            var item               = new fo.DicomDataset ( ) ;


            referencedInstance.Merge ( item ) ;

            _dataset.AddOrUpdate ( referencedSeq ) ;
            referencedSeq.Items.Add ( item ) ;
            
            item.AddOrUpdate<string> (fo.DicomTag.RetrieveURI, UrlProvider.GetInstanceUrl ( DicomObjectIdFactory.Instance.CreateObjectId ( ds ) ) ) ; 
            
            if ( _failureAdded )
            {
                HttpStatus = HttpStatusCode.Accepted ; 
            }
            else
            {
                HttpStatus = HttpStatusCode.OK ;
            }

            _successAdded = true ;
        }
        
        private fo.DicomDataset GetReferencedInstsance ( fo.DicomDataset ds )
        {
            var classUID = ds.Get<fo.DicomElement> ( fo.DicomTag.SOPClassUID, null ) ;
            var sopUID   = ds.Get<fo.DicomElement> ( fo.DicomTag.SOPInstanceUID, null ) ;
            var dataset  = new fo.DicomDataset ( ) ;


            dataset.AddOrUpdate ( classUID ) ;
            dataset.AddOrUpdate ( sopUID ) ;

            return dataset ;
        }
    
        private void SetError(Exception ex, DicomDataset responseDS )
        {
            if ( ex is DICOMcloudException )
            {
                HttpStatus    = HttpStatusCode.Conflict ;
                StatusMessage = ex.Message ;
            }
            else if (ex is KeyNotFoundException)        {HttpStatus = HttpStatusCode.NotFound;}
            else if (ex is ArgumentException)           {HttpStatus = HttpStatusCode.BadRequest;}
            else if (ex is InvalidOperationException)   {HttpStatus = HttpStatusCode.BadRequest;}
            else if (ex is UnauthorizedAccessException) {HttpStatus = HttpStatusCode.Unauthorized;}
            else
            {
                HttpStatus    = HttpStatusCode.InternalServerError ;
                StatusMessage = "" ;
            }

            ////0110 - Processing failure
            responseDS.AddOrUpdate<UInt16> (fo.DicomTag.FailureReason, 272) ; 
        }
    }
}

//6.6.1.3.2.1.2 Failure Reason
//*****************************
//A7xx - Refused out ofResources
//The STOW-RS Service did not store the instance because it was out of resources.

//A9xx - Error: Data Set does notmatch SOP Class
//The STOW-RS Service did not store the instance because the instance does not conform to itsspecified SOP Class.

//Cxxx - Error: Cannotunderstand
//The STOW-RS Service did not store the instance because it cannot understand certain Data Ele-ments.

//C122 - Referenced TransferSyntax not supported
//The STOW-RS Service did not store the instance because it does not support the requestedTransfer Syntax for the instance.

//0110 - Processing failure
//The STOW-RS Service did not store the instance because of a general failure in processing theoperation.

//0122 - Referenced SOP Classnot supported
//The STOW-RS Service did not store the instance because it does not support the requested SOPClass.
//*********************************
