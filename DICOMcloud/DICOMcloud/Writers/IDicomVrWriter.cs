using fo = Dicom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace DICOMcloud
{
    public interface IDicomVrWriter<T,N>
    {
        T WriteElement ( fo.DicomItem element, N writer ) ;
    }

    public interface IDicomXmlVrWriter : IDicomVrWriter<string,XmlWriter>
    {
//        string WriteElement ( fo.DicomItem element, XmlWriter writer ) ;
    }
}
