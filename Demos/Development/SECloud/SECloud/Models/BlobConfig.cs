using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SECloud.Models
{
    public class BlobConfig
    {
        public string? ContainerName { get; set; }
        public string? BlobEndpoint { get; set; }
        public string? ApiKey { get; set; }
    }
}
