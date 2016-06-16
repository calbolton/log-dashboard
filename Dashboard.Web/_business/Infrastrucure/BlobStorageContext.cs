using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Dashboard.Web._business.Infrastrucure
{
    public class BlobStorageContext
    {
        public string ConnectionString { get; set; }

        public string Container { get; set; }
    }
}