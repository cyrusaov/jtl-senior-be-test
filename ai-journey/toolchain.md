# Toolchain

| Tool | Model / version | Used for |
| --- | --- | --- |
| Claude Code (CLI) | _e.g. Claude Opus 5.5_ | Main agent: analysis, planning, implementation |
| Claude Code plan mode | same | Architecture plan before any code |
| Claude Code subagent (`Plan` / `general-purpose`) | same | Independent review against the evaluation criteria |
| Claude Code hooks (`.claude/hooks/`) | — | Auto-capture prompts, plans, questions, transcripts |
| `CLAUDE.md` | — | Persistent working agreement + architecture guardrails |
| MCP servers | _none / list_ | |
| Skills | _none / list_ | |

## What I deliberately did NOT use
- _e.g. no parallel multi-agent implementation: 4 endpoints, coherence of DDD decisions mattered more than speed._
