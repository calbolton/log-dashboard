using System;
using System.Collections.Generic;
using System.Linq;
using Dashboard.Web._business.Infrastrucure;
using Microsoft.Azure;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.WindowsAzure.Storage;
using Newtonsoft.Json;

namespace Dashboard.Tests
{
    [TestClass]
    public class DevTests
    {
        [TestMethod]
        public void Test()
        {
            var connectionString = CloudConfigurationManager.GetSetting("StorageConnectionString");

            BlobStorageRepository repo = new BlobStorageRepository(new BlobStorageContext()
            {
                ConnectionString = connectionString,
                Container = "dashboardlogs"
            });

            var logs = repo.GetLogsAfter(new DateTime(2016, 6, 16, 15,50, 0));
            var logMessages = logs.Select(x => x.Message).ToList();

            var log = logs.First(x => x.Log.MicroService != "UNCONVERTABLE");


        }

        [TestMethod]
        public void Test2()
        {
            var dt = DateTime.Now;

            var json =
                "2016-06-16T08:34:14,Verbose,CalsDashboard,7a256e,636016628543631261,0,8004,-1,\"{\"\"MicroService\"\":\"\"Micro Service\"\",\"\"Module\"\":\"\"Module\"\",\"\"ThreadId\"\":1,\"\"Action\"\":\"\"Action\"\",\"\"UserId\"\":\"\"UserId\"\",\"\"Type\"\":1,\"\"Message\"\":\"\"Message\"\",\"\"Exception\"\":null,\"\"DateTimeOffset\"\":\"\"2016-06-16T08:34:14.3474966+00:00\"\"}\"";

            var split = Split(json, ",", "\"").ToList();
        }

        public static IEnumerable<string> Split(string input,
                                        string separator,
                                        string escapeCharacter)
        {
            int startOfSegment = 0;
            int index = 0;
            while (index < input.Length)
            {
                index = input.IndexOf(separator, index);
                if (index > 0 && input[index - 1].ToString() == escapeCharacter)
                {
                    index += separator.Length;
                    continue;
                }
                if (index == -1)
                {
                    break;
                }
                yield return input.Substring(startOfSegment, index - startOfSegment);
                index += separator.Length;
                startOfSegment = index;
            }
            yield return input.Substring(startOfSegment);
        }
    }
}
