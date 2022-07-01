using NServiceBus;
using OrderService;

namespace UserService;

[Destination("UserService")]
public class CreateUserCommand : ICommand
{
    public string Name { get; init; }
}