using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AwsApi
{
    public class AwsApiConfig
    {
        public string accessKey { get; set; }
        public string secretKey { get; set; }
        public string bucketName { get; set; }
        public string region { get; set; }
    }
}
