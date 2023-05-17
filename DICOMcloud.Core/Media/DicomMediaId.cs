using System;
using System.Collections.Generic;
using DICOMcloud.IO;
using DICOMcloud;
using fo = Dicom;

namespace DICOMcloud.Media
{
    //TODO: can be extended to support other formats by passing different 
    //types in the constructor (e.g. json, XML...)
    public class DicomMediaId : IMediaId
    {
        public readonly static int PartsLength = 5 ;
        
        public virtual string    MediaType      { get; set ; }
        public virtual string    TransferSyntax { get; set ; }
        public virtual IObjectId DicomObject    { get; set; }
                
        public DicomMediaId ( ) {}

        public DicomMediaId ( string [] parts ) 
        {
            if ( parts == null || parts.Length != PartsLength ) { throw new ArgumentOutOfRangeException ( "parts array must be " + PartsLength.ToString ( ) ) ; }

            DicomObject = new ObjectId ( ) { StudyInstanceUID = parts[1], SeriesInstanceUID = parts[2], SOPInstanceUID = parts[3], Frame = int.Parse(parts[4]) };

            var mediaTypeParts = parts[0].Split (';') ;

            MediaType = mediaTypeParts[0] ;

            if ( mediaTypeParts.Length == 2 )
            {
                TransferSyntax = mediaTypeParts[1] ;
            }
        }

        public DicomMediaId 
        ( 
            fo.DicomDataset dataset, 
            int frame, 
            string mediaType,
            string transferSyntax
        )
        {
            var dicomObject   = new ObjectId ( ) {
            StudyInstanceUID  = dataset.GetValueOrDefault(fo.DicomTag.StudyInstanceUID, 0, "" ),
            SeriesInstanceUID = dataset.GetValueOrDefault ( fo.DicomTag.SeriesInstanceUID, 0,""), 
            SOPInstanceUID    = dataset.GetValueOrDefault ( fo.DicomTag.SOPInstanceUID, 0, ""),
            Frame             = frame } ;
            
            Init ( dicomObject, mediaType, transferSyntax ) ;
        }

        public DicomMediaId ( IObjectId objectId, DicomMediaProperties mediaInfo )
        {
            Init ( objectId, mediaInfo.MediaType, GetTransferSyntax ( mediaInfo.TransferSyntax ) );
        }

        public virtual string[] GetIdParts ( )
        {
            //TODO: sanitize all parts..... on storage NOT here!
            List<string> parts = new List<string> ( )  ;
            string mediaType = MediaType.Replace ("/", "-").Replace ( "+", "" ) ;

            if ( !string.IsNullOrWhiteSpace(TransferSyntax) )
            {
                mediaType += ";" + TransferSyntax ;
            }

            parts.Add ( mediaType ) ;

            if( string.IsNullOrWhiteSpace ( DicomObject.StudyInstanceUID ) )
            {
                return parts.ToArray ( ) ;
            }

            parts.Add ( DicomObject.StudyInstanceUID ) ;

            if( string.IsNullOrWhiteSpace ( DicomObject.SeriesInstanceUID ) )
            {
                return parts.ToArray ( ) ;
            }

            parts.Add ( DicomObject.SeriesInstanceUID ) ;

            if( string.IsNullOrWhiteSpace ( DicomObject.SOPInstanceUID ) )
            {
                return parts.ToArray ( ) ;
            }

            parts.Add ( DicomObject.SOPInstanceUID ) ;

            if( DicomObject.Frame == null )
            {
                parts.Add ( FIRST_FRAME_NUMER ) ;
            }
            else
            {
                parts.Add ( DicomObject.Frame.ToString ( ) ) ;
            }

            return parts.ToArray ( ) ;
        }
        

        private void Init  ( IObjectId objectId, string mediaType, string transferSyntax )
        {
            DicomObject    = objectId ;
            MediaType      = mediaType ;
            TransferSyntax = transferSyntax?? "" ;
        }

        private static string GetTransferSyntax ( string transferSyntax )
        {
            return (!string.IsNullOrWhiteSpace ( transferSyntax ) ) ? transferSyntax : "" ;
        }

        private const string FIRST_FRAME_NUMER = "1" ;
    }
}
