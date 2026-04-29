import pytesseract
from PIL import Image
from pypdf import PdfReader
import io
import os

# 1. Define the path
tesseract_path = r'C:\Program Files\Tesseract-OCR\tesseract.exe'

# 2. Debugging: Check if the file actually exists on your disk
if os.path.exists(tesseract_path):
    pytesseract.pytesseract.tesseract_cmd = tesseract_path
else:
    print(f"CRITICAL ERROR: Tesseract not found at {tesseract_path}")

def extract_text_from_pdf(file_stream):
    try:
        reader = PdfReader(file_stream)
        text = ""
        for page in reader.pages:
            content = page.extract_text()
            if content:
                text += content + "\n"
        return text
    except Exception as e:
        print(f"PDF Error: {e}")
        return ""

def extract_text_from_image(file_stream):
    try:
        # Read stream and reset
        image_bytes = file_stream.read()
        image = Image.open(io.BytesIO(image_bytes))
        
        # Perform OCR
        text = pytesseract.image_to_string(image)
        return text
    except Exception as e:
        # This will print the EXACT reason for the 500 error in your terminal
        print(f"OCR Error Details: {e}")
        return ""