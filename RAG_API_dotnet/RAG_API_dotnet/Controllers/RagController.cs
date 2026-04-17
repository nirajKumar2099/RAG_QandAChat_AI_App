using Microsoft.AspNetCore.Mvc;
using RAG_API_dotnet.Application.Services.Interface;
using RAG_API_dotnet.Models;

namespace RAG_API_dotnet.Controllers
{
    [ApiController]
    [Route("api/rag")]
    public class RagController : ControllerBase
    {
        private readonly IRagService _ragService;
        public RagController(IRagService ragService) // constructor Injection
        {
            _ragService = ragService;
        }

        [HttpPost("askquestion")]
        public async Task<IActionResult> Ask([FromBody] QuestionRequest request)
        {
            var result = await _ragService.AskQuestion(request.Question);
            return Ok(result);
        }

        [HttpPost("uploaddoc")]
        public async Task<IActionResult> Upload([FromBody] UploadRequest request)
        {
            var result = await _ragService.UploadDocument(request.Content);
            return Ok(result);
        }
    }
}
