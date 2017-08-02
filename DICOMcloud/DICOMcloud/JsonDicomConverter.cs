using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using Newtonsoft.Json;
using fo = Dicom;

namespace DICOMcloud
{
    public interface IJsonDicomConverter : IDicomConverter<string>
    {
    }

    public class JsonDicomConverter : DicomConverterBase, IJsonDicomConverter
    {
        private int _minValueIndex;

        public string TransferSyntaxUID
        {
            get; set;
        }

        public JsonDicomConverter ( )
        {
            IncludeEmptyElements = false;
        }

        public bool IncludeEmptyElements
        {
            get
            {
                return ( _minValueIndex == -1 );
            }
            set
            {
                _minValueIndex = ( value ? -1 : 0 );
            }
        }

        public string Convert ( fo.DicomDataset ds )
        {
            StringBuilder sb = new StringBuilder ( );
            StringWriter  sw = new StringWriter  ( sb );


            using ( JsonWriter writer = new JsonTextWriter ( sw ) )
            {
                writer.Formatting = Formatting.Indented;

                writer.WriteStartObject ( );

                WriteHeaders  ( ds, writer ) ;
                WriteChildren ( ds, writer );

                writer.WriteEndObject ( );
            }

            return sb.ToString ( );
        }

        //public fo.DicomDataset Convert ( string jsonDcm )
        //{
        //    return JsonConvert.DeserializeObject<fo.DicomDataset> ( jsonDcm ) ;
        //}
        public fo.DicomDataset Convert ( string jsonDcm )
        {
            fo.DicomDataset dataset = null ;


            using ( var strReader = new StringReader ( jsonDcm ) )
            {
                using ( var reader = new JsonTextReader ( strReader ) )
                {
                    if ( reader.Read ( ) )
                    { 
                        dataset = new fo.DicomDataset ( );
                    
                        if ( reader.TokenType == JsonToken.Null )
                            return null;
                        if ( reader.TokenType != JsonToken.StartObject )
                            throw new JsonReaderException ( "Malformed DICOM json" );

                        ReadChildren ( reader, dataset, 0 );
                    }
                }
            }

            return dataset;
        }

        #region Write Methods

        protected virtual void WriteHeaders ( fo.DicomDataset ds, JsonWriter writer)
        {
            ds.AddOrUpdate(fo.DicomTag.TransferSyntaxUID, ds.InternalTransferSyntax) ;
        }

        protected virtual void WriteChildren ( fo.DicomDataset ds, JsonWriter writer )
        {
            //TODO: add orderby tag val
            foreach ( var element in ds )
            {
                WriteDicomItem ( ds, element, writer );
            }
        }

        protected virtual void WriteDicomItem
        (
            fo.DicomDataset ds,
            fo.DicomItem element,
            JsonWriter writer
        )
        {
            //group length element must not be written
            if ( null == element || element.Tag.Element == 0x0000 )
            {
                return;
            }

            fo.DicomVR dicomVr = element.ValueRepresentation;

            writer.WritePropertyName ( element.Tag.Group.ToString("X4") + element.Tag.Element.ToString("X4") ) ;

            writer.WriteStartObject ( );


            //writer.WritePropertyName ( "temp" );
            //writer.WriteValue ( element.Tag.DictionaryEntry.Keyword );

            writer.WritePropertyName ( "vr" );
            writer.WriteValue ( element.ValueRepresentation.Code );


            switch ( element.ValueRepresentation.Code ) 
            {
                case fo.DicomVRCode.SQ:
                {
                    WriteVR_SQ ( (fo.DicomSequence) element, writer );                
                }
                break;

                case fo.DicomVRCode.PN:
                {
                    WriteVR_PN ( (fo.DicomElement) element, writer );
                }
                break;

                case fo.DicomVRCode.OB:
                case fo.DicomVRCode.OD:
                case fo.DicomVRCode.OF:
                case fo.DicomVRCode.OW:
                case fo.DicomVRCode.OL:
                case fo.DicomVRCode.UN:
                { 
                    WriteVR_Binary ( element, writer );                    
                }
                break;

                default:
                {
                    WriteVR_Default ( ds, (fo.DicomElement) element, writer, dicomVr );                
                }
                break;
            }

            writer.WriteEndObject ( );
        }

        protected virtual void WriteVR_SQ ( fo.DicomSequence element, JsonWriter writer )
        {
            for ( int index = 0; index < element.Items.Count; index++ )
            {
                StringBuilder sqBuilder = new StringBuilder ( );
                StringWriter sw = new StringWriter ( sqBuilder );

                using ( JsonWriter sqWriter = new JsonTextWriter ( sw ) )
                {
                    var item = element.Items[index];
                    
                    
                    sqWriter.Formatting = Formatting.Indented;//TODO: make it an option

                    sqWriter.WriteStartArray ( );

                    sqWriter.WriteStartObject ( );
                    
                    if ( null != item )
                    {
                        WriteChildren ( item, sqWriter );
                    }
                    
                    sqWriter.WriteEndObject ( );
                    sqWriter.WriteEndArray ( );

                }

                WriteSequenceValue ( writer, sqBuilder.ToString ( ) );
            }
        }

        protected virtual void WriteSequenceValue ( JsonWriter writer, string data )
        {
            writer.WritePropertyName ( JsonConstants.ValueField );
            writer.WriteRawValue ( data );
        }

        protected virtual void WriteVR_Default
        ( 
            fo.DicomDataset ds, 
            fo.DicomElement element, 
            JsonWriter writer, 
            fo.DicomVR dicomVr 
        )
        {
            writer.WritePropertyName ( JsonConstants.ValueField );
            writer.WriteStartArray ( );
            
            for ( int index = 0; index < element.Count; index++ )
            {
                string stringValue = GetTrimmedString ( element.Get<string> ( index ) ) ;

                if ( _numberBasedVrs.Contains ( element.ValueRepresentation.Code ) )
                {
                    //parse with the greatest type that can handle
                    //need to do that to remove the ' ' around the string
                    if ( _decimalBasedVrs.Contains ( element.ValueRepresentation.Code ) )
                    {
                        writer.WriteValue ( double.Parse (stringValue, System.Globalization.NumberStyles.Any) );
                    }
                    else
                    {
                        writer.WriteValue ( long.Parse (stringValue, System.Globalization.NumberStyles.Number) );                     
                    }
                }
                else
                {
                    writer.WriteValue ( stringValue ); 
                }
            }
            
            writer.WriteEndArray ( );
        }

        protected virtual void WriteVR_Binary ( fo.DicomItem item, JsonWriter writer )
        {
            fo.IO.Buffer.IByteBuffer buffer = GetItemBuffer ( item );


            if ( buffer is fo.IO.Buffer.IBulkDataUriByteBuffer )
            {
                WriteBinaryValue ( writer, ( (fo.IO.Buffer.IBulkDataUriByteBuffer) buffer ).BulkDataUri, 
                                   JsonConstants.ELEMENT_BULKDATA ) ;
            }
            else
            {
                if ( this.WriteInlineBinary )
                {
                    WriteBinaryValue ( writer, 
                                       System.Convert.ToBase64String ( buffer.Data ), 
                                       JsonConstants.ELEMENT_INLINEBINARY ) ;
                }
            }
        }

        protected virtual void WriteVR_PN ( fo.DicomElement element, JsonWriter writer )
        {
            writer.WritePropertyName ( JsonConstants.ValueField );
            writer.WriteStartArray   (                          );

            for ( int index = 0; index < element.Count; index++ )
            {
                writer.WriteStartObject ( );
                
                var pnComponents = GetTrimmedString ( element.Get<string> ( ) ).Split ( '=' );

                for ( int compIndex = 0; ( compIndex < pnComponents.Length ) && ( compIndex < 3 ); compIndex++ )
                {
                    writer.WritePropertyName ( Utilities.PersonNameComponents.PN_Components[compIndex] );
                    writer.WriteValue        ( GetTrimmedString ( pnComponents[compIndex] )            );
                    writer.WriteEndObject    (                                                         );
                }
            }

            writer.WriteEndArray ( );
        }

        protected virtual void WriteBinaryValue
        ( 
            JsonWriter writer, 
            string data, 
            string propertyName = JsonConstants.ValueField 
        )
        {
            data = data ?? "";

            writer.WritePropertyName ( propertyName );
            writer.WriteValue ( data ); //TODO: can/should trim?
        }

        #endregion

        #region Read Methods
        protected virtual void ReadChildren ( JsonTextReader reader, fo.DicomDataset dataset, int level )
        {
            while ( reader.Read ( ) && reader.TokenType == JsonToken.PropertyName )
            {
                var tag = fo.DicomTag.Parse ( (string) reader.Value );
                
                if ( reader.Read ( ) && reader.TokenType == JsonToken.StartObject )
                {
                    ReadDicomItem ( reader, tag, dataset, level );
                }
            }

            if ( reader.TokenType != JsonToken.EndObject )
                throw new JsonReaderException ( "Malformed DICOM json" );
        }

        protected virtual void ReadDicomItem
        (
            JsonTextReader reader,
            fo.DicomTag tag,
            fo.DicomDataset dataset, 
            int level
        )
        {
            var vr = tag.DictionaryEntry.ValueRepresentations.FirstOrDefault ( );

            while ( reader.Read ( ) && reader.TokenType == JsonToken.PropertyName )
            {
                string propertyValue = (string) reader.Value ;
                
                switch (  propertyValue )
                {
                    case JsonConstants.VrField: 
                    {
                        reader.Read ( );

                        vr = fo.DicomVR.Parse ( (string) reader.Value );
                    }
                    break ;

                    case JsonConstants.ValueField:
                    {
                        ReadDefaultVr ( tag, vr, reader, dataset, level );
                    }
                    break ;

                    case JsonConstants.ELEMENT_BULKDATA:
                    {
                        ReadBulkData ( tag, vr, reader, dataset, level ) ;
                    }
                    break ;

                    case JsonConstants.ELEMENT_INLINEBINARY:
                    {
                        ReadInlineBinary ( tag, vr, reader, dataset, level );
                    }
                    break ;

                    default:
                    {
                        reader.Skip ( ) ;
                    }
                    break ;
                }
            }
        }

        private void ReadBulkData 
        ( 
            fo.DicomTag tag, 
            fo.DicomVR vr, 
            JsonTextReader reader, 
            fo.DicomDataset dataset,
            int level
        )
        {
            fo.IO.Buffer.BulkDataUriByteBuffer data = null ;

            
            if ( reader.Read ( ) )
            {
                string uri  = (string) reader.Value ;
                
                
                if ( !string.IsNullOrEmpty ( uri ) )
                {
                    data = new fo.IO.Buffer.BulkDataUriByteBuffer ( uri ) ;
                }

                if ( tag == fo.DicomTag.PixelData && level == 0 )
                {
                    dataset.AddOrUpdatePixelData ( vr, data, fo.DicomTransferSyntax.Parse ( TransferSyntaxUID ) ) ;
                }
                else
                {
                    dataset.AddOrUpdate<fo.IO.Buffer.IByteBuffer> ( vr, tag, data ) ;
                }
            }
        }

        private void ReadInlineBinary
        (
            fo.DicomTag tag, 
            fo.DicomVR vr, 
            JsonTextReader reader, 
            fo.DicomDataset dataset,
            int level
        )
        {
            if ( reader.Read ( ) )
            {
                fo.IO.Buffer.MemoryByteBuffer buffer = null ;
                byte[] data   = new byte[0] ;
                string base64 = (string) reader.Value ;
            
                
                if ( !string.IsNullOrEmpty ( base64 ) )
                {
                    buffer = new fo.IO.Buffer.MemoryByteBuffer ( System.Convert.FromBase64String ( base64 ) ) ;
                }
            
                if ( tag == fo.DicomTag.PixelData && level == 0 )
                {
                    dataset.AddOrUpdatePixelData ( vr, buffer, fo.DicomTransferSyntax.Parse ( TransferSyntaxUID ) ) ;
                }
                else
                {
                    dataset.AddOrUpdate<fo.IO.Buffer.IByteBuffer> ( vr, tag, buffer ) ;
                }
            }
        }

        protected virtual void ReadDefaultVr
        ( 
            fo.DicomTag tag,
            fo.DicomVR vr, 
            JsonTextReader reader, 
            fo.DicomDataset dataset,
            int level
        )
        {
            //if VR was the first property we already read the right thing above, 
            //otherwise we'll got it from the dictionary. Could it be defined after the value?
            switch ( vr.Code )
            {
                case fo.DicomVRCode.SQ:
                {
                    ReadVr_SQ ( reader, tag, dataset, level );
                }
                break;

                case fo.DicomVRCode.PN:
                {
                    ReadVr_PN ( reader, tag, dataset );
                }
                break;

                default:
                {
                    List<string> values = new List<string> ( );


                    while ( reader.Read ( ) && reader.TokenType == JsonToken.StartArray )
                    {
                        while ( reader.Read ( ) && reader.TokenType != JsonToken.EndArray )
                        {
                            values.Add ( System.Convert.ToString ( reader.Value ) );
                        }

                        break;
                    }

                    if ( tag == fo.DicomTag.TransferSyntaxUID )
                    {
                        TransferSyntaxUID = values.FirstOrDefault ( ) ; 
                    }

                    dataset.AddOrUpdate<string> ( vr, tag, values.ToArray ( ) );                
                }
                break ;
            }

        }

        protected virtual void ReadVr_PN ( JsonTextReader reader, fo.DicomTag tag, fo.DicomDataset dataset )
        {
            List<string> pnNames = new List<string> ( );


            while ( reader.Read ( ) && reader.TokenType == JsonToken.StartArray )
            {
                PersonNameValue personName = new PersonNameValue ( );


                //keep reading until reach end of array
                while ( reader.Read ( ) && reader.TokenType != JsonToken.EndArray )
                {
                    PersonNameReader pnReader = new PersonNameReader ( );


                    while ( reader.Read ( ) && reader.TokenType == JsonToken.PropertyName )
                    {
                        string componentName = (string) reader.Value;
                        string component = "";


                        if ( reader.Read ( ) )
                        {
                            component = (string) reader.Value;
                        }

                        pnReader.Add ( componentName, component );
                    }

                    personName.Add ( pnReader );
                }

                dataset.AddOrUpdate<string> ( tag, personName.ToString ( ) );

                break;
            }
        }

        protected virtual void ReadVr_SQ ( JsonTextReader reader, fo.DicomTag tag, fo.DicomDataset dataset, int level )
        {
            fo.DicomSequence seq = new fo.DicomSequence ( tag, new fo.DicomDataset[0] ) ;


            if ( reader.Value as string == JsonConstants.ValueField )
            {
                while ( reader.Read ( ) && reader.TokenType == JsonToken.StartArray )
                {
                    while ( reader.Read ( ) && reader.TokenType != JsonToken.EndArray )
                    {
                        fo.DicomDataset itemDs = new fo.DicomDataset ( ) ;
                        
                        ReadChildren ( reader, itemDs, ++level ) ;
                        
                        --level ;

                        seq.Items.Add ( itemDs ) ;
                    }
                    
                    break ;
                }
            }

            dataset.AddOrUpdate ( seq ) ;
        }

        #endregion

        private string GetTrimmedString ( string value )
        {
            return value.TrimEnd ( PADDING );
        }

        private static char[] PADDING = new char[] { '\0', ' ' };

        private static string[] _decimalBasedVrs = new string[] { fo.DicomVRCode.DS, fo.DicomVRCode.FL, fo.DicomVRCode.FD} ;
        private static string[] _numberBasedVrs = new string[] { fo.DicomVRCode.DS, fo.DicomVRCode.FL, fo.DicomVRCode.FD, fo.DicomVRCode.IS, 
                                                                 fo.DicomVRCode.SL, fo.DicomVRCode.SS, fo.DicomVRCode.UL, fo.DicomVRCode.US  } ;

        private const string QuoutedStringFormat = "\"{0}\"";
        private const string QuoutedKeyValueStringFormat = "\"{0}\":\"{1}\"";
        private const string QuoutedKeyValueArrayFormat = "\"Value\":[\"{0}\"]";
        private const string SequenceValueFormatted = "\"Value\":[{\"{0}\"}]";
        private const string NumberValueFormatted = "\"Value\":[{1}]";

        private abstract class JsonConstants
        {
            public const string ValueField = "Value";
            public const string VrField = "vr" ;
            public const string Alphabetic = "Alphabetic";
            public const string ELEMENT_BULKDATA = "BulkDataURI" ;
            public const string ELEMENT_INLINEBINARY = "InlineBinary" ;
        }
    }

    internal class PersonNameValue
    {
        private string _rawValue = "" ;


        internal void Add ( PersonNameReader pnReader )
        {
            _rawValue += pnReader.ToString ( ) + "\\" ;
        }

        public override string ToString ( )
        {
            return _rawValue.TrimEnd ( '\\' ) ;
        }
    }

    internal class PersonNameReader  
    {
        public PersonNameReader ( ) 
        {
            __Components = new string [2] ;
        }

        public void Add ( string componentName, string component )
        {
            int index = GetIndex ( componentName ) ;

             __Components[index] = component ;
        }

        private int GetIndex ( string componentName )
        {
            return Array.IndexOf<string> ( _pnComponents, componentName ) ;
        }

        public override string ToString ( )
        {
            string personName = "" ;
        
            foreach ( var component in __Components )
            {
                personName += component + "=" ;
            }    

            return personName.TrimEnd ( '=' );
        }

        private string[] __Components { get; set ; }
        
        private static string[] _pnComponents =  Enum.GetNames (typeof(PersonNameComponents)) ;
        
        public enum PersonNameComponents
        {
            Alphabetic = 0 ,
            Ideographic,
            Phonetic
        }
    }
}
