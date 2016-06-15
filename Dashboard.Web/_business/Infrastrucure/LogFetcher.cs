using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using Dashboard.Web.SignalR;
using Dashboard.Web._business.Domain;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace Dashboard.Web._business.Infrastrucure
{
    public class LogFetcher
    {
        // Singleton instance
        private static readonly Lazy<LogFetcher> _instance = new Lazy<LogFetcher>(
            () => new LogFetcher(GlobalHost.ConnectionManager.GetHubContext<LogHub>().Clients));

        private Timer _timer;
        private readonly TimeSpan _updateInterval = TimeSpan.FromMilliseconds(30000);

        private LogFetcher(IHubConnectionContext<dynamic> clients)
        {
            Logs = new List<Log>();
            Clients = clients;
        }
        public static LogFetcher Instance => _instance.Value;

        private IHubConnectionContext<dynamic> Clients
        {
            get;
            set;
        }

        public ICollection<Log> Logs { get; set; }
        
        public void Start()
        {
            _timer = new Timer(UpdateLogs, null, _updateInterval, _updateInterval);
        }

        private void UpdateLogs(object state)
        {
            Logs.Add(new Log() {Description = "LOG"});
            Clients.All.logsAdded(Logs);
        }
        
    }
}