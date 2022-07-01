namespace OrderService;

[AttributeUsage(AttributeTargets.Class)]
public sealed class DestinationAttribute : Attribute
{
    public string EndpointName { get; }

    public bool IsInternal { get; }

    public DestinationAttribute(string endpointName, bool isInternal = false)
    {
        EndpointName = endpointName;
        IsInternal = isInternal;
    }
}