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

    public static ProcessStatus ToProcessStatusEnum(this string status)
    {
        return status switch
        {
            nameof(ProcessStatus.queued) => ProcessStatus.queued,
            nameof(ProcessStatus.processed) => ProcessStatus.processed,
            nameof(ProcessStatus.processing) => ProcessStatus.processing,
            nameof(ProcessStatus.failure) => ProcessStatus.failure,
            _ => throw new ArgumentOutOfRangeException(nameof(status)),
        };
    }
}