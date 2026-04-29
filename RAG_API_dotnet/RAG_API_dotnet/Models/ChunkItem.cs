namespace RAG_API_dotnet.Models
{
    public class ChunkItem
    {
        public string text { get; set; }
        public List<double> embedding { get; set; }
    }
}
