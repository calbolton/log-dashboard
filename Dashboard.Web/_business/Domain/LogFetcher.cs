using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Dashboard.Web.SignalR;
using Dashboard.Web._business.Domain.Infrastructure;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace Dashboard.Web._business.Domain
{
    public class LogFetcher
    {
        // Singleton instance
        private static readonly Lazy<LogFetcher> _instance = new Lazy<LogFetcher>(
            () => new LogFetcher(GlobalHost.ConnectionManager.GetHubContext<LogHub>().Clients));

        private ILogRepository _logRepository;
        private Timer _timer;
        private DateTime _lastUpdatedDateTime = DateTime.Now;//.AddHours(-30);
        private static bool _gettingLogs = false;
        public static bool IsStarted { get; private set; } = false;

        private LogFetcher(IHubConnectionContext<dynamic> clients)
        {
            Logs = new List<AzureLog>();
            Clients = clients;
        }
        public static LogFetcher Instance => _instance.Value;

        private IHubConnectionContext<dynamic> Clients
        {
            get;
            set;
        }

        public ICollection<AzureLog> Logs { get; set; }
        
        public void Start(ILogRepository logRepository, TimeSpan interval, DateTime startDateTime)
        {
            if (IsStarted)
            {
                return;
            }

            IsStarted = true;
            _lastUpdatedDateTime = startDateTime;
            _logRepository = logRepository;
            _timer = new Timer(UpdateLogs, null, interval, interval);
            UpdateLogs(null);
        }

        private void UpdateLogs(object state)
        {
            if (_gettingLogs)
            {
                return;
            }

            try
            {
                _gettingLogs = true;
                var tempLastUpdatedDate = _lastUpdatedDateTime;
                _lastUpdatedDateTime = DateTime.Now;
                var logs = _logRepository.GetLogsAfter(tempLastUpdatedDate);

                var orderedLogs = logs.OrderByDescending(x => x.Date);

                foreach (var azureLog in orderedLogs)
                {
                    Logs.Add(azureLog);
                }

                if (!logs.Any())
                {
                    return;
                }

                Clients.All.logsAdded(logs);
            }
            finally
            {
                _gettingLogs = false;
            }

            //WriteDummyLog();
        }

        private static void WriteDummyLog()
        {
            Trace.WriteLine("Log Created");

            var errorlog = new Log()
            {
                Action = "Error",
                DateTimeOffset = DateTimeOffset.Now,
                Message = "Message",
                MicroService = "Micro Service",
                Module = "Module",
                ThreadId = 1,
                Type = Log.LogType.Error,
                UserId = "UserId"
            };

            var globallog = new Log()
            {
                Action = "global",
                DateTimeOffset = DateTimeOffset.Now,
                Message = "Message",
                MicroService = "Micro Service",
                Module = "Module",
                ThreadId = 1,
                Type = Log.LogType.Global,
                UserId = "UserId"
            };

            var infolog = new Log()
            {
                Action = "warning",
                DateTimeOffset = DateTimeOffset.Now,
                Message = "Message",
                MicroService = "Micro Service",
                Module = "Module",
                ThreadId = 1,
                Type = Log.LogType.Info,
                UserId = "UserId"
            };

            var warninglog = new Log()
            {
                Action = "warning",
                DateTimeOffset = DateTimeOffset.Now,
                Message = "Message",
                MicroService = "Micro Service",
                Module = "Module",
                ThreadId = 1,
                Type = Log.LogType.Warn,
                UserId = "UserId"
            };

            var fatallog = new Log()
            {
                Action = "Fatal",
                DateTimeOffset = DateTimeOffset.Now,
                Message = "Message",
                MicroService = "Micro Service",
                Module = "Module",
                ThreadId = 1,
                Type = Log.LogType.Fatal,
                UserId = "UserId"
            };

            var debuglog = new Log()
            {
                Action = "Debug",
                DateTimeOffset = DateTimeOffset.Now,
                Message = "Message",
                MicroService = "Micro Service",
                Module = "Module",
                ThreadId = 1,
                Type = Log.LogType.Debug,
                UserId = "UserId"
            };

            Trace.WriteLine(errorlog.ToString());
            Trace.WriteLine(globallog.ToString());
            Trace.WriteLine(infolog.ToString());
            Trace.WriteLine(warninglog.ToString());
            Trace.WriteLine(fatallog.ToString());
            Trace.WriteLine(debuglog.ToString());
        }
    }
}