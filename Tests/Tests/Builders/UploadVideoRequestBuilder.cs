using Application.UseCases.UploadVideo;

namespace Tests.Builders;

public class UploadVideoRequestBuilder
{
    public static UploadVideoRequest Instancia()
    {
        return new UploadVideoRequest { VideoStream = VideoTestBuilder.Instancia() };
    }
}
