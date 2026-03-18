import axios from "axios";

const API_BASE = import.meta.env.VITE_API_BASE || "http://localhost:5000";

export async function sendMessage(messages) {
  const { data } = await axios.post(`${API_BASE}/api/chat`, { messages });
  return data.reply;
}