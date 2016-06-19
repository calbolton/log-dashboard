using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Dashboard.Web._business.Infrastrucure;
using FileHelpers;
using Microsoft.WindowsAzure.Storage;
using Newtonsoft.Json;

namespace Dashboard.Web._business.Domain
{
    [DelimitedRecord(",")]
    public class AzureLog
    {//2016-06-15T19:43:56
        [FieldConverter(ConverterKind.Date, "yyyy-MM-ddTHH:mm:ss")]
        public DateTime Date;

        public string Level;

        public string ApplicationName;

        public string InstanceId;

        [FieldConverter(ConverterKind.Int64)]
        public long EventTickCount;

        [FieldConverter(ConverterKind.Int32)]
        public int EventId;

        [FieldConverter(ConverterKind.Int32)]
        public int PId;

        [FieldConverter(ConverterKind.Int32)]
        public int TId;

        [FieldConverter(typeof(LogConverter))]
        public string Message;

        public Log Log
        {
            get
            {
                try
                {
                    return JsonConvert.DeserializeObject<Log>(Message);
                }
                catch (Exception ex)
                {

                    try // Connvert it to event
                    {
                        var ev = JsonConvert.DeserializeObject<DomainEvent>(Message);
                        return new Log()
                        {
                            MicroService = "Event",
                            Message = Message,
                            Type = Log.LogType.Info,
                            Action = ev.Name,
                            DateTimeOffset = ev.DateTime,
                            UserId = ev.UserId,
                            Exception = ex,
                            Module = ev.Type

                        };
                    }
                    catch (Exception)
                    {
                        // Create general azure log
                        var level = Log.LogType.Info;
                        if (Level == LogLevel.Error.ToString())
                        {
                            level = Log.LogType.Error;
                        }
                        else if (Level == "Information")
                        {
                            level = Log.LogType.Info;
                        }
                        else if (Level == LogLevel.Off.ToString())
                        {
                            level = Log.LogType.Global;
                        }
                        else if (Level == LogLevel.Verbose.ToString())
                        {
                            level = Log.LogType.Info;
                        }
                        else if (Level == LogLevel.Warning.ToString())
                        {
                            level = Log.LogType.Warn;
                        }
                        return new Log()
                        {
                            MicroService = "Azure",
                            Message = Message,
                            Type = level,
                            DateTimeOffset = Date,
                            Action = ApplicationName
                        };
                    }
                }

            }
        }

        public string ActivityId;
        

    }
}