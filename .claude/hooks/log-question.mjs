// PostToolUse(AskUserQuestion): record each clarifying question Claude asked,
// the options it proposed (incl. its "Recommended" pick) and what I answered.
// This is the raw evidence behind ai-journey/decisions.md.
import path from "node:path";
import { run, journeyDir, stamp, appendWithHeader } from "./_util.mjs";

function findAnswers(input) {
  const candidates = [input.tool_input?.answers, input.tool_response?.answers, input.tool_response];
  for (const c of candidates) if (c && typeof c === "object" && !Array.isArray(c)) return c;
  return null;
}

await run(async (input) => {
  const questions = input.tool_input?.questions ?? [];
  if (!questions.length) return;

  const answers = findAnswers(input) ?? {};
  let md = `\n---\n\n### ${stamp()}\n`;

  for (const q of questions) {
    md += `\n**Q (${q.header ?? "question"}): ${q.question}**\n\n`;
    for (const o of q.options ?? []) {
      md += `- ${o.label}${o.description ? ` — ${o.description}` : ""}\n`;
    }
    const a = answers[q.question];
    md += `\n➡️ **My answer:** ${a ? (typeof a === "string" ? a : JSON.stringify(a)) : "_(see transcript)_"}\n`;
  }

  if (!Object.keys(answers).length && input.tool_response) {
    md += `\n<details><summary>raw tool response</summary>\n\n\`\`\`json\n${JSON.stringify(input.tool_response, null, 2)}\n\`\`\`\n</details>\n`;
  }

  appendWithHeader(
    path.join(journeyDir(input), "questions-raw.md"),
    "# Clarifying questions asked by Claude\n\n_Captured automatically by a `PostToolUse(AskUserQuestion)` hook. Curated decisions: [`decisions.md`](./decisions.md)._\n",
    md
  );
});
