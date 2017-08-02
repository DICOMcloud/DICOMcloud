using System.Collections.Generic;

using DICOMcloud;


namespace DICOMcloud.Pacs.Commands
{
    public class DeleteCommandData
    {
        public IEnumerable<IObjectId> Instances   { get; set; }
        public ObjectQueryLevel            DeleteLevel { get; set; }
    }
}
