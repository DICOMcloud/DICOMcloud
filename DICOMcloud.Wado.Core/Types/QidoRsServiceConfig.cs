using CommunityToolkit.HighPerformance.Helpers;
using Microsoft.Extensions.Configuration;
using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DICOMcloud.Wado.Core.Types
{
    public class QidoRsServiceConfig
    {
        private IConfiguration _configuration;
        private const string MaximumResultsLimit_ConfigName = "qido:maximumResultsLimit";

        public QidoRsServiceConfig(IConfiguration configuration) 
        {
            MaxResultLimit = configuration.GetValue<int?>(MaximumResultsLimit_ConfigName);
        }


        public QidoRsServiceConfig(int maxRestultLimit)
        {
            MaxResultLimit = MaxResultLimit;
        }

        public int? MaxResultLimit { get; set; }
    }
}
