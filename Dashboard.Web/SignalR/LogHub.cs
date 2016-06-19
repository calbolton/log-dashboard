using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Dashboard.Web._business.Domain;
using Dashboard.Web._business.Infrastrucure;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.Azure;

namespace Dashboard.Web.SignalR
{
    [HubName("log")]
    public class LogHub : Hub
    {
        private readonly LogFetcher _logFetcher;
        
        public LogHub()
        {
            if (_logFetcher != null)
                return;

            var connectionString = CloudConfigurationManager.GetSetting("StorageConnectionString");
            var logRepository = new BlobStorageRepository(new BlobStorageContext()
            {
                ConnectionString = connectionString,
                Container = "developmentwebapp"
            });

            _logFetcher = LogFetcher.Instance;
            _logFetcher.Start(logRepository, TimeSpan.FromSeconds(5), DateTime.Now.AddDays(-2));
        }

        public ICollection<AzureLog> GetAllLogs()
        {
            return _logFetcher.Logs;
        } 
    }
}