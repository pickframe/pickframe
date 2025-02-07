using Application.Services;
using FFMpegCore;
using Microsoft.Extensions.Logging;
using System.Diagnostics.CodeAnalysis;

namespace FFMpeg;

[ExcludeFromCodeCoverage]
public class FFMpegProcessor : IVideoProcessorService
{
    public FFMpegProcessor(ILogger<FFMpegProcessor> logger)
    {
        Logger = logger;
    }

    public ILogger<FFMpegProcessor> Logger { get; }

    public bool PickFramesAt(string inputFile, string outputFolder, int intervalFrames)
    {
        try
        {
            var videoInfo = FFProbe.Analyse(inputFile);
            var duration = videoInfo.Duration;
            var interval = TimeSpan.FromSeconds(intervalFrames);

            for (var currentTime = TimeSpan.Zero; currentTime < duration; currentTime += interval)
            {
                Logger.LogInformation("Processando frame: {CurrentFrame}", currentTime);
                var outputPath = Path.Combine(outputFolder, $"frame_at_{currentTime.TotalSeconds}.jpg");
                FFMpegCore.FFMpeg.Snapshot(inputFile, outputPath, new System.Drawing.Size(1920, 1080), currentTime);
            }

            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
}
