using DICOMcloud.Wado;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DICOMcloud.UnitTest
{
    class MockRetrieveUrlProvider : IRetrieveUrlProvider
    {
        public string BaseWadoRsUrl { get => "https://localhost/mock/wado-rs"; set {} }
        public string BaseWadoUriUrl { get => "https://localhost/mock/wado-uri"; set {} }
        public bool PreferWadoUri { get => true; set {} }

        public string GetInstanceUrl(IObjectId instance)
        {
            return BaseWadoUriUrl + "//" + instance;
        }

        public string GetStudyUrl(IStudyId study)
        {
            return BaseWadoUriUrl + "//" + study;
        }
    }
}
