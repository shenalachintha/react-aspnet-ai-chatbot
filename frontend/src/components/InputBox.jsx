import React, { useState } from "react";

export default function InputBox({ onSend, disabled }) {
  const [text, setText] = useState("");

  const submit = (e) => {
    e.preventDefault();
    onSend(text);
    setText("");
  };

  return (
    <form
      onSubmit={submit}
      className="flex flex-col gap-3 sm:flex-row sm:items-center"
    >
      <div className="relative flex-1">
        <textarea
          rows={2}
          className="w-full resize-none rounded-xl border border-slate-700 bg-slate-900/70 px-4 py-3 text-slate-100 shadow-inner shadow-slate-950/60 outline-none ring-1 ring-transparent transition focus:border-emerald-500/70 focus:ring-emerald-500/40 disabled:opacity-60"
          value={text}
          onChange={(e) => setText(e.target.value)}
          placeholder="Ask anything…"
          disabled={disabled}
        />
        <div className="pointer-events-none absolute bottom-2 right-3 text-[11px] uppercase tracking-[0.2em] text-slate-500">
          Enter to send
        </div>
      </div>
      <button
        type="submit"
        disabled={disabled}
        className="inline-flex items-center justify-center rounded-xl bg-emerald-500 px-5 py-3 text-sm font-semibold text-emerald-950 shadow-lg shadow-emerald-500/30 transition hover:-translate-y-[1px] hover:shadow-emerald-500/40 disabled:cursor-not-allowed disabled:opacity-60"
      >
        Send
      </button>
    </form>
  );
}