using Application.UseCases.CreateProcess;

namespace Tests.Builders;

public class CreateProcessRequestBuilder
{
    public static CreateProcessRequest Instancia()
    {
        return new CreateProcessRequest { VideoStream = VideoTestBuilder.Instancia() };
    }
}
