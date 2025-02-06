using MediatR;

namespace Application.UseCases.GetFrames;

public class GetFramesRequest : IRequest
{
    public string Id { get; set; } = string.Empty;
}