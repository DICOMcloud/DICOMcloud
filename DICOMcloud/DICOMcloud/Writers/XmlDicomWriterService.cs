using fo = Dicom;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace DICOMcloud
{
    public class XmlDicomWriterService
    {
        static XmlDicomWriterService ( )
        {

            _vrWriters = new ConcurrentDictionary<string,IDicomXmlVrWriter> ( ) ;
        }
        public XmlDicomWriterService ( )
        { }

        public XmlDicomWriterService ( fo.DicomElement dicomElement )
        { 
            DicomElement = dicomElement ;
        }

        internal string WriteElement(fo.DicomDataset ds, fo.DicomElement element, XmlWriter writer)
        {
            IDicomXmlVrWriter vrWriter = GetVrWriter ( element ) ;

            return vrWriter.WriteElement ( element, writer ) ;
        }

        private IDicomXmlVrWriter GetVrWriter(fo.DicomElement element)
        {
            return _vrWriters.GetOrAdd ( element.ValueRepresentation.Name, CreateDefualtVrWriter(element.ValueRepresentation));
        }

        protected virtual IDicomXmlVrWriter CreateDefualtVrWriter(fo.DicomVR dicomVr)
        {
            IDicomXmlVrWriter writer = null ;

            if ( !_defaultVrWriters.TryGetValue ( dicomVr.Code, out writer) )
            { 
                throw new ApplicationException ( "Default VR writer not registered!") ;
            }

            return writer ;
        }

    
    
    
        public fo.DicomElement DicomElement { get; set; }
    


        private static ConcurrentDictionary<string,IDicomXmlVrWriter> _vrWriters ;
        private static ConcurrentDictionary<string,IDicomXmlVrWriter> _defaultVrWriters ;

        private static ConcurrentDictionary<string,IDicomXmlVrWriter> CreateDefaultWriters ( )
        {
            ConcurrentDictionary<string,IDicomXmlVrWriter> writers = new ConcurrentDictionary<string, IDicomXmlVrWriter> ( ) ;



            return null ;
        }
    }
}
