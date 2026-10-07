# AI journey

How I used AI on this task, and where my own judgment came in.

| File | What it is | Written by |
| --- | --- | --- |
| [`plan.md`](./plan.md) | The plan I worked from, with dated revisions | Claude, approved by me |
| [`plans/`](./plans/) | Verbatim snapshots of every approved plan | Hook (automatic) |
| [`decisions.md`](./decisions.md) | Every design decision: AI proposal vs. my final call | Claude, reviewed by me |
| [`prompts.md`](./prompts.md) | Curated key prompts with why I wrote them that way | Me |
| [`prompts-raw.md`](./prompts-raw.md) | Every prompt, unedited | Hook (automatic) |
| [`questions-raw.md`](./questions-raw.md) | Clarifying questions Claude asked me, with my answers | Hook (automatic) |
| [`transcripts/`](./transcripts/) | Full session transcripts (JSONL + readable Markdown) | Hook (automatic) |
| [`toolchain.md`](./toolchain.md) | Tools, models, skills, MCP servers and what each was for | Me |
| [`judgment.md`](./judgment.md) | Where AI helped, where it was wrong, where I overrode it | Me |

**How the capture works:** `.claude/settings.json` registers four Claude Code hooks
(`.claude/hooks/*.mjs`). I set this up before writing any code so the record is
complete rather than reconstructed afterwards.
