import React, { useEffect, useRef } from "react";
import MessageBubble from "./MessageBubble.jsx";

export default function ChatWindow({ messages }) {
  const bottomRef = useRef(null);

  useEffect(() => {
    bottomRef.current?.scrollIntoView({ behavior: "smooth" });
  }, [messages]);

  return (
    <div className="flex h-[65vh] flex-col gap-3 overflow-y-auto p-4 sm:p-6">
      {messages.map((m, idx) => (
        <MessageBubble key={idx} role={m.role} content={m.content} />
      ))}
      <div ref={bottomRef} />
    </div>
  );
}