using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using Dicom.Imaging;
using Newtonsoft.Json;
using Dicom;

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

        public string Convert ( DicomDataset ds )
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

        //public DicomDataset Convert ( string jsonDcm )
        //{
        //    return JsonConvert.DeserializeObject<DicomDataset> ( jsonDcm ) ;
        //}
        public DicomDataset Convert ( string jsonDcm )
        {
            DicomDataset dataset = null ;


            using ( var strReader = new StringReader ( jsonDcm ) )
            {
                using ( var reader = new JsonTextReader ( strReader ) )
                {
                    if ( reader.Read ( ) )
                    { 
                        dataset = new DicomDataset ( ).NotValidated();
                    
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

        protected virtual void WriteHeaders ( DicomDataset ds, JsonWriter writer)
        {
            ds.AddOrUpdate(DicomTag.TransferSyntaxUID, ds.InternalTransferSyntax) ;
        }

        protected virtual void WriteChildren ( DicomDataset ds, JsonWriter writer )
        {
            //TODO: add orderby tag val
            foreach ( var element in ds )
            {
                WriteDicomItem ( ds, element, writer );
            }
        }

        protected virtual void WriteDicomItem
        (
            DicomDataset ds,
            DicomItem element,
            JsonWriter writer
        )
        {
            //group length element must not be written
            if ( null == element || element.Tag.Element == 0x0000 )
            {
                return;
            }

            DicomVR dicomVr = element.ValueRepresentation;

            writer.WritePropertyName ( element.Tag.Group.ToString("X4") + element.Tag.Element.ToString("X4") ) ;

            writer.WriteStartObject ( );


            //writer.WritePropertyName ( "temp" );
            //writer.WriteValue ( element.Tag.DictionaryEntry.Keyword );

            writer.WritePropertyName ( "vr" );
            writer.WriteValue ( element.ValueRepresentation.Code );


            switch ( element.ValueRepresentation.Code ) 
            {
                case DicomVRCode.SQ:
                {
                    WriteVR_SQ ( (DicomSequence) element, writer );                
                }
                break;

                case DicomVRCode.PN:
                {
                    WriteVR_PN ( (DicomElement) element, writer );
                }
                break;

                case DicomVRCode.OB:
                case DicomVRCode.OD:
                case DicomVRCode.OF:
                case DicomVRCode.OW:
                case DicomVRCode.OL:
                case DicomVRCode.UN:
                { 
                    WriteVR_Binary ( element, writer );                    
                }
                break;

                default:
                {
                    WriteVR_Default ( ds, (DicomElement) element, writer, dicomVr );                
                }
                break;
            }

            writer.WriteEndObject ( );
        }

        protected virtual void WriteVR_SQ ( DicomSequence element, JsonWriter writer )
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
            DicomDataset ds, 
            DicomElement element, 
            JsonWriter writer, 
            DicomVR dicomVr 
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
                        writer.WriteValue ( double.Parse (stringValue, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture) );
                    }
                    else
                    {
                        writer.WriteValue ( long.Parse (stringValue, System.Globalization.NumberStyles.Number, System.Globalization.CultureInfo.InvariantCulture) );                     
                    }
                }
                else
                {
                    writer.WriteValue ( stringValue ); 
                }
            }
            
            writer.WriteEndArray ( );
        }

        protected virtual void WriteVR_Binary ( DicomItem item, JsonWriter writer )
        {
            Dicom.IO.Buffer.IByteBuffer buffer = GetItemBuffer ( item );


            if ( buffer is Dicom.IO.Buffer.IBulkDataUriByteBuffer )
            {
                WriteBinaryValue ( writer, ( (Dicom.IO.Buffer.IBulkDataUriByteBuffer) buffer ).BulkDataUri, 
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

        protected virtual void WriteVR_PN ( DicomElement element, JsonWriter writer )
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
        protected virtual void ReadChildren ( JsonTextReader reader, DicomDataset dataset, int level )
        {
            while ( reader.Read ( ) && reader.TokenType == JsonToken.PropertyName )
            {
                var tag = DicomTag.Parse ( (string) reader.Value );
                
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
            DicomTag tag,
            DicomDataset dataset, 
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

                        vr = DicomVR.Parse ( (string) reader.Value );
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
            DicomTag tag, 
            DicomVR vr, 
            JsonTextReader reader, 
            DicomDataset dataset,
            int level
        )
        {
            Dicom.IO.Buffer.BulkDataUriByteBuffer data = null ;

            
            if ( reader.Read ( ) )
            {
                string uri  = (string) reader.Value ;
                
                
                if ( !string.IsNullOrEmpty ( uri ) )
                {
                    data = new Dicom.IO.Buffer.BulkDataUriByteBuffer ( uri ) ;
                }

                if ( tag == DicomTag.PixelData && level == 0 )
                {
                  var pixelData= DicomPixelData.Create(dataset, true);  //2nd parameter is true since we are adding new data here
                    pixelData.AddFrame(data);

                }
                else
                {
                    dataset.AddOrUpdate<Dicom.IO.Buffer.IByteBuffer> ( vr, tag, data ) ;
                }
            }
        }

        private void ReadInlineBinary
        (
            DicomTag tag, 
            DicomVR vr, 
            JsonTextReader reader, 
            DicomDataset dataset,
            int level
        )
        {
            if ( reader.Read ( ) )
            {
                Dicom.IO.Buffer.MemoryByteBuffer buffer = null ;
                string base64 = (string) reader.Value ;
            
                
                if ( !string.IsNullOrEmpty ( base64 ) )
                {
                    buffer = new Dicom.IO.Buffer.MemoryByteBuffer ( System.Convert.FromBase64String ( base64 ) ) ;
                }
            
                if ( tag == DicomTag.PixelData && level == 0 )
                {
                    
                    var pixelData= DicomPixelData.Create(dataset, true);  //2nd parameter is true since we are adding new data here
                 
                    pixelData.AddFrame(buffer);
                }
                else
                {
                    dataset.AddOrUpdate<Dicom.IO.Buffer.IByteBuffer> ( vr, tag, buffer ) ;
                }
            }
        }

        protected virtual void ReadDefaultVr
        ( 
            DicomTag tag,
            DicomVR vr, 
            JsonTextReader reader, 
            DicomDataset dataset,
            int level
        )
        {
            //if VR was the first property we already read the right thing above, 
            //otherwise we'll got it from the dictionary. Could it be defined after the value?


            switch ( vr.Code )
            {
                case DicomVRCode.SQ:
                {
                    ReadVr_SQ ( reader, tag, dataset, level );
                }
                break;

                case DicomVRCode.PN:
                {
                    ReadVr_PN ( reader, tag, dataset );
                }
                break;

                case DicomVRCode.DS:
                {
                    ReadVr_DS(reader, tag, dataset);
                }
                break;

                default:
                {
                    List<string> values = new List<string> ( );


                    while ( reader.Read ( ) && reader.TokenType == JsonToken.StartArray )
                    {
                        while ( reader.Read ( ) && reader.TokenType != JsonToken.EndArray )
                        {
                            values.Add ( System.Convert.ToString ( reader.Value ));
                        }

                        break;
                    }

                    if ( tag == DicomTag.TransferSyntaxUID )
                    {
                        TransferSyntaxUID = values.FirstOrDefault ( ) ; 
                    }

                    dataset.AddOrUpdate<string> ( vr, tag, System.Text.Encoding.Default ,values.ToArray ( ) );                
                }
                break ;
            }

        }


        protected virtual void ReadVr_DS(JsonTextReader reader, DicomTag tag, DicomDataset dataset)
        {

            List<string> values = new List<string>();

            while (reader.Read() && reader.TokenType == JsonToken.StartArray)
            {
                while (reader.Read() && reader.TokenType != JsonToken.EndArray)
                {
                    values.Add(System.Convert.ToString(reader.Value, System.Globalization.CultureInfo.InvariantCulture));
                }

                break;
            }

            dataset.AddOrUpdate<string>(DicomVR.DS, tag, values.ToArray());
        }

        protected virtual void ReadVr_PN ( JsonTextReader reader, DicomTag tag, DicomDataset dataset )
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

                dataset.AddOrUpdate<string> ( tag,System.Text.Encoding.Default , personName.ToString ( ) );

                break;
            }
        }

        protected virtual void ReadVr_SQ ( JsonTextReader reader, DicomTag tag, DicomDataset dataset, int level )
        {
            DicomSequence seq = new DicomSequence ( tag, new DicomDataset[0] ) ;


            if ( reader.Value as string == JsonConstants.ValueField )
            {
                while ( reader.Read ( ) && reader.TokenType == JsonToken.StartArray )
                {
                    while ( reader.Read ( ) && reader.TokenType != JsonToken.EndArray )
                    {
                        DicomDataset itemDs = new DicomDataset ( ).NotValidated();
                        
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

        private static string[] _decimalBasedVrs = new string[] { DicomVRCode.DS, DicomVRCode.FL, DicomVRCode.FD} ;
        private static string[] _numberBasedVrs = new string[] { DicomVRCode.DS, DicomVRCode.FL, DicomVRCode.FD, DicomVRCode.IS, 
                                                                 DicomVRCode.SL, DicomVRCode.SS, DicomVRCode.UL, DicomVRCode.US  } ;

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
