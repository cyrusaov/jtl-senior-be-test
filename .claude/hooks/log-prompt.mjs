// UserPromptSubmit: append every prompt I type to ai-journey/prompts-raw.md.
// Start a prompt with [nolog] to keep it out of the log (e.g. throwaway questions).
import path from "node:path";
import { run, journeyDir, stamp, appendWithHeader } from "./_util.mjs";

await run(async (input) => {
  const prompt = (input.prompt ?? "").trim();
  if (!prompt || prompt.startsWith("[nolog]")) return;

  const file = path.join(journeyDir(input), "prompts-raw.md");
  const header =
    "# Raw prompt log\n\n" +
    "_Captured automatically by a `UserPromptSubmit` hook (`.claude/hooks/log-prompt.mjs`)._\n" +
    "_Curated version with commentary: [`prompts.md`](./prompts.md)._\n";

  appendWithHeader(
    file,
    header,
    `\n---\n\n### ${stamp()} · session \`${(input.session_id ?? "").slice(0, 8)}\`\n\n${prompt}\n`
  );
});
