using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DICOMcloud.Pacs.Commands
{
    public interface IDCloudCommandResult
    {
        CommandStatus Status  { get; set; }
        Exception     Error   { get; set; }
        string        Message { get; set; }
    }

    public class DCloudCommandResult : IDCloudCommandResult
    {
        public CommandStatus Status  { get; set; }
        public Exception     Error   { get; set; }
        public string        Message { get; set; }
    }

    public enum CommandStatus
    {
        Success,
        Failed
    }
}
