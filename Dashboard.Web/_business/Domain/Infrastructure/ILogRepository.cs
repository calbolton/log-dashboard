using System;
using System.Collections.Generic;

namespace Dashboard.Web._business.Domain.Infrastructure
{
    public interface ILogRepository
    {
        ICollection<AzureLog> GetAllLogs();

        ICollection<AzureLog> GetLogsAfter(DateTime afterDate);
    }
}