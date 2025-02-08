using MediatR;

namespace Application.UseCases.GetFrames;

public class GetFramesRequest : IRequest<GetFramesResponse>
{
    public string Id { get; set; } = string.Empty;
}