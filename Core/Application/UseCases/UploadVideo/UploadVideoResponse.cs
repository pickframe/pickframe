using MediatR;

namespace Application.UseCases.UploadVideo;

public class UploadVideoResponse : IRequest
{
    public string Id { get; set; } = string.Empty;
    public bool Success { get; set; }
    public string Path { get; set; } = string.Empty;
}