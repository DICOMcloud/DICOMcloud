using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DICOMcloud
{
    public class DCloudNotFoundException: DCloudException
    {
        //
        // Summary:
        //     Initializes a new instance of the System.Exception class.
        public DCloudNotFoundException()
        : base ( )
        {
        
        }

        //
        // Summary:
        //     Initializes a new instance of the System.Exception class with a specified error
        //     message.
        //
        // Parameters:
        //   message:
        //     The message that describes the error.
        public DCloudNotFoundException(string message)
        : base ( message )
        {}

        //
        // Summary:
        //     Initializes a new instance of the System.Exception class with a specified error
        //     message and a reference to the inner exception that is the cause of this exception.
        //
        // Parameters:
        //   message:
        //     The error message that explains the reason for the exception.
        //
        //   innerException:
        //     The exception that is the cause of the current exception, or a null reference
        //     (Nothing in Visual Basic) if no inner exception is specified.
        public DCloudNotFoundException(string message, Exception innerException)
        : base ( message, innerException )
        {
        
        }
    }
}
