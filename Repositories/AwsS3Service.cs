using Amazon.S3;
using Amazon.S3.Transfer;
using EmployeeManagementSystem.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace EmployeeManagementSystem.Services
{
    public class AwsS3Service : IAwsS3Service
    {
        private readonly IAmazonS3 _s3Client;
        private readonly string _bucketName;

        public AwsS3Service(IConfiguration configuration)
        {
            var awsOptions = configuration.GetSection("AwsS3");
            _bucketName = awsOptions["BucketName"]!;
            var accessKey = awsOptions["AccessKey"];
            var secretKey = awsOptions["SecretKey"];
            var region = Amazon.RegionEndpoint.GetBySystemName(awsOptions["Region"]);

            _s3Client = new AmazonS3Client(accessKey, secretKey, region);
        }

        public async Task<string?> UploadFileAsync(IFormFile file, string fileName)
        {
            try
            {
                using var stream = file.OpenReadStream();
                var uploadRequest = new TransferUtilityUploadRequest
                {
                    InputStream = stream,
                    Key = fileName,
                    BucketName = _bucketName,
                    ContentType = file.ContentType,
                   
                };

                var transferUtility = new TransferUtility(_s3Client);
                await transferUtility.UploadAsync(uploadRequest);

                var fileUrl = $"https://{_bucketName}.s3.amazonaws.com/{fileName}";
                Log.Information("File uploaded to S3: {FileUrl}", fileUrl);
                return fileUrl;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to upload file to S3");
                return null;
            }
        }
    }
}
