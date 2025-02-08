using MediatR;

namespace Application.UseCases.CreateProcess;

public class CreateProcessRequest : IRequest<CreateProcessResponse>
{
    public required MemoryStream VideoStream { get; set; }
}