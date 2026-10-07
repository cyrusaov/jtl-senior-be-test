# Clarifying questions asked by Claude

_Captured automatically by a `PostToolUse(AskUserQuestion)` hook. Curated decisions: [`decisions.md`](./decisions.md)._


---

### 2026-10-07 21:25

**Q (Username): Username rules: what invariants should the Username value object enforce?**

- Unique, case-insens. (Recommended) — Trimmed, 3–32 chars, [a-zA-Z0-9._-], uniqueness checked case-insensitively (stored as entered + normalized key). Shows a real invariant in the VO and a uniqueness rule in the app layer. Cost: one repository lookup + a 409 path.
- Unique, case-sensitive — Same format rules but 'Alice' and 'alice' are different users. Simpler, but surprising for humans and a classic impersonation footgun.
- Format only, no uniqueness — Just non-empty + max length. Least work, but the Username VO becomes nearly anemic and loses the most interesting domain rule.

➡️ **My answer:** Unique, case-insens. (Recommended)

**Q (Assignee): Must the assignee user exist when a work item is created?**

- Validate via Contracts (Recommended) — WorkItems calls a Users.Contracts interface (e.g. IUserExistence) in-process; unknown user → 404/422. Demonstrates a clean cross-module contract. Trade-off: runtime coupling (WorkItems needs Users available).
- No validation — Accept any well-formed UserId. Zero coupling, but allows orphaned work items and dodges the most interesting boundary question in the task.
- Local replica via events — Users publishes UserCreated; WorkItems keeps its own known-users set. Fully decoupled and eventually consistent, but it's gold-plating for a 2–4h task (outbox, races).

➡️ **My answer:** Validate via Contracts (Recommended)

**Q (Unknown user): GET work items for a user id that does not exist (or has none):**

- 200 with empty list (Recommended) — Query is just a filter over WorkItems' own data; no cross-module call on the read side, keeps queries cheap and side-effect free. Trade-off: client can't distinguish 'no such user' from 'no items'.
- 404 if user unknown — More informative for clients, but every read now depends on the Users module (extra call, coupling on the query path).

➡️ **My answer:** 200 with empty list (Recommended)

**Q (ID type): ID type for UserId / WorkItemId?**

- Strongly-typed Guid (Recommended) — readonly record struct UserId(Guid Value), generated in the domain factory. Aggregate can be created with its identity without a DB round-trip; non-guessable. Trade-off: less readable URLs; .NET 8 has no built-in Guid v7 (random v4 is fine in-memory).
- Strongly-typed long (store-generated) — Readable ids (/users/42), but identity comes from persistence, so the aggregate is incomplete until saved and ids are enumerable.
- ULID / string — Sortable and readable, but adds a dependency and custom (de)serialization for little gain here.

➡️ **My answer:** Strongly-typed Guid (Recommended)

---

### 2026-10-07 21:28

**Q (Errors): Error / response contract for failures (validation, not found, conflict)?**

- Result + ProblemDetails (Recommended) — Handlers return a Result/ErrorOr-style value for expected failures (NotFound, Conflict, Validation); endpoints map to RFC 7807 via FastEndpoints' ProblemDetails support. Expected errors aren't exceptions; explicit and testable. Cost: a small Result type + mapping helper.
- Domain exceptions + global handler — Throw DomainException/NotFoundException, one middleware maps to ProblemDetails. Less code, but control flow via exceptions and the failure modes are invisible in handler signatures.
- FastEndpoints defaults only — Use ThrowError/AddError and FE's default error shape. Fastest, but business failures leak into the endpoint layer, against the thin-endpoint goal.

➡️ **My answer:** Result + ProblemDetails (Recommended)

**Q (Persistence): Persistence?**

- EF Core InMemory, DbContext per module (Recommended) — UsersDbContext and WorkItemsDbContext, each internal to its module's Infrastructure. Makes 'each module owns its data' visible and swaps to SQL/schema-per-module with a provider change. Trade-off: InMemory provider ignores unique constraints/transactions, so uniqueness is enforced in code.
- Hand-rolled in-memory repos — ConcurrentDictionary-backed repositories per module. No EF dependency, very transparent, but less realistic and you'd re-solve querying by hand.
- SQLite via EF Core — Real constraints (unique index on username) and migrations; more setup and file/connection handling for little evaluation gain.

➡️ **My answer:** EF Core InMemory, DbContext per module (Recommended)

**Q (CQRS lib): CQRS dispatch mechanism?**

- FastEndpoints command bus (Recommended) — Use FE's built-in ICommand<T>/ICommandHandler with our own ICommand/IQuery marker interfaces on top to make the split explicit. No extra dependency; MediatR is commercially licensed since v13 (2025). Trade-off: FE's bus doesn't natively distinguish queries, so the split is by convention + arch test.
- MediatR — Familiar to most .NET reviewers, pipeline behaviors for validation. Adds a dependency with license considerations (pin v12 or accept the commercial license).
- Hand-rolled dispatcher — ~40 lines, total control, very explicit ICommandHandler/IQueryHandler split. Reinvents what FE already ships.

➡️ **My answer:** FastEndpoints command bus (Recommended)

**Q (Tests): Test scope for the 'few tests that show how you'd test the design'?**

- Arch + domain + 1 API per module (Recommended) — NetArchTest boundary rules, unit tests for value objects/aggregates, one WebApplicationFactory integration test per module (incl. cross-module assignee check). Covers each layer once without chasing coverage.
- Arch + domain only — Fastest; but doesn't prove endpoints/wiring work end-to-end.
- Add handler unit tests too — Most thorough; more fakes/mocks to maintain, drifts toward the 'exhaustive' they said not to do.

➡️ **My answer:** Arch + domain + 1 API per module (Recommended)

---

### 2026-10-07 21:33

**Q (Layout): How many projects per module? (Biggest structural choice; CLAUDE.md wants 'internal' by default AND layer separation.)**

- Module + Contracts (Recommended) — 2 projects per module: Users.Contracts (public) + Users (Domain/Application/Infrastructure/Endpoints as folders, all internal). Internals are compiler-hidden even if someone adds a reference; layer direction inside the module enforced by NetArchTest namespace rules. Fewer projects, 'internal' actually works. Cost: layering inside a module is test-enforced, not compiler-enforced.
- Classic 4–5 per module — Users.Domain / .Application / .Infrastructure / .Endpoints / .Contracts. Layering enforced by the compiler and instantly familiar to DDD reviewers. Cost: ~10 module projects, and most types must become public (or a web of InternalsVisibleTo) to cross project lines, which weakens the module boundary.
- Middle: Core + Infra + Contracts — Users.Core (Domain+Application), Users.Infrastructure (EF + endpoints), Users.Contracts. Compiler keeps EF out of the domain; still needs InternalsVisibleTo between Core and Infra. A compromise that is neither the simplest nor the most explicit.

➡️ **My answer:** Module + Contracts (Recommended)

**Q (CQRS lib): Which CQRS dispatcher? (Revisits decision #7 now that Mediator is on the table.)**

- FastEndpoints bus (Recommended) — Already in the stack, no extra dependency, and handlers/command middleware can stay internal (FE discovers them by reflection). We add our own ICommand<T>/IQuery<T> markers over FE's ICommand<T>, and an arch test enforces the split. Cost: the bus has no built-in query concept, so the split is by our convention; it also ties the application layer to FastEndpoints.
- Mediator (source-gen) — MIT-licensed, compile-time generated, has separate ICommand/IQuery/handler interfaces out of the box, so the CQRS split is native, and the application layer stays independent of the web framework. Cost: one more dependency, and the generated dispatcher in Host can't see internal handlers in other assemblies, so handlers must be public (conflicts with 'internal by default').
- MediatR — Most familiar to reviewers. Since v13 (2025) it is commercially licensed with a key (a free community tier exists for small orgs); v12 is still Apache-2.0 but frozen. A licensing detour, and the dependency isn't worth it for 4 handlers.

➡️ **My answer:** FastEndpoints bus (Recommended)

**Q (Contract): How does WorkItems ask Users 'does this user exist?' (sync in-process is already decided; this is the shape of the contract)**

- Facade interface (Recommended) — Users.Contracts exposes `IUserDirectory { Task<bool> ExistsAsync(UserId) }` (or returns UserSummary?). Users implements it internally against its own DbContext and registers it in DI. Explicit, trivially fakeable in WorkItems tests, and the contract surface is obvious. Cost: a second entry point into Users besides the bus.
- Public query via the bus — Users.Contracts exposes `UserExistsQuery : IQuery<bool>`; WorkItems sends it on the bus. Uniform 'everything is a message' style, easy to move out-of-process later. Cost: harder to fake (bus is static in FE), contract is less discoverable, and it blurs the module's internal CQRS with its public API.

➡️ **My answer:** Facade interface (Recommended)

**Q (Uniqueness): Where is username uniqueness enforced (a rule that spans all User aggregates)?**

- App handler + repo check (Recommended) — CreateUserHandler asks `IUserRepository.ExistsByUsernameAsync(username)` and returns Conflict before calling User.Create. Simple, honest about where the set-level check really happens. Cost: the domain doesn't 'own' the rule; a second creation path could forget it. Race not covered with InMemory (documented; a unique index fixes it in SQL).
- Domain service in factory — `User.Create(username, IUsernameUniquenessChecker)` so the domain can't build a user without the check. Rule visibly lives in the domain. Cost: async I/O leaks into the domain factory, and it's still not race-safe; many consider this over-engineering.

➡️ **My answer:** App handler + repo check (Recommended)

---

### 2026-10-07 21:34

**Q (Validation): Input validation: where do format rules (e.g. username length/charset) live?**

- Value objects only (Recommended) — Username.Create / WorkItemName.Create return Result<T>; handler maps failure to a 400 ProblemDetails with field errors. One source of truth for each rule, in the domain. FE validators only for transport-level shape (required, Guid parse). Cost: errors come back one-at-a-time per VO rather than all fields in one pass.
- FE validators + VOs — FluentValidation validators on requests duplicate the rules for fast, aggregated 400s; VOs re-check as last line of defense. Nicer client errors. Cost: rules duplicated in two places and can drift.

➡️ **My answer:** Value objects only (Recommended)

**Q (Data access): Data access shape (commands vs queries):**

- Repo for writes, DbContext reads (Recommended) — Commands go through an aggregate repository (IUserRepository) defined in Domain; query handlers read the module DbContext directly with AsNoTracking + projection to DTOs. This is the asymmetry CQRS is for. Cost: two access styles in one module (intentional, explained in README).
- Repositories for both — Every handler uses repositories; uniform and easy to fake. Cost: query methods bloat the repo, and reads load full aggregates just to map them to DTOs, which blurs the CQRS split.

➡️ **My answer:** Repo for writes, DbContext reads (Recommended)

**Q (Route): Route for 'list work items by assignee':**

- GET /work-items?assigneeId= (Recommended) — Clearly a WorkItems resource with a filter; matches 'empty list for unknown user' (decision #3). Cost: assigneeId required, so a missing param must be a 400 rather than 'list all'.
- GET /users/{id}/work-items — Reads naturally as a sub-resource. Cost: WorkItems module squats in the Users route space, and nesting under a user implies the user should exist (404), which contradicts decision #3.

➡️ **My answer:** GET /work-items?assigneeId= (Recommended)

**Q (Bad assignee): Status code when creating a work item with an unknown assigneeId:**

- 422 ProblemDetails (Recommended) — Request is well-formed but references something that doesn't exist: a semantic error. Distinguishes it from malformed input (400). Cost: some teams find 422 unusual.
- 400 with field error — Treat it as validation on assigneeId. Simplest for clients (one error path). Cost: mixes a lookup failure with format validation.
- 404 — Common in the wild, but misleading: the endpoint exists and the target resource isn't the one in the URL.

➡️ **My answer:** 422 ProblemDetails (Recommended)

---

### 2026-10-07 22:47

**Q (Commits): CLAUDE.md now says to commit after each green slice. Slice 0 (skeleton) and the hook fix are still uncommitted. How should they land?**

- 3 commits on main (Recommended) — Commit now, before slice 1 code: (1) fix(ai-journey) hook fix, (2) chore: scaffold skeleton, then (3) feat(users): create user at the end of this slice. Clean, reviewable history that mirrors the slices; directly on main, as a take-home repo usually is.
- Fold into one slice-1 commit — Single commit at the end containing skeleton + hook + CreateUser. Fewer steps, but loses the slice-by-slice history reviewers find useful.
- Feature branch per slice — Branch, commit, merge to main per slice. Most 'production-like', but ceremony with no reviewer for a solo take-home.

➡️ **My answer:** 3 commits on main (Recommended)

**Q (Location): POST /users must return a Location header, but GET /users/{id} only arrives in slice 2. How to handle it now?**

- Literal /users/{id} now (Recommended) — Set Location to the agreed route string today; in slice 2 switch to FastEndpoints CreatedAtAsync<GetUserByIdEndpoint> so the link is derived from the real route. Contract is correct from day one; one tiny follow-up edit.
- Omit until slice 2 — Return 201 + {id} without Location, add it with the GET endpoint. Never advertises a route that 404s, but the slice-1 API test can't pin the full contract.

➡️ **My answer:** Literal /users/{id} now (Recommended)

**Q (EF Core): EF Core version for the InMemory provider (runtime is .NET 8)?**

- EF Core 8.0.x (Recommended) — Matches the .NET 8 LTS runtime and the stack; support ends with .NET 8 (Nov 2026), same as everything else here. Zero surprises.
- EF Core 9.0.x — Also runs on net8.0, a few newer features (none needed here). Mixed-version stack is something to explain for no benefit.

➡️ **My answer:** EF Core 8.0.x (Recommended)

---

### 2026-10-07 22:54

**Q (Read path): Plan conflict: query handlers read the DbContext (Infrastructure) but Application must not depend on Infrastructure. How to resolve?**

- Read-store port in Application (Recommended) — Application/GetUserById defines `IUserReadStore.GetByIdAsync(Guid) -> UserDto?`; Infrastructure implements it with AsNoTracking + Select projection. Handler stays in Application next to the command handler (same layout), layering rule holds, handler is unit-testable. Still not the aggregate repository, so decision #14's asymmetry survives. Cost: one small interface per read use case.
- Query handlers in Infrastructure — Query + DTO in Application, handler in Infrastructure/Queries/ using the DbContext directly. Leanest read side, common in CQRS. Cost: command and query handlers live in different layers (the inconsistency you asked me to avoid), and the arch rule needs a carve-out.
- Relax the layering rule — Let Application query handlers use UsersDbContext directly. Least code. Cost: EF Core leaks into Application; 'Application must not depend on Infrastructure' no longer holds and can't be arch-tested.

➡️ **My answer:** Read-store port in Application (Recommended)

**Q (Error API): Error factory signatures are inconsistent (Validation takes field first, Conflict/Unprocessable take it last). Fix?**

- Unify to (code, message, field) (Recommended) — Every factory: Error.X(code, message, field?) — Validation keeps field required. Tiny change now (4 call sites), prevents swapped-argument bugs later.
- Leave as is — No churn; the asymmetry stays and every new call site has to remember which order applies.

➡️ **My answer:** Unify to (code, message, field) (Recommended)

**Q (Errors home): Where should application-level errors (UsernameTaken, the new UserNotFound) live?**

- One UserErrors catalog in Domain (Recommended) — Keep a single per-module list of error codes; 'username taken' and 'user not found' are still domain language even if the application layer detects them. One place for clients/devs to find every code. Cost: Domain holds errors it doesn't raise itself.
- Split Domain / Application — VO errors stay in Domain/UserErrors; UsernameTaken and UserNotFound move to Application/UserApplicationErrors. Purer layering. Cost: two catalogs to search, more files for 4 errors.

➡️ **My answer:** One UserErrors catalog in Domain (Recommended)

---

### 2026-10-07 23:07

**Q (Contract): What should Users.Contracts return for a user lookup?**

- bool ExistsAsync(Guid) (Recommended) — `IUserDirectory.ExistsAsync(Guid userId, ct)`, no DTO. Exactly what WorkItems needs today (assignee validation); smallest surface, nothing about a user leaks across the boundary, trivial to fake. Cost: if WorkItems later needs the username, the contract must grow (a deliberate, visible change).
- UserSummary? FindAsync(Guid) — Returns a contract DTO `UserSummary(Guid Id, string Username)` or null. Ready for e.g. showing assignee names. Cost: exposes data nobody consumes yet, and once published, every field is a promise other modules can start depending on.

➡️ **My answer:** bool ExistsAsync(Guid) (Recommended)

---

### 2026-10-07 23:21

**Q (Ordering): In what order should GET /work-items?assigneeId= return items? (WorkItem has no timestamp today.)**

- By name, then id (Recommended) — Deterministic, no domain change, easy to test. Cost: not 'newest first', which many UIs want; ordering is done in the read store (in-memory today, pushed to SQL later).
- Add CreatedAt, newest first — Most realistic for a task list. Cost: new domain field + a TimeProvider abstraction for testable time, more scope than the plan (2–4h box).
- Unspecified order — Least code; document that order is undefined. Cost: flaky-looking API and tests must compare as sets.

➡️ **My answer:** By name, then id (Recommended)
