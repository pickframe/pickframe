using MediatR;

namespace Application.UseCases.UploadVideo;

public record UploadVideoRequest : IRequest<UploadVideoResponse>
{
    public required MemoryStream VideoStream { get; set; }
}