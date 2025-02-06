using Application.Services;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.UseCases.UploadVideo;

public class UploadVideoHandler : IRequestHandler<UploadVideoRequest, UploadVideoResponse>
{
    private readonly ILogger<UploadVideoHandler> _logger;
    private readonly IStorageService _storageService;

    public UploadVideoHandler(ILogger<UploadVideoHandler> logger, IStorageService storageService)
    {
        _logger = logger;
        _storageService = storageService;
    }

    public async Task<UploadVideoResponse> Handle(UploadVideoRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("UploadVideoHandler called");

        UploadVideoResponse response = new();

        try
        {
            var newGuid = Guid.NewGuid().ToString();
            response.Id = newGuid;

            var path = "media/" + newGuid;
            var result = await _storageService.UploadFileAsync(path, request.VideoStream);
            response.Path = path;

            if (result)
            {
                _logger.LogInformation("File uploaded successfully");
                _logger.LogInformation("Saving video to database");
                response.Success = true;
            }
            else
            {
                _logger.LogError("Error uploading file");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("Error uploading video: {Message}", ex.Message);
        }

        return response;
    }
}