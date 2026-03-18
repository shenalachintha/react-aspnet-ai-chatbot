import React, { useState } from "react";
import ChatWindow from "../components/ChatWindow.jsx";
import InputBox from "../components/InputBox.jsx";
import { sendMessage } from "../services/chatService.js";

export default function ChatPage() {
  const [messages, setMessages] = useState([
    { role: "assistant", content: "Hi! Ask me anything." },
  ]);
  const [isSending, setIsSending] = useState(false);
  const [error, setError] = useState(null);

  const handleSend = async (text) => {
    if (!text.trim()) return;
    setError(null);
    const next = [...messages, { role: "user", content: text }];
    setMessages(next);
    setIsSending(true);
    try {
      const reply = await sendMessage(next);
      setMessages((prev) => [...prev, { role: "assistant", content: reply }]);
    } catch (err) {
      setError(err?.response?.data?.message || err.message || "Failed to send");
    } finally {
      setIsSending(false);
    }
  };

  return (
    <div className="mx-auto flex min-h-screen max-w-5xl flex-col px-4 py-10">
      <header className="mb-8 flex flex-col gap-2 sm:flex-row sm:items-center sm:justify-between">
        <div>
          <p className="text-sm uppercase tracking-[0.25em] text-slate-400">
            Phase 1
          </p>
        </div>
        <h1 className="text-3xl font-semibold text-white">AI Chatbot Console</h1>
        <p className="text-slate-400">
          Chat with your assistant. Messages are not persisted yet.
        </p>
      </header>

      <div className="flex flex-1 flex-col gap-4">
        <div className="flex-1 overflow-hidden rounded-2xl border border-slate-800 bg-gradient-to-b from-slate-900 to-slate-950 shadow-2xl shadow-slate-900/60 chat-window">
          <ChatWindow messages={messages} />
        </div>

        <div className="rounded-2xl border border-slate-800 bg-slate-900/80 p-4 shadow-xl shadow-slate-900/50">
          <InputBox onSend={handleSend} disabled={isSending} />
          <div className="mt-3 flex items-center gap-3 text-sm text-slate-400">
            {isSending && (
              <span className="inline-flex items-center gap-2">
                <span className="h-2 w-2 animate-pulse rounded-full bg-emerald-400" />
                Thinking…
              </span>
            )}
            {error && <span className="text-rose-400">⚠️ {error}</span>}
          </div>
        </div>
      </div>
    </div>
  );
}