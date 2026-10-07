// PostToolUse(ExitPlanMode): snapshot every approved plan, verbatim, into ai-journey/plans/.
// Rejected plans don't reach PostToolUse, so this folder only holds plans that were accepted.
import fs from "node:fs";
import path from "node:path";
import { run, journeyDir, fileStamp, stamp } from "./_util.mjs";

// Newer Claude Code versions call ExitPlanMode without input and return { plan, filePath }
// in tool_response; older ones passed the plan as tool_input.plan. Support both.
function resolvePlan(input) {
  const res = typeof input.tool_response === "object" ? input.tool_response : {};
  if (input.tool_input?.plan) return input.tool_input.plan;
  if (res?.plan) return res.plan;
  if (res?.filePath && fs.existsSync(res.filePath)) return fs.readFileSync(res.filePath, "utf8");
  return undefined;
}

await run(async (input) => {
  const plan = resolvePlan(input);
  if (!plan) {
    process.stderr.write("[ai-journey hook] save-plan: no plan found in tool_input or tool_response\n");
    return;
  }

  const dir = path.join(journeyDir(input), "plans");
  fs.mkdirSync(dir, { recursive: true });

  const n = fs.readdirSync(dir).filter((f) => f.endsWith(".md")).length + 1;
  const file = path.join(dir, `${String(n).padStart(2, "0")}_${fileStamp()}.md`);
  fs.writeFileSync(
    file,
    `<!-- Auto-saved by save-plan.mjs at ${stamp()} (session ${input.session_id ?? "?"}) — verbatim, do not edit -->\n\n${plan}\n`
  );
});
