using Dapper;
using Npgsql;
using RAG_API_dotnet.Application.Services.Interface;
using RAG_API_dotnet.Models;

namespace RAG_API_dotnet.Application.Services.Service
{
    public class RagService : IRagService
    {
        private readonly IConfiguration _configuration;

        public RagService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<string> UploadDocument(IFormFile file)
        {
            using var http = new HttpClient();

            // Create multipart content
            using var content = new MultipartFormDataContent();

            using var stream = file.OpenReadStream();
            content.Add(new StreamContent(stream), "file", file.FileName);

            // Call Python API
            var response = await http.PostAsync(
                "http://localhost:8000/process-document",
                content
            );

            if (!response.IsSuccessStatusCode)
                throw new Exception("Python API failed");

            var result = await response.Content.ReadFromJsonAsync<ProcessResponse>();

            // Save to DB
            await SaveChunksToDb(result.chunks);

            return "Document processed successfully";
        }

        // save data to DB 
        private async Task SaveChunksToDb(List<ChunkItem> chunks)
        {
            using var conn = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            foreach (var chunk in chunks)
            {
                // 🔹 Step 1: Check duplicate
                var existsQuery = @"
            SELECT COUNT(1)
            FROM documents
            WHERE content = @content;
        ";

                var exists = await conn.ExecuteScalarAsync<int>(existsQuery, new
                {
                    content = chunk.text
                });

                if (exists > 0)
                {
                    // 👉 Skip duplicate
                    continue;
                }

                // 🔹 Step 2: Insert if not exists
                var vectorString = "[" + string.Join(",", chunk.embedding) + "]";

                var insertSql = @"
            INSERT INTO documents (content, embedding)
            VALUES (@content, CAST(@embedding AS vector));
        ";

                await conn.ExecuteAsync(insertSql, new
                {
                    content = chunk.text,
                    embedding = vectorString
                });
            }
        }
        public async Task<string> AskQuestion(string question)
        {
            // Step 1: Get context from DB
            var contextList = await SearchContextAsync(question);

            using var http = new HttpClient();

            // Step 2: Call Python LLM API
            var response = await http.PostAsJsonAsync(
                "http://localhost:8000/ask",
                new
                {
                    question = question,
                    context = contextList
                }
            );

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"LLM API failed: {response.StatusCode} - {errorContent}");
            }

            var result = await response.Content.ReadFromJsonAsync<AskResponse>();

            if (result == null || string.IsNullOrWhiteSpace(result.answer))
            {
                var raw = await response.Content.ReadAsStringAsync();
                throw new Exception($"Invalid response from LLM API: {raw}");
            }

            return result.answer;

        }


        public async Task<List<string>> SearchContextAsync(string query)
        {
            //Step 1: Get real embedding
            var embedding = await GetEmbeddingFromPython(query);

            var vectorString = "[" + string.Join(",", embedding) + "]";

            // Extract keyword
            var keyword = query.Split(' ', StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();


            var sql = @"
                        SELECT content
                        FROM documents
                        ORDER BY embedding <-> CAST(@embedding AS vector)
                        LIMIT 3;
                    ";

            using var conn = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            var result = await conn.QueryAsync<string>(sql, new { embedding = vectorString });

            return result.ToList();
        }

        private async Task<float[]> GetEmbeddingFromPython(string text)
        {
            using var http = new HttpClient();

            var response = await http.PostAsJsonAsync(
                "http://localhost:8000/userquery-embed",
                new { text = text }
            );

            if (!response.IsSuccessStatusCode)
                throw new Exception("Embedding API failed");

            var result = await response.Content.ReadFromJsonAsync<EmbeddingResponse>();

            return result.embedding.Select(x => (float)x).ToArray();
        }


        // RAG Pipeline flow :The /ask-question API orchestrates the flow. It generates the query embedding,
        // calls the /search-context API to retrieve relevant documents using pgvector similarity search,
        // and then sends the retrieved context along with the user query to the LLM to generate the final response.

    }
}
