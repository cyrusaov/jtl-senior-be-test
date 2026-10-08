# Judgment

## Where AI helped

- **Turning ambiguity into explicit choices.** Before writing any code, Claude listed the open questions per phase.
  Each one came with 2–3 options, their trade-offs and a recommended default. That gave me 29 logged decisions instead
  of silent assumptions, e.g. 422 vs. 400 for an unknown assignee (#16), or `200 []` vs. 404 for an unknown user (#3).
- **Comparing libraries with current facts.** For CQRS dispatch it compared MediatR (commercial since v13), Mediator
  (source-generated, but it needs public handlers) and the FastEndpoints bus. That made the trade-off against `internal`
  visible (#7, #10).
- **Speed on the mechanical parts.** Solution skeleton, value objects, handlers, API tests and NetArchTest rules came
  out slice by slice, each building and green before the next. That left my time for reviewing rather than typing.
- **Reviewing its own work with a fresh context.** The independent subagent review found real problems: the missing
  README (Clarity 2/5) and the domain transitively depending on ASP.NET through `BuildingBlocks`. The author context
  had missed both.

## Where AI was wrong or unhelpful

- **The approved plan contradicted itself.** It said query handlers read the DbContext directly, and also that
  Application must not depend on Infrastructure. Followed literally, the first query handler would have broken the
  planned architecture test. It was caught before any code was written and resolved with a read-store port (#20,
  Correction 3).
- **It claimed things the code didn't do.** A comment in `Messages.cs` said architecture tests enforce the CQRS
  split, but no such test existed until the review flagged it. A "(next slice)" note also outlived its slice
  (Correction 4). This was the pattern I trusted least: confident wording with no evidence behind it.
- **A silent tooling failure.** The plan-snapshot hook read the plan from the wrong part of the payload, so the
  approved plan wasn't saved and nothing errored. Claude noticed the missing file afterwards, fixed the hook and
  back-filled the snapshot (Correction 1).
- **Acting on stale state.** It asked me how to commit slice 0 based on a `git status` from the start of the session.
  I had already committed it (Correction 2).

## Where I overrode it
(see [`decisions.md`](./decisions.md) for the full list)

- **#25:** Claude wanted to fix two convention drifts. I kept the read-store port fix, which is production code, and
  declined the test-helper refactor as low value for test-only code.
- **#29:** Claude recommended keeping a "Known limitations" section and adding no diagram. I dropped the section,
  moved the most important caveat into the README's decisions table, and asked for a sequence diagram of the one
  cross-module call.
- **#10:** I didn't take the first CQRS answer. I made it revisit #7 with Mediator added to the comparison, and the
  original choice held.

I accepted most recommended defaults, and I think that is the expected outcome of how I set it up, not a lack of
scrutiny. Every recommendation had to come with alternatives and trade-offs. My job was to check that reasoning,
and I logged the cases where I disagreed.

## What I'd do differently with AI next time

- **Run the independent review after the first slice, not near the end.** It found the most important issues, and
  some of them (the domain → framework dependency) would have been cheaper to fix early.
- **Add a rule to `CLAUDE.md`: any claim that something is "enforced" must name the test that enforces it.** That
  would have prevented Correction 4.
- **Make the agent re-check repository state (`git status`/`git log`) right before asking about it,** and make the
  hooks fail loudly instead of silently.
- **Have it check the plan for internal consistency before I approve it.** The contradiction in #20 was in the plan
  I had approved.
