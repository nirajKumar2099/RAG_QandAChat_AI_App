using Azure.Core;
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

        [HttpPost("ask-question")]
        public async Task<IActionResult> Ask([FromBody] QuestionRequest request)
        {
            var result = await _ragService.AskQuestion(request.Question);
            return Ok(result);
        }

        [HttpPost("upload-doc")]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            var result = await _ragService.UploadDocument(file);
            return Ok(result);
        }

        [HttpPost("search-context")]
        public async Task<IActionResult> SearchContext([FromBody] SearchContextRequest request)
        {
            var result = await _ragService.SearchContextAsync(request.Queryforembedding);
            return Ok(result);
        }
    }
}
