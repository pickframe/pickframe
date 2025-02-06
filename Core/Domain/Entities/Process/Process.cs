using Core.Domain.Notification;

namespace Domain.Entities.Process;

public class Process : IEntity
{
    public Process(string id, ProcessStatus status, string inputMediaPath, string outputMediaPath = "")
    {
        Id = id;
        Status = status;
        InputMediaPath = inputMediaPath;
        OutputMediaPath = outputMediaPath;
    }
    
    public string Id { get; set; }
    public ProcessStatus Status { get; set; }
    public string InputMediaPath { get; set; }
    public string? OutputMediaPath { get; set; }

    public ErrorList Validate()
    {
        var errors = new ErrorList();

        if (string.IsNullOrWhiteSpace(InputMediaPath))
        {
            errors.AddError("Input media path is required.");
        }

        return errors;
    }

    public void UpdateStatus(ProcessStatus processStatus)
    {
        Status = processStatus;
    }
}