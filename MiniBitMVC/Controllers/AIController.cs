using Microsoft.AspNetCore.Mvc;
using Service;

namespace MiniBitMVC.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AIController : ControllerBase
    {
        private readonly IAIService _aiService;

        public AIController(IAIService aiService)
        {
            _aiService = aiService;
        }

        [HttpPost("advice/{userId}")]
        public async Task<IActionResult> GetAdvice(int userId, [FromBody] string prompt)
        {
            var advice = await _aiService.GetFinancialAdviceAsync(userId, prompt);
            return Ok(new { advice });
        }
    }
}
