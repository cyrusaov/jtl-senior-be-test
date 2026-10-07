// Stop: after every Claude turn, export the session transcript into ai-journey/transcripts/
//   <session>.jsonl  raw copy (complete, includes tool calls)
//   <session>.md     readable version: my prompts + Claude's text replies, tool calls summarised
import fs from "node:fs";
import path from "node:path";
import { run, journeyDir } from "./_util.mjs";

function textOf(content) {
  if (typeof content === "string") return { text: content, tools: [] };
  const parts = [];
  const tools = [];
  for (const b of content ?? []) {
    if (b.type === "text" && b.text) parts.push(b.text);
    else if (b.type === "tool_use") tools.push(b.name);
  }
  return { text: parts.join("\n\n"), tools };
}

await run(async (input) => {
  const src = input.transcript_path;
  if (!src || !fs.existsSync(src)) return;

  const dir = path.join(journeyDir(input), "transcripts");
  fs.mkdirSync(dir, { recursive: true });
  const id = (input.session_id ?? path.basename(src, ".jsonl")).slice(0, 8);

  fs.copyFileSync(src, path.join(dir, `${id}.jsonl`));

  let md = `# Session ${id}\n\n_Auto-exported by save-transcript.mjs. Raw JSONL alongside._\n`;
  for (const line of fs.readFileSync(src, "utf8").split("\n")) {
    if (!line.trim()) continue;
    let e;
    try { e = JSON.parse(line); } catch { continue; }
    if ((e.type !== "user" && e.type !== "assistant") || e.isMeta) continue;

    const { text, tools } = textOf(e.message?.content);
    if (!text.trim() && !tools.length) continue; // tool results etc.
    if (e.type === "user" && text.startsWith("<")) continue; // system/command wrappers

    md += e.type === "user" ? `\n---\n\n## 🧑 Me\n\n${text}\n` : `\n**🤖 Claude:**\n\n${text}\n`;
    if (tools.length) md += `\n_tools: ${tools.join(", ")}_\n`;
  }
  fs.writeFileSync(path.join(dir, `${id}.md`), md);
});
