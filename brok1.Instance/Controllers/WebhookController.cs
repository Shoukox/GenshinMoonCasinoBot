using brok1.Instance.Services;
using brok1.Instance.Types;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Threading;
using System.Runtime.CompilerServices;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace brok1.Instance.Controllers
{
    [ApiController]
    [Route("/")]
    public class WebhookController : ControllerBase
    {
        [HttpPost]
        public async Task PostAsync(
              [FromBody] Update update,
              [FromServices] UpdateHandler handleUpdateService,
              CancellationToken cancellationToken)
        {
            await handleUpdateService.HandleUpdateAsync(update, cancellationToken);
        }
    }
}