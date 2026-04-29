import httpx
from fastapi import FastAPI, UploadFile, File
from pydantic import BaseModel
from services.extractor import extract_text_from_pdf, extract_text_from_image
from services.chunker import chunk_text
from services.embedder import generate_embedding


app = FastAPI()

class TextRequest(BaseModel):
    text: str

class AskRequest(BaseModel):
    question: str
    context: list[str]    
    
    #  API STARTS HERE

@app.post("/ask")
async def ask_question(request: AskRequest):

    # Handle empty context safely
    context_text = "\n".join(request.context) if request.context else ""

    prompt = f"""
You are an AI assistant.

Answer ONLY from the given context.
Return ONLY the relevant part of the answer.

If the question is about a specific topic (like office hours),
DO NOT include other topics.

If answer is not found in context, say: "Not found in document".

Keep answer short and precise.

Context:
{context_text}

Question:
{request.question}

Answer:
"""

    try:
        async with httpx.AsyncClient(timeout=120.0) as client:
            response = await client.post(
                "http://localhost:11434/api/generate",
                json={
                    "model": "tinyllama",
                    "prompt": prompt,
                    "stream": False
                }
            )

        # Proper error handling
        if response.status_code != 200:
            return {
                "error": "Ollama API failed",
                "status_code": response.status_code,
                "details": response.text
            }

        result = response.json()

        # Safe parsing
        answer = result.get("response")

        if not answer:
            return {
                "error": "Empty response from LLM",
                "raw": result
            }

        return {
            "answer": answer.strip()
        }

    except Exception as e:
        # Catch unexpected errors
        return {
            "error": "Internal server error",
            "details": str(e)
        }

    


@app.post("/process-document")
async def process_document(file: UploadFile = File(...)):
    
    # Crucial: Start from the beginning of the file
    await file.seek(0)
    
    filename = file.filename.lower()
    
    if filename.endswith(".pdf"):
        text = extract_text_from_pdf(file.file)
    elif filename.endswith((".png", ".jpg", ".jpeg")):
        # Read the bytes here to be safe
        text = extract_text_from_image(file.file)
    else:
        return {"error": "Unsupported file type"}

    if not text:
        # If this happens, check your terminal for the "OCR Error Details"
        return {"error": "Extraction failed. Check server logs."}

     
    # Step 3: Embedding
    result = []
    embedding = generate_embedding(text)
        
    return {
        "chunks": [
            {
                "text": text,
                "embedding": embedding
            }
        ]
    }
    
@app.post("/userquery-embed")
def embed_text(request: TextRequest):
    embedding = generate_embedding(request.text)
    return {
        "embedding": embedding
    }

    
''' uvicorn app:app --reload '''