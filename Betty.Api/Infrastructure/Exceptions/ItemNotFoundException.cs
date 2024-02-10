namespace Betty.Api.Infrastructure.Exceptions;

public class ItemNotFoundException : Exception
{
    public ItemNotFoundException(string? message) : base(message)
    {
    }
}
