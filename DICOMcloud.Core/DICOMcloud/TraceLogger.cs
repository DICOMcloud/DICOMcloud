using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using fo = Dicom ;
using Dicom.Log ;
namespace DICOMcloud
{
    /// <summary>
    /// Logger for output to the <see cref="Console"/>.
    /// </summary>
    public class TraceLogger : Logger
    {
        /// <summary>
        /// Singleton instance of the <see cref="TraceLogger"/>.
        /// </summary>
        public static readonly Logger Instance = new TraceLogger();

        private readonly object @lock = new object();

        /// <summary>
        /// Initializes an instance of the <see cref="TraceLogger"/>.
        /// </summary>
        private TraceLogger()
        {
        }

        /// <summary>
        /// Log a message to the logger.
        /// </summary>
        /// <param name="level">Log level.</param>
        /// <param name="msg">Log message (format string).</param>
        /// <param name="args">Log message arguments.</param>
        public override void Log(LogLevel level, string msg, params object[] args)
        {
            lock (this.@lock)
            {
                switch (level)
                {
                    case LogLevel.Debug:
                        System.Diagnostics.Trace.TraceInformation (NameFormatToPositionalFormat(msg), args) ;
                        break;
                    case LogLevel.Info:
                        System.Diagnostics.Trace.TraceInformation (NameFormatToPositionalFormat(msg), args) ;
                        break;
                    case LogLevel.Warning:
                        System.Diagnostics.Trace.TraceWarning (NameFormatToPositionalFormat(msg), args) ;
                        break;
                    case LogLevel.Error:
                        System.Diagnostics.Trace.TraceError (NameFormatToPositionalFormat(msg), args) ;
                        break;
                    case LogLevel.Fatal:
                        System.Diagnostics.Trace.TraceError (NameFormatToPositionalFormat(msg), args) ;
                        break;
                    default:
                        System.Diagnostics.Trace.TraceInformation (NameFormatToPositionalFormat(msg), args) ;
                        break ;
                }

            }
        }
    }

    /// <summary>
    /// Manager for logging to the console.
    /// </summary>
    public class TraceLogManager : LogManager
    {
        /// <summary>
        /// Singleton instance of the <see cref="TraceLogManager"/>.
        /// </summary>
        public static readonly LogManager Instance = new TraceLogManager();

        /// <summary>
        /// Initializes an instance of the <see cref="TraceLogManager"/>.
        /// </summary>
        private TraceLogManager()
        {
        }

        /// <summary>
        /// Get logger from the current log manager implementation.
        /// </summary>
        /// <param name="name">Classifier name, typically namespace or type name.</param>
        /// <returns>Logger from the current log manager implementation.</returns>
        protected override Logger GetLoggerImpl(string name)
        {
            return TraceLogger.Instance;
        }
    }
}
