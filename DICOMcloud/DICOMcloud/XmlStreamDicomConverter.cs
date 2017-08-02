using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using fo = Dicom;

namespace DICOMcloud
{
    public interface IXmlStreamDicomConverter : IDicomConverter<Stream>
    {}

    public class XmlStreamDicomConverter : IXmlStreamDicomConverter
    {
        public XmlStreamDicomConverter ( ) : this ( new XmlDicomConverter ( ) )
        { }

        public XmlStreamDicomConverter ( IXmlDicomConverter xmlconverter )
        { 
            XmlConverter = xmlconverter ;
        }

        public Stream Convert(fo.DicomDataset ds)
        {
            return new MemoryStream ( ASCIIEncoding.UTF8.GetBytes ( XmlConverter.Convert (ds)));
        }

        public fo.DicomDataset Convert ( Stream xmlStream )
        {
            StreamReader reader    = new StreamReader (xmlStream) ;
            string       xmlString = reader.ReadToEnd ( ) ;

            return XmlConverter.Convert ( xmlString ) ;
        }

        public IXmlDicomConverter XmlConverter { get; set; }
    }
}
