using Application.Services;
using Domain.Entities.Process;
using FFMpegCore;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.UseCases.GetFrames;

public class GetFramesHandler : IRequestHandler<GetFramesRequest, GetFramesResponse>
{
    private readonly ILogger<GetFramesHandler> _logger;
    private readonly IProcessRepository _processRepository;
    private readonly IStorageService _storageService;

    public GetFramesHandler(ILogger<GetFramesHandler> logger, IProcessRepository videoRepository, IStorageService storageService)
    {
        _logger = logger;
        _processRepository = videoRepository;
        _storageService = storageService;
    }

    public async Task<GetFramesResponse> Handle(GetFramesRequest request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("GetFramesHandler called");
            var response = new GetFramesResponse() { Id = request.Id };

            await _processRepository.UpdateStatus(request.Id, ProcessStatus.processing);
            _logger.LogInformation("Process status updated to processing");

            try
            {
                // Create workdir for process
                var workDir = Path.GetFullPath(AppContext.BaseDirectory) + "pickframe-results/" + request.Id;
                _logger.LogInformation($"Creating workdir: {workDir}");
                Directory.CreateDirectory(workDir);

                // Download video
                var video = await _processRepository.GetById(request.Id);
                var localPath = $"{workDir}/Input/{video.Id}.mp4";
                _logger.LogInformation($"Downloading video to: {localPath}");
                await _storageService.DownloadFileAsync(video.InputMediaPath, localPath);

                if (File.Exists(localPath))
                {
                    _logger.LogInformation("File downloaded successfully");
                }
                else
                {
                    _logger.LogError("Error downloading file");
                    throw new Exception("Error downloading file");
                }

                // Create output folder
                var outputFolder = $"{workDir}/Output";
                _logger.LogInformation($"Creating output folder: {outputFolder}");
                Directory.CreateDirectory(outputFolder);

                var videoInfo = FFProbe.Analyse(localPath);
                var duration = videoInfo.Duration;
                var interval = TimeSpan.FromSeconds(20);

                _logger.LogInformation($"Duration: {duration}");
                _logger.LogInformation($"Interval: {interval}");
                _logger.LogInformation($"Extracting frames...");

                for (var currentTime = TimeSpan.Zero; currentTime < duration; currentTime += interval)
                {
                    _logger.LogInformation($"Processando frame: {currentTime}");
                    var outputPath = Path.Combine(outputFolder, $"frame_at_{currentTime.TotalSeconds}.jpg");
                    FFMpeg.Snapshot(localPath, outputPath, new System.Drawing.Size(1920, 1080), currentTime);
                }

                _logger.LogInformation("Frames extracted successfully");
                await _processRepository.UpdateStatus(request.Id, ProcessStatus.processed);
                _logger.LogInformation("Process status updated to processed");

                if (Directory.Exists(workDir))
                {
                    _logger.LogInformation("Deleting workdir");
                    Directory.Delete(workDir, true);
                }
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogInformation("Updating status to failure");
                await _processRepository.UpdateStatus(request.Id, ProcessStatus.failure);
                _logger.LogInformation("Process status updated to failure");
            }

            var process = await _processRepository.GetById(request.Id);
            response.Id = process.Id;
            response.Status = process.Status.ToString();
            response.OutputPath = process.OutputMediaPath;

            return response;        
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex.Message);
            throw;
        }
    }
}