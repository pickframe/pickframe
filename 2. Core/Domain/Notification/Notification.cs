namespace Core.Domain.Notification;

public class ErrorList
{
    private readonly List<Erro> Errors = [];

    public bool HasErrors => Errors.Any();
    
    public void AddError(string message, Exception? exception = null)
    {
        Errors.Add(new Erro(message, exception));
    }

    public string GetErrors()
    {
        return string.Join("\n", Errors.Select(e => e.Message));
    }
}