using System.Collections.Generic;
using Dicom;

namespace DICOMcloud.DataAccess
{
    public interface IDicomDataParameter
    {
        uint KeyTag            { get; set ; }
        DicomVR  VR                { get; set ; }
        bool AllowExtraElement { get; set ; }
        IList<uint> SupportedTags     { get ; }

        bool                 IsSupported     ( DicomItem element );
        void                 SetElement      ( DicomItem element ) ;
        string[]             GetValues       ( ) ;
        List<PersonNameData> GetPNValues     ( ) ;
        IDicomDataParameter  CreateParameter ( ) ;
        IList<DicomItem>  Elements { get; set ; }
    
    }
}
