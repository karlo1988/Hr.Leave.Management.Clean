
namespace HR.Leave.Management.Application.Exceptions;

[Serializable]
public class NotFoundException : Exception
{


    public NotFoundException()
    {
    }

    public NotFoundException(string? message) : base(message)
    {
    }

    public NotFoundException(string name, object key) : base($"{name} ({key}) was not found.")
    {
      
    }

    public NotFoundException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}