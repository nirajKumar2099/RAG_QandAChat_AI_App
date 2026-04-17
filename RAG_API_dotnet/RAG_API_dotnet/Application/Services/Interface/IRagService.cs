namespace RAG_API_dotnet.Application.Services.Interface
{
    public interface IRagService
    {
        Task<string> UploadDocument(string content);
        Task<string> AskQuestion(string question);
    }
}
