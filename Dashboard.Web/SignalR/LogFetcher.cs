using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Dashboard.Web._business.Domain;
using Dashboard.Web._business.Domain.Infrastructure;
using Dashboard.Web._business.Infrastrucure;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.Azure;

namespace Dashboard.Web.SignalR
{
    public class LogFetcher
    {
        // Singleton instance
        private static readonly Lazy<LogFetcher> _instance = new Lazy<LogFetcher>(
            () => new LogFetcher(GlobalHost.ConnectionManager.GetHubContext<LogHub>().Clients));

        private ILogRepository _logRepository;
        private Timer _timer;
        private readonly TimeSpan _updateInterval;
        private DateTime _lastUpdatedDateTime = DateTime.Now.AddHours(-1);

        private LogFetcher(IHubConnectionContext<dynamic> clients)
        {
            _updateInterval = TimeSpan.FromMilliseconds(30000);
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
        
        public void Start(ILogRepository logRepository)
        {
            _logRepository = logRepository;
            _timer = new Timer(UpdateLogs, null, _updateInterval, _updateInterval);
            UpdateLogs(null);
        }

        private void UpdateLogs(object state)
        {
            var logs = _logRepository.GetLogsAfter(_lastUpdatedDateTime);
            _lastUpdatedDateTime = DateTime.Now;

            foreach (var azureLog in logs)
            {
                Logs.Add(azureLog);
            }

            Clients.All.logsAdded(logs);

            WriteDummyLog();
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