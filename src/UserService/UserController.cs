using Microsoft.AspNetCore.Mvc;
using NServiceBus;

namespace UserService
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> logger;
        private readonly IMessageSession messageSession;

        public UserController(ILogger<UserController> logger, IMessageSession messageSession)
        {
            this.logger = logger;
            this.messageSession = messageSession;
        }

        [HttpGet(Name = "GetUsers")]
        public async Task<IEnumerable<User>> Get()
        {
            var users = new List<User>();
            for (var i = 0; i < 10; i++)
            {
                var user = new User { Name = $"User{i}" };
                users.Add(user);
                await messageSession.Publish(new UserEvent { User = user, Timestamp = DateTime.UtcNow });
            }

            return users;
        }
    }
}