import React from "react";

const roleStyles = {
  user: {
    bg: "bg-emerald-500/10 border-emerald-500/30",
    text: "text-emerald-50",
    badge: "bg-emerald-500 text-emerald-950",
  },
  assistant: {
    bg: "bg-slate-800/80 border-slate-700",
    text: "text-slate-100",
    badge: "bg-slate-700 text-slate-200",
  },
};

export default function MessageBubble({ role, content }) {
  const style = role === "user" ? roleStyles.user : roleStyles.assistant;
  return (
    <div
      className={`w-full rounded-xl border ${style.bg} ${style.text} shadow-lg shadow-slate-950/50`}
    >
      <div className="flex items-center gap-2 border-b border-white/5 px-4 py-2 text-xs uppercase tracking-[0.25em] text-slate-400">
        <span
          className={`rounded-full px-2 py-1 text-[10px] font-semibold ${style.badge}`}
        >
          {role === "user" ? "You" : "Assistant"}
        </span>
        <span className="text-[11px]">Conversation</span>
      </div>
      <div className="px-4 py-3 text-base leading-relaxed whitespace-pre-wrap">
        {content}
      </div>
    </div>
  );
}