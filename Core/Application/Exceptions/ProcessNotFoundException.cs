using System.Runtime.Serialization;

namespace Application.Exceptions;

[Serializable]
public class ProcessNotFoundException : Exception
{
    public ProcessNotFoundException() : base() { }
    public ProcessNotFoundException(string message) : base(message) { }
    public ProcessNotFoundException(string message, Exception inner) : base(message, inner) { }

    [Obsolete("Resolve sonarqube code smell", DiagnosticId = "SYSLIB0051")]
    protected ProcessNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context) { }
}
