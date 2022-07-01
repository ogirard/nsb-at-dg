using NServiceBus;
using UserService;

namespace OrderService;

public class UserEventHandler : IHandleMessages<UserEvent>
{
    private readonly ILogger<UserEventHandler> logger;

    public UserEventHandler(ILogger<UserEventHandler> logger)
    {
        this.logger = logger;
    }
    public Task Handle(UserEvent message, IMessageHandlerContext context)
    {
        logger.LogInformation("Handling UserEvent, UserName: {userName}", message.User.Name);
        return Task.CompletedTask;
    }
}