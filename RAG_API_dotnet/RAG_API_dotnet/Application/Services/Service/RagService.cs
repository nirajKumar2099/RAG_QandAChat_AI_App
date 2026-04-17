using RAG_API_dotnet.Application.Services.Interface;

namespace RAG_API_dotnet.Application.Services.Service
{
    public class RagService : IRagService
    {
        public async Task<string> AskQuestion(string question)
        {
           
            // Later: RAG pipeline call
            return await Task.FromResult($"Answer for: {question}");
        }

        public async Task<string> UploadDocument(string content)
        {
            // Later: Python AI service call
            return await Task.FromResult("Document processed");
        }
    }
}
