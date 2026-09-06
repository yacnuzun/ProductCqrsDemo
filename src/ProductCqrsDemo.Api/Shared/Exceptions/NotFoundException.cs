public sealed class NotFoundException : Exception
{
    public NotFoundException(string entityName, object key)
        : base($"{entityName} bulunamadı. (Id: {key})") { }
}