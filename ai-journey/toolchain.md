# Toolchain

All of this was built in a single Claude Code session ([`transcripts/8afeacf8.md`](./transcripts/8afeacf8.md),
2026-10-07). The other transcript (`40df2274`) is a later session where I prepared the presentation; it changed no code.

| Tool | Model / version | Used for |
| --- | --- | --- |
| Claude Code (CLI) | Claude Code 2.1.292, model `claude-opus-5-5` (Claude Opus 5.5) | Main agent: requirements analysis, planning, implementation, tests, docs. It ran in auto permission mode, with the working agreement in `CLAUDE.md` as the gate. |
| Claude Code plan mode | same | The architecture plan, approved before any code was written ([`plans/01_2026-10-07_2136.md`](./plans/01_2026-10-07_2136.md)) |
| `AskUserQuestion` | same | 12 batches of clarifying questions, each with options, trade-offs and a recommended default. Logged in [`questions-raw.md`](./questions-raw.md) and [`decisions.md`](./decisions.md) |
| Claude Code subagent (`general-purpose`) | same | One independent "hiring-panel" review: a fresh context that got only the repo and `instructions.md`, not `ai-journey/`. Findings and triage are in [`decisions.md`](./decisions.md#independent-review-2026-10-07) |
| Claude Code hooks (`.claude/hooks/`, Node.js) | — | Automatic capture: prompts (`UserPromptSubmit`), approved plans (`PostToolUse: ExitPlanMode`), questions and answers (`PostToolUse: AskUserQuestion`), and transcripts (`Stop`) |
| `CLAUDE.md` | — | Persistent working agreement (plan first, ask with a recommended default, log every decision, vertical slices, small commits) plus the architecture guardrails |
| MCP servers | none used | Some were available in the environment (e.g. a codebase-graph server), but the transcript has no MCP calls. The codebase was small enough to read directly. |
| Skills / plugins | none invoked | A skills plugin was installed, but no skill was called. `CLAUDE.md` already defined the process I wanted. |

Non-AI tools that check the AI's output: the .NET 10 SDK with warnings as errors, xUnit, NetArchTest (architecture
rules), `WebApplicationFactory` (API tests), and git, with one commit per green slice.

## What I deliberately did NOT use
- **No parallel multi-agent implementation.** There are only four endpoints, and keeping the DDD/CQRS decisions
  consistent mattered more than speed. Every slice was built sequentially in one context.
- **Only one subagent, used for review only.** Its value was a context that hadn't seen the decisions being made,
  not extra throughput.
- **No AI-generated decisions without a log entry.** Every design choice went through `AskUserQuestion` and
  `decisions.md`, even when I accepted the recommended default.
