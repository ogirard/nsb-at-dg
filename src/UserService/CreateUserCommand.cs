using NServiceBus;

namespace UserService;

public class CreateUserCommand : ICommand
{
    public string Name { get; init; }
}