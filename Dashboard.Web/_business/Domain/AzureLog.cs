using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Dashboard.Web._business.Infrastrucure;
using FileHelpers;
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
                    return new Log()
                    {
                        MicroService = "UNCONVERTABLE",
                        Message = Message,
                        Exception = ex
                    };
                }

            }
        }

        public string ActivityId;
        

    }
}