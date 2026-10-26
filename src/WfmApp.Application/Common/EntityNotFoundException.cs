namespace WfmApp.Application.Common;

public sealed class EntityNotFoundException : Exception
{
    public EntityNotFoundException(string message)
        : base(message)
    {
    }
}
