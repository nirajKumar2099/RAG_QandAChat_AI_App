# RAG_QandAChat_AI_App
🚀 Built a Full-Stack RAG-based AI Application

I recently developed an end-to-end Retrieval-Augmented Generation (RAG) system that enables intelligent question-answering over documents and images.

💡 Use Cases

Designed for real-world enterprise scenarios where knowledge is scattered:

• 📘 HR policy assistant (leave policy, office timings, benefits)
• 📄 Enterprise document Q&A (SOPs, manuals, guidelines)
• 🛠️ IT/helpdesk knowledge assistant
• 🏥 Healthcare documentation insights
• 📊 Internal knowledge base search
• 🤖 AI-powered chatbot for business workflows

🧠 How It Works

• Documents/images → text extraction → chunking
• Chunking → vector embeddings → stored in pgvector
• User query → semantic search → retrieves relevant context
• Context + query → LLM → accurate, contextual response

🏗️ Tech Stack

• Backend: .NET Web API
• Frontend: Streamlit (Chat UI)
• AI Services: Python + FastAPI
• Database: PostgreSQL (pgvector)

📚 Key Libraries & Tools

• LangChain → orchestration of RAG pipeline
• sentence-transformers → embeddings
• Ollama (LLM) → answer generation
• pypdf, pytesseract, PIL → document & image processing
• httpx, .NET HttpClient → service communication


## 🏗️ Architecture Diagram

```mermaid
flowchart LR

    User[👤 User] --> UI[💻 Streamlit Frontend]

    UI --> API[⚙️ .NET Web API]

    API --> AI[🧠 FastAPI AI Service]

    AI --> EMB[🔎 Embedding Model\n(sentence-transformers)]
    AI --> LLM[🤖 LLM (Ollama)]

    EMB --> DB[(🗄️ PostgreSQL + pgvector)]
    DB --> AI

    LLM --> AI
    AI --> API
    API --> UI



---

# 🎯 **What This Shows (you can optionally explain below it)**

You can add a short description:

```markdown
### 🔍 Flow Explanation

1. User interacts with Streamlit UI  
2. Request goes to .NET Backend API  
3. Backend calls FastAPI AI Service  
4. AI Service:
   - Generates embeddings  
   - Retrieves relevant data from pgvector  
   - Sends context to LLM (Ollama)  
5. LLM generates response → returned to user
____________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________
⚙️ Prerequisites

Make sure you have installed:

.NET 8 SDK
Python 3.10+
PostgreSQL (with pgvector enabled)
Ollama
🚀 Step 1: Start Ollama (LLM)
ollama run phi3

👉 First time only:

ollama pull phi3

👉 Runs on: http://localhost:11434

🧠 Step 2: Start AI Service (FastAPI)
cd ai-service

Activate virtual environment:

venv\Scripts\activate   # Windows
# source venv/bin/activate   # Mac/Linux

Install dependencies (if not already):

pip install -r requirements.txt

Run FastAPI:

uvicorn main:app --reload --host 0.0.0.0 --port 8000

👉 Runs on: http://localhost:8000

⚙️ Step 3: Start .NET Backend API

Open a new terminal:

cd backend

Restore & run:

dotnet restore
dotnet run

👉 Runs on:
http://localhost:5000
 (or 5001)

💻 Step 4: Start Streamlit Frontend

Open another terminal:

cd frontend

Install dependencies (if needed):

pip install -r requirements.txt

Run UI:

streamlit run app.py

👉 Runs on: http://localhost:8501

🔄 Execution Order (Important)

Start services in this order:

✅ Ollama
✅ FastAPI (AI Service)
✅ .NET Backend API
✅ Streamlit UI
🔗 Service URLs


