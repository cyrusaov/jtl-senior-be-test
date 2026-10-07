// Shared helpers for the ai-journey hooks.
// Hooks must never break the session: every failure is swallowed and the process exits 0.
import fs from "node:fs";
import path from "node:path";

export async function readStdinJson() {
  const chunks = [];
  for await (const chunk of process.stdin) chunks.push(chunk);
  const raw = Buffer.concat(chunks).toString("utf8").trim();
  return raw ? JSON.parse(raw) : {};
}

export function journeyDir(input) {
  const root = process.env.CLAUDE_PROJECT_DIR || input.cwd || process.cwd();
  const dir = path.join(root, "ai-journey");
  fs.mkdirSync(dir, { recursive: true });
  return dir;
}

// Local timestamp like "2026-10-07 20:44"
export function stamp(d = new Date()) {
  const p = (n) => String(n).padStart(2, "0");
  return `${d.getFullYear()}-${p(d.getMonth() + 1)}-${p(d.getDate())} ${p(d.getHours())}:${p(d.getMinutes())}`;
}

export function fileStamp(d = new Date()) {
  return stamp(d).replace(/[: ]/g, (c) => (c === " " ? "_" : ""));
}

export function appendWithHeader(file, header, text) {
  if (!fs.existsSync(file)) fs.writeFileSync(file, header + "\n");
  fs.appendFileSync(file, text);
}

export async function run(fn) {
  try {
    await fn(await readStdinJson());
  } catch (err) {
    // Report to stderr (visible in --debug) but never block Claude.
    process.stderr.write(`[ai-journey hook] ${err?.message ?? err}\n`);
  }
  process.exit(0);
}
