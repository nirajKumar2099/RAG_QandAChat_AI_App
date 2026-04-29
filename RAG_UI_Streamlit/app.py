import streamlit as st
import requests

# -------------------------
# Config
# -------------------------
BASE_URL = "https://localhost:44353/api/rag"

st.set_page_config(
    page_title="QueryGenie AI",
    page_icon="🤖",
    layout="wide"
)

# -------------------------
# Styling (Clean Chat UI)
# -------------------------
st.markdown("""
<style>
.user-bubble {
    background-color: #DCF8C6;
    padding: 10px;
    border-radius: 10px;
    margin: 5px 0;
    text-align: right;
}

.bot-bubble {
    background-color: #F1F0F0;
    padding: 10px;
    border-radius: 10px;
    margin: 5px 0;
    text-align: left;
}

.chat-container {
    max-width: 800px;
    margin: auto;
}
</style>
""", unsafe_allow_html=True)

# -------------------------
# Sidebar Navigation
# -------------------------
st.sidebar.title("🚀 QueryGenie AI")
menu = st.sidebar.radio("Navigation", ["💬 Chat", "📄 Upload Document"])

# -------------------------
# Session State
# -------------------------
if "chat_history" not in st.session_state:
    st.session_state.chat_history = []

# -----------------------------
# 📄 Upload Section
# -----------------------------
if menu == "📄 Upload Document":
    st.title("📄 Upload Document")

    file = st.file_uploader("Upload PDF/Image", type=["pdf", "png", "jpg", "jpeg"])

    if file:
        if st.button("Upload"):
            with st.spinner("Processing..."):
                files = {"file": (file.name, file, file.type)}

                try:
                    res = requests.post(
                        f"{BASE_URL}/upload-doc",
                        files=files,
                        verify=False
                    )

                    if res.status_code == 200:
                        st.success("✅ Uploaded successfully")
                    else:
                        st.error(res.text)

                except Exception as e:
                    st.error(f"Connection error: {e}")

# -----------------------------
# 💬 Chat Section
# -----------------------------
elif menu == "💬 Chat":
    st.title("💬 Q&A Chat")

    # Chat container
    chat_container = st.container()

    with chat_container:
        for chat in st.session_state.chat_history:
            st.markdown(f"<div class='user-bubble'>🧑 {chat['q']}</div>", unsafe_allow_html=True)
            st.markdown(f"<div class='bot-bubble'>🤖 {chat['a']}</div>", unsafe_allow_html=True)

    st.markdown("---")

    col1, col2 = st.columns([8, 1])

    with col1:
        question = st.text_input(
            "Ask your question...",
            placeholder="Type your question here...",
            label_visibility="collapsed"
        )

    with col2:
        send = st.button("➤")

    # Send logic
    if send and question:
        with st.spinner("Thinking... 🤖"):
            try:
                res = requests.post(
                    f"{BASE_URL}/ask-question",
                    json={"question": question},
                    verify=False
                )

                if res.status_code == 200:
                    answer = res.json().get("answer")

                    st.session_state.chat_history.append({
                        "q": question,
                        "a": answer
                    })

                    st.rerun()

                else:
                    st.error(res.text)

            except Exception as e:
                st.error(f"Connection error: {e}")

    # Clear chat button
    if st.button("🗑️ Clear Chat"):
        st.session_state.chat_history = []
        st.rerun()