using NServiceBus;

namespace UserService;


public class User
{
    public string Name { get; init; }
}

public class UserEvent : IEvent
{
    public DateTime Timestamp { get; init; }

    public User User { get; init; }
}