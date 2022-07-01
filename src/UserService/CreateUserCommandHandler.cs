using NServiceBus;

namespace UserService;

public class CreateUserCommandHandler : IHandleMessages<CreateUserCommand>
{
    private readonly ILogger<CreateUserCommandHandler> logger;

    public CreateUserCommandHandler(ILogger<CreateUserCommandHandler> logger)
    {
        this.logger = logger;
    }

    public Task Handle(CreateUserCommand message, IMessageHandlerContext context)
    {
        logger.LogInformation("CreateUserCommand, user name: {userName}", message.Name);
        return Task.CompletedTask;
    }
}