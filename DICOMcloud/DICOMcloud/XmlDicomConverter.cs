using fo = Dicom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.IO;

namespace DICOMcloud
{
    
    public interface IXmlDicomConverter : IDicomConverter<string>
    {}

    public class XmlDicomConverter : DicomConverterBase, IXmlDicomConverter
    {
        public XmlDicomConverter()
        {
            XmlSettings = new XmlWriterSettings ( ) ;
            XmlSettings.Encoding = new UTF8Encoding ( false ) ; //force utf-8! http://www.timvw.be/2007/01/08/generating-utf-8-with-systemxmlxmlwriter/
            XmlSettings.Indent = true ;
        }

        public XmlWriterSettings XmlSettings
        {
            get;
            private set ;
        }

        public  fo.DicomTransferSyntax TransferSyntax
        {
            get; protected set;
        }

        public string Convert ( fo.DicomDataset ds )
        {
            string result ;

            using (var ms = new MemoryStream())
            {
                using (XmlWriter writer = XmlTextWriter.Create(ms, XmlSettings))
                {
                    writer.WriteStartDocument ( ) ;
                    writer.WriteStartElement  ( Constants.ROOT_ELEMENT_NAME ) ;
                    
                    WriteHeaders  ( ds, writer ) ;
                    WriteChildren ( ds, writer ) ;
                    
                    writer.WriteEndElement ( ) ;

                    writer.Close ( ) ;
                }

                result = Encoding.Default.GetString ( ms.ToArray ( ) ) ;
            }

            return result ;
        }

        public fo.DicomDataset Convert ( string xmlDcm )
        {
            fo.DicomDataset ds       = new fo.DicomDataset( ) ;
            XDocument       document = XDocument.Parse ( xmlDcm ) ;

            ReadChildren(ds, document.Root, 0 );

            fo.DicomFile df = new fo.DicomFile ( ds ) ;

            return ds ;
        }

        #region Write Methods

        protected virtual void WriteHeaders ( fo.DicomDataset ds, XmlWriter writer)
        {
            ds.AddOrUpdate(fo.DicomTag.TransferSyntaxUID, ds.InternalTransferSyntax) ;
//            WriteDicomAttribute ( ds, ds.Get<fo.DicomElement> ( fo.DicomTag.TransferSyntaxUID, null ), writer );
            
        }

        protected virtual void WriteChildren ( fo.DicomDataset ds, XmlWriter writer ) 
        {
            foreach ( var element in ds )
            {
                WriteDicomAttribute ( ds, element, writer ) ;
            }
        }
        
        protected virtual void WriteDicomAttribute 
        ( 
            fo.DicomDataset ds, 
            fo.DicomItem element, 
            XmlWriter writer 
        )
        {
            //group length element must not be written
            if ( null == element || element.Tag.Element == 0x0000 ) { return ; }

            fo.DicomVR dicomVr = element.ValueRepresentation ;


            writer.WriteStartElement ( Constants.ATTRIBUTE_NAME ) ;

            writer.WriteAttributeString ( Constants.ATTRIBUTE_KEYWORD, element.Tag.DictionaryEntry.Keyword ) ;
            writer.WriteAttributeString ( Constants.ATTRIBUTE_TAG, element.Tag.ToString("J", null) ) ;
            writer.WriteAttributeString ( Constants.ATTRIBUTE_VR, element.ValueRepresentation.Code.ToUpper ( ) ) ;

            if ( element.Tag.IsPrivate && null != element.Tag.PrivateCreator ) 
            {
                writer.WriteAttributeString ( Constants.ATTRIBUTE_PRIVATE_CREATOR, element.Tag.PrivateCreator.Creator ) ;
            }

            switch ( element.ValueRepresentation.Code ) 
            {
                case fo.DicomVRCode.SQ:
                {
                    WriteVR_SQ ( ( fo.DicomSequence ) element, writer ) ;
                }
                break ;

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
                    WriteVR_Default ( ds, (fo.DicomElement) element, writer );                
                }
                break;            
            }

            writer.WriteEndElement ( ) ;
        }

        protected virtual void WriteVR_Binary ( fo.DicomItem item, XmlWriter writer )
        {
            fo.IO.Buffer.IByteBuffer buffer = GetItemBuffer ( item );

            if ( buffer is fo.IO.Buffer.IBulkDataUriByteBuffer )
            {
                writer.WriteStartElement ( Constants.ELEMENT_BULKDATA );
                //TODO: what about uuid? how is this represented in foDicom? fo.IO.Buffer.IBulkDataUUIDByteBuffer                
                writer.WriteAttributeString ( Constants.ATTRIBUTE_BULKDATAURI, ( (fo.IO.Buffer.IBulkDataUriByteBuffer) buffer ).BulkDataUri );
                writer.WriteEndElement ( );
            }
            else
            {
                if ( this.WriteInlineBinary )
                {
                    writer.WriteStartElement ( Constants.ELEMENT_INLINEBINARY );
                    WriteStringValue ( writer, buffer.Data );
                    writer.WriteEndElement ( );
                }
            }
        }

        protected virtual void WriteStringValue ( XmlWriter writer, byte[] buffer )
        {
            writer.WriteBase64 ( buffer, 0, buffer.Length ) ;
        }

        protected virtual void WriteVR_SQ (fo.DicomSequence element, XmlWriter writer )
        {
            for ( int index = 0; index < element.Items.Count; index++ )
            {
                var item = element.Items [ index ] ;

                writer.WriteStartElement ( Constants.ATTRIBUTE_ITEM_NAME ) ;
                WriteNumberAttrib(writer, index);
                
                WriteChildren(item, writer);

                writer.WriteEndElement ( ) ;
            }
        }

        protected virtual void WriteVR_PN ( fo.DicomElement element, XmlWriter writer )
        {
            for (int index = 0; index < element.Count; index++)
            {
                writer.WriteStartElement ( Constants.PN_PERSON_NAME );
                    WriteNumberAttrib(writer, index) ;

                    var pnComponents = GetTrimmedString ( element.Get<string> ( ) ).Split ( '=') ;

                    for ( int compIndex = 0; (compIndex < pnComponents.Length) && (compIndex < 3); compIndex++ )
                    {
                        writer.WriteStartElement ( Utilities.PersonNameComponents.PN_Components[compIndex] ) ;

                            fo.DicomPersonName pn = new fo.DicomPersonName ( element.Tag, pnComponents[compIndex]  ) ; 
                            
                            writer.WriteElementString ( Utilities.PersonNameParts.PN_Family, pn.Last ) ;
                            writer.WriteElementString ( Utilities.PersonNameParts.PN_Given, pn.First ) ;
                            writer.WriteElementString ( Utilities.PersonNameParts.PN_Midlle, pn.Middle ) ;
                            writer.WriteElementString ( Utilities.PersonNameParts.PN_Prefix, pn.Prefix ) ;
                            writer.WriteElementString ( Utilities.PersonNameParts.PN_Suffix, pn.Suffix ) ;

                        writer.WriteEndElement ( ) ;
                    }
                writer.WriteEndElement ( ) ;
            }
        }

        protected virtual void WriteVR_Default ( fo.DicomDataset ds, fo.DicomElement element, XmlWriter writer )
        {
            fo.DicomVR dicomVr = element.ValueRepresentation ;


            for ( int index = 0; index < element.Count; index++ )
            {
                writer.WriteStartElement ( Constants.ATTRIBUTE_VALUE_NAME ) ;

                WriteNumberAttrib ( writer, index ) ;
                    
                if ( dicomVr.Equals(fo.DicomVR.AT))
                {
                    var    atElement   = ds.Get<fo.DicomElement>    ( element.Tag, null ) ;
                    var    tagValue    = atElement.Get<fo.DicomTag> ( ) ;
                    string stringValue = tagValue.ToString          ( "J", null ) ;

                    writer.WriteString ( stringValue ) ;
                }
                else
                {
                    writer.WriteString ( GetTrimmedString ( ds.Get<string> ( element.Tag, index, string.Empty ) ) ); 
                }

                writer.WriteEndElement ( );
            }
        }
        
        protected virtual void WriteNumberAttrib(XmlWriter writer, int index)
        {
            writer.WriteAttributeString("number", (index + 1).ToString());
        }

        #endregion

        #region Read Methods
        
        private void ReadChildren ( fo.DicomDataset ds, XContainer document, int level = 0 ) 
        {
            foreach ( var element in document.Elements (Constants.ATTRIBUTE_NAME) ) 
            {
                ReadDicomAttribute(ds, element, level);
            }
        }

        private void ReadDicomAttribute ( fo.DicomDataset ds, XElement element, int level )
        {
            XAttribute              vrNode  ;
            fo.DicomTag             tag ;
            fo.DicomDictionaryEntry dicEntry ;
            fo.DicomVR              dicomVR  ;


            vrNode  = element.Attribute( Constants.ATTRIBUTE_VR ) ;
            tag     = fo.DicomTag.Parse ( element.Attribute(Constants.ATTRIBUTE_TAG).Value ) ;
            dicomVR = null ;
            

            //if ( tag.ToString ("J") == "00020010" )
            //{
            //    ds.InternalTransferSyntax = ReadValue ( element ).FirstOrDefault ( ) ;
            //}

            if ( vrNode != null && !string.IsNullOrEmpty ( vrNode.Value ) )
            {
                dicomVR = fo.DicomVR.Parse ( vrNode.Value ) ;
            }

            if ( tag.IsPrivate ) 
            {
                tag = ds.GetPrivateTag ( tag ) ;
            
                if ( null != vrNode )
                {
                    dicomVR = fo.DicomVR.Parse ( vrNode.Value ) ;
                }
            }
            
            if ( null == dicomVR )
            {
                dicEntry = fo.DicomDictionary.Default[tag];
                dicomVR  = dicEntry.ValueRepresentations.FirstOrDefault ( ) ;
            }

            if ( dicomVR == fo.DicomVR.SQ )
            {
                ReadSequence ( ds, element, tag, level ) ;
            }
            else
            {
                ReadElement ( ds, element, tag, dicomVR, level ) ;
            }

        }

        private void ReadSequence 
        ( 
            fo.DicomDataset ds,  
            XElement element, 
            fo.DicomTag tag, 
            int level 
        )
        {
            fo.DicomSequence seq = new fo.DicomSequence ( tag, new fo.DicomDataset[0] ) ;


            foreach ( var item in  element.Elements ( Constants.ATTRIBUTE_ITEM_NAME ) )
            {
                fo.DicomDataset itemDs = new fo.DicomDataset ( ) ;
                
                level++ ;

                ReadChildren ( itemDs, item, level ) ;

                level--;

                seq.Items.Add ( itemDs ) ;
            }

            ds.AddOrUpdate ( seq ) ;
        }
        
        private void ReadElement 
        ( 
            fo.DicomDataset ds, 
            XElement element, 
            fo.DicomTag tag, 
            fo.DicomVR dicomVr, 
            int level
        )
        {
            if ( dicomVr == fo.DicomVR.PN )
            {
                string personNameValue = "" ;

                foreach ( var personNameElementValue in element.Elements ( ).OrderBy ( n=>n.Attribute (Constants.ATTRIBUTE_NUMBER)))
                {
                    foreach ( var personNameComponent in personNameElementValue.Elements ( ) )
                    {
                        if ( personNameComponent.Name == Utilities.PersonNameComponents.PN_COMP_ALPHABETIC || 
                             personNameComponent.Name == Utilities.PersonNameComponents.PN_COMP_IDEOGRAPHIC || 
                             personNameComponent.Name == Utilities.PersonNameComponents.PN_COMP_PHONETIC )
                        {
                            personNameValue = UpdatePersonName ( personNameValue, personNameComponent, Utilities.PersonNameParts.PN_Family );
                            personNameValue = UpdatePersonName ( personNameValue, personNameComponent, Utilities.PersonNameParts.PN_Given );
                            personNameValue = UpdatePersonName ( personNameValue, personNameComponent, Utilities.PersonNameParts.PN_Midlle );
                            personNameValue = UpdatePersonName ( personNameValue, personNameComponent, Utilities.PersonNameParts.PN_Prefix );
                            personNameValue = UpdatePersonName ( personNameValue, personNameComponent, Utilities.PersonNameParts.PN_Suffix, true );

                            personNameValue = personNameValue.TrimEnd ( '^') ; // extra cleanup 

                            personNameValue += "=";
                        }
                    }

                    personNameValue = personNameValue.TrimEnd ( '=') ;
                    
                    personNameValue += "\\" ;
                }

                personNameValue = personNameValue.TrimEnd ( '\\' ) ;
                ds.AddOrUpdate<string> ( dicomVr, tag, personNameValue ) ;
            }
            else if ( Utilities.IsBinaryVR ( dicomVr ) )
            {
                var dataElement = element.Elements ( ).OfType<XElement> ( ).FirstOrDefault ( ) ;

                if ( null != dataElement ) 
                {
                    fo.IO.Buffer.IByteBuffer data ;


                    if ( dataElement.Name == Constants.ELEMENT_BULKDATA ) 
                    {
                        string uri = dataElement.Attribute(Constants.ATTRIBUTE_BULKDATAURI).Value ;
                        
                        
                        data = new fo.IO.Buffer.BulkDataUriByteBuffer ( uri ) ;
                    }
                    else
                    {
                        var base64 = System.Convert.FromBase64String ( dataElement.Value ) ;
                        
                        
                        data = new fo.IO.Buffer.MemoryByteBuffer ( base64 ) ;
                    }
                    
                    if ( tag == fo.DicomTag.PixelData && level == 0 ) 
                    {
                        ds.AddOrUpdatePixelData ( dicomVr, data, TransferSyntax ) ;
                    }
                    else
                    {
                        ds.AddOrUpdate<fo.IO.Buffer.IByteBuffer> ( dicomVr, tag, data ) ;
                    }
                }
            }
            else 
            {
                var values = ReadValue ( element );
                
                if ( tag == fo.DicomTag.TransferSyntaxUID )
                {
                    TransferSyntax = fo.DicomTransferSyntax.Parse ( values.FirstOrDefault ( ) ) ;
                }

                ds.AddOrUpdate<string> ( dicomVr, tag, values.ToArray ( ) );
            }            
        }

        private static string UpdatePersonName
        ( 
            string personNameValue, 
            XElement personNameComponent, 
            string partName,
            bool isLastPart = false
        )
        {
            XElement partElement = personNameComponent.Element ( partName ) ;


            if ( null == partElement )
            {
                personNameValue += "" ;
            }
            else
            {
                personNameValue += partElement.Value ?? "" ;
            }

            if ( !isLastPart )
            {
                personNameValue += "^" ;
            }

            return personNameValue ;
        }

        private static IList<string> ReadValue ( XElement element )
        {
            SortedList<int,string> values = new SortedList<int, string> ( ) ;
            
            
            foreach ( var valueElement in element.Elements (Constants.ATTRIBUTE_VALUE_NAME) )
            {
                values.Add ( int.Parse ( valueElement.Attribute ( Constants.ATTRIBUTE_NUMBER ).Value ), 
                             valueElement.Value ) ;
            }

            return values.Values ;
        }

        #endregion

        //trimming the padding the only allowed raw value transformation in XML
        //part 19 A.1.1
        private static string GetTrimmedString ( string value )
        {
            return value.TrimEnd (PADDING) ;
        }
         
        //TODO: fo dicom VR has property to read padding char
        private static char[] PADDING = new char[] {'\0',' '};
        
        private static class Constants
        {
            public const string ROOT_ELEMENT_NAME = "NativeDicomModel" ;
            public const string ATTRIBUTE_NAME = "DicomAttribute" ;
            public const string ATTRIBUTE_VALUE_NAME = "Value" ;
            public const string ATTRIBUTE_ITEM_NAME = "Item" ;
            public const string ELEMENT_BULKDATA = "BulkData" ;
            public const string ELEMENT_INLINEBINARY = "InlineBinary" ;

            public const string ATTRIBUTE_TAG = "tag" ;
            public const string ATTRIBUTE_VR = "vr" ;
            public const string ATTRIBUTE_NUMBER = "number" ;
            public const string ATTRIBUTE_KEYWORD = "keyword" ;
            public const string ATTRIBUTE_PRIVATE_CREATOR = "privateCreator" ;
            public const string ATTRIBUTE_BULKDATAURI = "uri" ;

            public const string PN_PERSON_NAME = "PersonName" ;
            
        }
    }
}
