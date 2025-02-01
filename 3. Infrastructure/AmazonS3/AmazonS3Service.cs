using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Transfer;
using Application.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AmazonS3;

public class AmazonS3Service : IStorageService
{
    private AmazonS3Client _client { get; set; }
    private ILogger<AmazonS3Service> _logger { get; set; }
    private string _bucketName { get; set; }

    public AmazonS3Service(IConfiguration configuration, ILogger<AmazonS3Service> logger)
    {
        _logger = logger;

        var acessKey = configuration["AWS:AccessKey"]!;
        var secretKey = configuration["AWS:SecretKey"]!;
        var token = configuration["AWS:Token"]!;
        var region = configuration["AWS:Region"]!;
        var bucketName = configuration["AWS:BucketName"]!;

        var awsCredentials = new SessionAWSCredentials(acessKey, secretKey, token);

        var config = new AmazonS3Config()
        {
            RegionEndpoint = RegionEndpoint.USEast1
        };

        _client = new AmazonS3Client(awsCredentials, config);
        _bucketName = bucketName;
    }

    public async Task<bool> UploadFileAsync(string path, MemoryStream memoryStream)
    {
        try
        {
            var tu = new TransferUtility(_client);

            await tu.UploadAsync(new TransferUtilityUploadRequest
            {
                BucketName = _bucketName,
                Key = path,
                InputStream = memoryStream
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }

        return true;
    }

    public async Task DownloadFileAsync(string path, string localDirectory)
    {
        try
        {
            var tu = new TransferUtility(_client);

            await tu.DownloadAsync(localDirectory, _bucketName, path);

            // await tu.DownloadDirectoryAsync(new TransferUtilityDownloadDirectoryRequest(){
            //     BucketName = _bucketName,
            //     S3Directory = path,
            //     LocalDirectory = localDirectory
            // });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
        }
    }
}
