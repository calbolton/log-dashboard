using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Dashboard.Web._business.Infrastrucure;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace Dashboard.Web.SignalR
{
    [HubName("log")]
    public class LogHub : Hub
    {
        public LogHub()
        {
            LogFetcher.Instance.Start();
        }
    }
}