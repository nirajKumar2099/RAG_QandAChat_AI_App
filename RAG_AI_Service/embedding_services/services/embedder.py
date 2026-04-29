from sentence_transformers import SentenceTransformer

print("--- EMBEDDER MODULE LOADED ---") # just to check this page is being executed without errors

model = SentenceTransformer('all-MiniLM-L6-v2')

def generate_embedding(text):
    return model.encode(text).tolist() #