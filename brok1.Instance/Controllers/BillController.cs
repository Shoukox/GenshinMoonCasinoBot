using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace brok1.Instance.Controllers
{
    [ApiController]
    [Route("/bill/")]
    public class BillController : ControllerBase
    {
        [HttpPost]
        [Route("/bill/events")]
        public async Task<IActionResult> Events()
        {
            Console.WriteLine("bill/events");
            return await Task.FromResult(Ok());
        }

        [HttpPost]
        [Route("/bill/success")]
        public async Task<IActionResult> Success()
        {
            Console.WriteLine("bill/success");
            return await Task.FromResult(Ok());
        }

        [HttpPost]
        [Route("/bill/failed")]
        public async Task<IActionResult> Failed()
        {
            Console.WriteLine("bill/failed");
            return await Task.FromResult(Ok());
        }

        [HttpGet, HttpPost]
        [Route("/bill/test")]
        public async Task<string> Test()
        {
            string text = $"{Thread.CurrentThread.ManagedThreadId}";
            await Task.Delay(1000);
            return text + $" {Thread.CurrentThread.ManagedThreadId}";
        }
    }
}
