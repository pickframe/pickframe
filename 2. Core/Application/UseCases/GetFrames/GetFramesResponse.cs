using MediatR;

namespace Application.UseCases.GetFrames
{
    public class GetFramesResponse : IRequest
    {
        public string? Id { get; set; }
        public string? Status { get; set; }
        public string? OutputPath { get; set; }
    }
}