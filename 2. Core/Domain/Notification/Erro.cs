namespace Core.Domain.Notification;

public class Erro(string message, Exception? cause)
{
    public string Message { get; set; } = message;
    public Exception? Cause { get; set; } = cause;
}