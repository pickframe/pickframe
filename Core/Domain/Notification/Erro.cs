namespace Core.Domain.Notification;

public class Erro
{
    public Erro(string message, Exception? cause)
    {
        Message = message;
        Cause = cause;
    }

    public string Message { get; set; }
    public Exception? Cause { get; set; }
}