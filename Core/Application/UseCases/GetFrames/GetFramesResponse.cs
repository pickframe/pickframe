using MediatR;

namespace Application.UseCases.GetFrames;

public class GetFramesResponse : IRequest
{
    public string OutputMediaPath { get; set; } = string.Empty;
    public bool Sucess { get; set; }
}