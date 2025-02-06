using MediatR;

namespace Application.UseCases.CreateProcess;

public class CreateProcessResponse : IRequest
{
    public string Id { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public bool Success { get; set; }
}