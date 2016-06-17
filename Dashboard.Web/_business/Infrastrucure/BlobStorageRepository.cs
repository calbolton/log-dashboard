using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using Dashboard.Web._business.Domain;
using Dashboard.Web._business.Domain.Infrastructure;
using FileHelpers;
using FileHelpers.Events;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json;

namespace Dashboard.Web._business.Infrastrucure
{
    public class BlobStorageRepository : ILogRepository
    {
        private readonly BlobStorageContext _context;
        private FileHelperEngine<AzureLog> _engine;

        public BlobStorageRepository(BlobStorageContext context)
        {
            this._context = context;
            

            // Create csv engine
            _engine = new FileHelperEngine<AzureLog>();
            _engine.Options.IgnoreFirstLines = 1;
            _engine.BeforeReadRecord += BeforeEvent;
            _engine.AfterReadRecord += AfterEvent;
        }

        public CloudBlobContainer Container {
            get
            {
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(_context.ConnectionString);
                // Create the blob client.
                CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

                // Retrieve reference to a previously created container.
                return blobClient.GetContainerReference(_context.Container);
            }
        }

        public ICollection<AzureLog> GetAllLogs()
        {
            var retLogs = new List<AzureLog>();

            var blobReferences = GetAllBlobReferences(DateTime.MinValue);

            foreach (var blobReference in blobReferences)
            {
                // Retrieve reference to a blob named "myblob.txt"
                CloudBlockBlob blockBlob2 = Container.GetBlockBlobReference(blobReference);
                var azureLogs = GetLogs(blockBlob2);
                retLogs.AddRange(azureLogs);

            }

            return retLogs;
        }

        private ICollection<AzureLog> GetLogs(CloudBlockBlob blockBlob2)
        {
            var retLogs = new List<AzureLog>();
            using (var memoryStream = new MemoryStream())
            {
                try
                {
                    // Download stream
                    blockBlob2.DownloadToStream(memoryStream);

                    // Convert using string
                    var csvString = Encoding.ASCII.GetString(memoryStream.ToArray());
                    var stringRecords = _engine.ReadString(csvString);

                    retLogs.AddRange(stringRecords);
                }
                catch (Exception ex)
                {
                    retLogs.Add(new AzureLog()
                    {
                        ApplicationName = "ADJUVANT",
                        Message = ex.Message
                    });
                }
            }

            return retLogs;
        }

        public ICollection<AzureLog> GetLogsAfter(DateTime fromDate)
        {
            var retLogs = new List<AzureLog>();

            var blobReferences = GetAllBlobReferences(fromDate);

            foreach (var blobReference in blobReferences)
            {
                // Retrieve reference to blob
                CloudBlockBlob blockBlob2 = Container.GetBlockBlobReference(blobReference);
                
                var azureLogs = GetLogs(blockBlob2);
                var recordsInRange = azureLogs.Where(x => DateTime.Compare(x.Date, fromDate.ToUniversalTime()) > 0);
                retLogs.AddRange(recordsInRange);
            }

            return retLogs;
        }

        

        private void BeforeEvent(EngineBase engine, BeforeReadEventArgs<AzureLog> e)
        {


            var openingBracket = e.RecordLine.IndexOf("{", StringComparison.Ordinal);

            if (openingBracket >= 0)
            {
                var closingBracket = e.RecordLine.LastIndexOf("}", StringComparison.Ordinal);

                if (closingBracket < 0)
                {
                    return;
                }

                var ss = e.RecordLine.Substring(openingBracket - 1, closingBracket - openingBracket + 3);

                //var l = JsonConvert.DeserializeObject<Log>(ss);

                StringBuilder myStringBuilder = new StringBuilder(e.RecordLine);
                myStringBuilder.Replace(",", "|", openingBracket, closingBracket - openingBracket + 1);
                e.RecordLine = myStringBuilder.ToString();

            }
            //  Sometimes changing the record line can be useful, for example to correct for
            //  a bad data layout.  Here is an example of this, commented out for this example

            //if (e.RecordLine.StartsWith(" "))
            //   e.RecordLine = "Be careful!";
        }


        private void AfterEvent(EngineBase engine, AfterReadEventArgs<AzureLog> e)
        {


            e.Record.Message = e.Record.Message?.Replace("|", ",");
            e.Record.Message = e.Record.Message?.Replace("\"{\"", "{");
            e.Record.Message = e.Record.Message?.Replace("\"}\"", "}");
            e.Record.Message = e.Record.Message?.Replace("\"\"", "\"");
            //var l = JsonConvert.DeserializeObject<Log>(e.Record.Message);
            //e.Record.Message = e.Record.Message.Replace("\":\"", ":");
            //e.Record.Message = e.Record.Message.Replace("\",\"", ",");
        }

        private ICollection<string> GetAllBlobReferences(DateTime lastModifiedDate)
        {
            var retBlobReferences = new List<string>();

            // Loop over items within the container and output the length and URI.
            foreach (IListBlobItem item in Container.ListBlobs(null, true))
            {
                

                if (item.GetType() == typeof(CloudBlockBlob))
                {
                    CloudBlockBlob blob = (CloudBlockBlob)item;

                    // Check if modified after date
                    if (blob.Properties.LastModified == null)
                        continue;

                    var modifiedOffset = (DateTimeOffset) blob.Properties.LastModified;
                    
                    if (DateTime.Compare(modifiedOffset.DateTime, lastModifiedDate.ToUniversalTime()) < 0)
                        continue;

                    retBlobReferences.Add(blob.Name);

                }
                else if (item.GetType() == typeof(CloudPageBlob))
                {
                    CloudPageBlob blob = (CloudPageBlob)item;

                    // Check if modified after date
                    if (blob.Properties.LastModified == null)
                        continue;

                    var modifiedOffset = (DateTimeOffset)blob.Properties.LastModified;



                    if (DateTime.Compare(modifiedOffset.DateTime, lastModifiedDate) < 0)
                        continue;

                    retBlobReferences.Add(blob.Name);

                }
                //else if (item.GetType() == typeof(CloudBlobDirectory))
                //{
                //    CloudBlobDirectory directory = (CloudBlobDirectory)item;

                //    Console.WriteLine("Directory: {0}", directory.Uri);
                //}
            }

            return retBlobReferences;
        }
    }
}