using brok1.Instance.Services;
using Microsoft.AspNetCore.Mvc;
using Telegram.Bot.Types;

namespace brok1.Instance.Controllers
{
    [ApiController]
    [Route("/")]
    public class WebhookController : ControllerBase
    {
        [HttpPost]
        public Task PostAsync(
              [FromBody] Update update,
              [FromServices] UpdateHandler handleUpdateService,
              CancellationToken cancellationToken)
        {
            //_ = Task.Run(() => handleUpdateService.HandleUpdateAsync(update, cancellationToken));
            return Task.CompletedTask;
        }
    }
}