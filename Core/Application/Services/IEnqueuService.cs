namespace Application.Services;

public interface IEnqueuService
{
    public Task EnqueueProcess(string processId);
}