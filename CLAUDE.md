# CLAUDE.md

## Context

This repo is a take-home task for a **Senior Backend Engineer** role.
Read `README.md` and `senior-backend-engineer/instructions.md` before doing anything.

- Stack (fixed): **.NET 8**, **FastEndpoints**, **CQRS**, **DDD**, **Modular Monolith**.
- Two modules: **Users** (create by username, get by id) and **WorkItems** (create with name + assignee user id, list by assignee).
- Time box: 2–4 hours. Evaluated on architecture quality and clarity, NOT feature count. Do not gold-plate.
- Out of scope: auth, CI/CD, logging/metrics infra, containerization, UI, exhaustive tests.
- The `ai-journey/` folder is a required deliverable and is weighted like the code itself.

## Working agreement (always follow)

1. **No code before an approved plan.** For any non-trivial step, propose a plan first and wait for approval.
2. **Ask, don't assume.** When something is ambiguous, ask me. Use the AskUserQuestion tool when available.
   Every question MUST include: 2–3 options, the trade-off of each, and your **recommended default** (marked "Recommended").
   Batch questions (max 4 at a time). Never silently pick an answer to an ambiguous requirement.
3. **Log every decision** in `ai-journey/decisions.md` right after I answer, using the table format in that file.
   Record your proposal and my final choice separately. If I overrode your recommendation, say so explicitly.
4. **Save plans.** After a plan is approved, write it verbatim to `ai-journey/plan.md`
   (append later plans as new dated sections; never delete earlier versions).
   A hook also snapshots plans automatically into `ai-journey/plans/`, but `plan.md` is the curated one.
5. **Work in vertical slices.** One slice = domain + application + endpoint + test. Build and test must be green before moving on.
6. **Small commits.** Suggest a Conventional Commit message after each green slice (`feat(users): ...`, `test(arch): ...`). After each green slice, commit with the suggested message, including ai-journey/ changes.
7. **Be honest about mistakes.** If you change your mind or find a bug in your own earlier output, note it in `ai-journey/decisions.md` under "Corrections".
8. **Propose, then ask.** At the start of each phase, list the open questions you see for that phase yourself (don't wait for me to spot them), each with your recommended default.

## ai-journey/ — what is automatic vs. what you maintain

Captured automatically by hooks in `.claude/hooks/` (do NOT edit these files by hand):
- `prompts-raw.md` — every prompt I submit (`UserPromptSubmit` hook)
- `questions-raw.md` — every AskUserQuestion you ask + my answer
- `plans/NN_*.md` — verbatim snapshot of every approved plan (`PostToolUse(ExitPlanMode)`)
- `transcripts/<session>.{jsonl,md}` — full session export after each turn (`Stop` hook)

Maintained by you (Claude), when I ask or at the end of each phase:
- `plan.md` — curated plan, dated sections, never rewrite history
- `decisions.md` — decision log + "Corrections" + "Where I overrode the AI"

Maintained by me, with your draft only on request: `prompts.md`, `toolchain.md`, `judgment.md`.

## Architecture guardrails

- A module exposes ONLY its `*.Contracts` project to other modules. No references to another module's Domain, Application, or Infrastructure.
- Each module owns its data (its own DbContext / store). No cross-module joins or shared tables.
- Domain models are rich: private setters, factory methods, invariants enforced in the domain, value objects for concepts like `Username`, `WorkItemName`, strongly-typed IDs.
- Endpoints are thin: map request → command/query, send, map result → response. No business logic in endpoints.
- Commands change state and return at most an ID. Queries never change state.
- Prefer `internal` for everything that does not need to be public.
- Architecture tests (NetArchTest or ArchUnitNET) must enforce module boundaries.

## Commands

Solution: `JTL-BE.sln` (targets `net8.0`; builds with the installed .NET 10 SDK + .NET 8 runtime).
Warnings are errors. Package versions live only in `Directory.Packages.props` (central package management).

- Build: `dotnet build`
- Test (all): `dotnet test`
- Test (architecture only): `dotnet test tests/Architecture.Tests`
- Run: `dotnet run --project src/Host` → listens on `http://localhost:5000` (Kestrel default; no launchSettings)
  - Swagger UI: `http://localhost:5000/swagger` · OpenAPI JSON: `http://localhost:5000/swagger/v1/swagger.json`
- Exercise the API: `requests.http` at the repo root (all 4 endpoints + one error case each; run top to bottom,
  ids flow via named requests). Data is in-memory and resets on every restart.

Layout: `src/Host` (composition root), `src/BuildingBlocks` (Result/Error, CQRS markers, error→problem-details),
`src/Modules/<Module>/<Module>` (internal layers as folders) + `src/Modules/<Module>/<Module>.Contracts` (public surface),
`tests/{Architecture,Users,WorkItems,Api}.Tests`. Register a new module in `src/Host/Program.cs` (`o.Assemblies`) and in `tests/Architecture.Tests/Modules.cs`.
