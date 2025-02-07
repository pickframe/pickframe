namespace Domain.Entities.Process;

public enum ProcessStatus
{   
    queued,
    processing,
    processed,
    failure,
}

public static class ProcessStatusExtension
{
    public static string ToText(this ProcessStatus status)
    {
        switch (status)
        {
            case ProcessStatus.queued: return "queued";
            case ProcessStatus.processing: return "processing";
            case ProcessStatus.processed: return "processed";
            case ProcessStatus.failure: return "failure";
            default: throw new ArgumentOutOfRangeException(nameof(status));
        }
    }
}