namespace RAG_API_dotnet.Application.Services.Interface
{
    public interface IRagService
    {
        Task<string> UploadDocument(IFormFile file);
        Task<string> AskQuestion(string question);
        Task<List<string>> SearchContextAsync(string Queryforembedding);
    }
}
