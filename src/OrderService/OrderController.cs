using Microsoft.AspNetCore.Mvc;
using NServiceBus;
using UserService;

namespace OrderService
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class OrderController : ControllerBase
    {
        private readonly ILogger<OrderController> logger;
        private readonly IMessageSession messageSession;

        public OrderController(ILogger<OrderController> logger, IMessageSession messageSession)
        {
            this.logger = logger;
            this.messageSession = messageSession;
        }

        [HttpPost(Name = "CreateBuyer")]
        public async Task CreateBuyer(string name)
        {
            await messageSession.Send(new CreateUserCommand { Name = name });
        }
    }
}