using Application.Exceptions;
using Application.Services;
using Domain.Entities.Process;
using FFMpegCore;
using MediatR;
using Microsoft.Extensions.Logging;
using System.IO.Compression;

namespace Application.UseCases.GetFrames;

public class GetFramesHandler : IRequestHandler<GetFramesRequest>
{
    private ILogger<GetFramesHandler> _logger { get; set; }
    private IProcessRepository _processRepository { get; set; }
    private IStorageService _storageService { get; set; }

    public GetFramesHandler(
        ILogger<GetFramesHandler> logger, 
        IProcessRepository videoRepository, 
        IStorageService storageService)
    {
        _logger = logger;
        _processRepository = videoRepository;
        _storageService = storageService;
    }

    public async Task Handle(GetFramesRequest request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("GetFramesHandler called");

            await _processRepository.UpdateStatus(request.Id, ProcessStatus.processing);
            _logger.LogInformation("Process status updated to processing");

            // Create workdir for process
            var workDir = Path.GetFullPath(AppContext.BaseDirectory) + "pickframe-results/" + request.Id;
            _logger.LogInformation($"Creating workdir");
            Directory.CreateDirectory(workDir);

            // Download video
            var process = await _processRepository.GetById(request.Id);
            if (process is null)
                throw new ProcessNotFoundException(request.Id);

            var localPath = $"{workDir}/Input/{process.Id}.mp4";
            _logger.LogInformation("Downloading video at {localPath}", localPath);
            await _storageService.DownloadFileAsync(process.InputMediaPath, localPath);

            if (File.Exists(localPath))
            {
                _logger.LogInformation("File downloaded successfully");
            }
            else
            {
                _logger.LogError("Error downloading {InputMediaPath} into {LocalPath}", process.InputMediaPath, localPath);
                await _processRepository.UpdateStatus(request.Id, ProcessStatus.failure);
                return;
            }

            // Create output folder
            var outputFolder = $"{workDir}/Output";
            _logger.LogInformation("Creating output folder {outputFolder}", outputFolder);
            Directory.CreateDirectory(outputFolder);

            var videoInfo = FFProbe.Analyse(localPath);
            var duration = videoInfo.Duration;
            var interval = TimeSpan.FromSeconds(20);

            for (var currentTime = TimeSpan.Zero; currentTime < duration; currentTime += interval)
            {
                _logger.LogInformation("Processando frame: {CurrentFrame}", currentTime);
                var outputPath = Path.Combine(outputFolder, $"frame_at_{currentTime.TotalSeconds}.jpg");
                FFMpeg.Snapshot(localPath, outputPath, new System.Drawing.Size(1920, 1080), currentTime);
            }


            _logger.LogInformation("Gerando arquivo ZIP");
            string zipName = $"{request.Id}-result.zip";
            string destinationZipFilePath = $"{workDir}/{zipName}";
            ZipFile.CreateFromDirectory(outputFolder, destinationZipFilePath);
            _logger.LogInformation("Arquivo ZIP gerado em: {destinationZipFilePath}", destinationZipFilePath);

            _logger.LogInformation("Enviando ZIP para o bucket");

            MemoryStream memStream = new MemoryStream();
            using (FileStream fileStream = File.OpenRead(destinationZipFilePath))
            {
                memStream.SetLength(fileStream.Length);
                fileStream.Read(memStream.GetBuffer(), 0, (int)fileStream.Length);
            }

            string outputMediaPath = $"results/{zipName}";
            if (!await _storageService.UploadFileAsync(outputMediaPath, memStream))
            {
                _logger.LogInformation("Fail to upload {VideoPath}", outputMediaPath);
                await _processRepository.UpdateStatus(request.Id, ProcessStatus.failure);
            }
            else
            {
                _logger.LogInformation("Frames extracted successfully");
                await _processRepository.UpdateStatus(request.Id, ProcessStatus.processed);
                await _processRepository.InsertOutputPath(request.Id, outputMediaPath);
                _logger.LogInformation("Process status updated to processed");
            }

            if (Directory.Exists(workDir))
            {
                _logger.LogInformation("Deleting workdir");
                Directory.Delete(workDir, true);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("Erro: {Message}", ex.Message);
            _logger.LogInformation("Updating status to failure");
            await _processRepository.UpdateStatus(request.Id, ProcessStatus.failure);
            _logger.LogInformation("Process status updated to failure");
        }
    }
}