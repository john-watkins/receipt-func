using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Transfer;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AwsApi
{
    public interface IAwsS3
    {
        Task UploadFileToS3(byte[] file, string name, DateTime created);
    }
    public class AwsS3 : IAwsS3
    {
        private readonly IConfiguration _config;
        private AwsApiConfig _awsApiConfig = new AwsApiConfig();
        public AwsS3(IConfiguration config)
        {
            _config = config;
            _config.Bind("receipt-func-secrets:aws", _awsApiConfig);
        }

        public async Task UploadFileToS3(byte[] file, string name, DateTime created)
        {

            var credentials = new BasicAWSCredentials(_awsApiConfig.accessKey, _awsApiConfig.secretKey);
            var config = new AmazonS3Config
            {
                RegionEndpoint = RegionEndpoint.GetBySystemName(_awsApiConfig.region)
            };
            using var client = new AmazonS3Client(credentials, config);
            await using var newMemoryStream = new MemoryStream(file);

            var uploadRequest = new TransferUtilityUploadRequest
            {
                InputStream = newMemoryStream,
                Key = name,
                BucketName = _awsApiConfig.bucketName,
                CannedACL = S3CannedACL.NoACL
            };
            uploadRequest.Metadata.Add("created", created.ToString("yyyy-MM-dd"));
            var fileTransferUtility = new TransferUtility(client);
            await fileTransferUtility.UploadAsync(uploadRequest);
        }
    }
}
