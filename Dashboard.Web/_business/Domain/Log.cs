using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;

namespace Dashboard.Web._business.Domain
{
    /// <summary>
    /// Represents a log that has occurred
    /// </summary>
    [Serializable]
    public class Log
    {
        /// <summary>
        /// Types of logs
        /// </summary>
        public enum LogType
        {
            Fatal,
            Error,
            Warn,
            Info,
            Debug,
            Global
        }

        /// <summary>
        /// Create a new log
        /// </summary>
        public Log()
        {
            ThreadId = Thread.CurrentThread.ManagedThreadId;
            DateTimeOffset = DateTimeOffset.Now;
        }

        public string MicroService { get; set; }

        public string Module { get; set; }

        public int ThreadId { get; set; }

        public string Action { get; set; }

        public string UserId { get; set; }

        public LogType Type { get; set; }

        public string Message { get; set; }

        public Exception Exception { get; set; }

        public DateTimeOffset DateTimeOffset { get; set; }

        /// <summary>
        /// Converts the log to json
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }
    }
}