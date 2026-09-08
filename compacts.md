# compacts.md — handoff notes for the next session

You are picking up the NeetCode pipeline. This file is the context a fresh chat
needs before changing anything. Read it first, then `PIPELINE.md` (what the
system is), `CODE-TOUR.md` (what every file does, line by line), and
`COMMANDS.md` (how Debarghya drives it).

Debarghya is not a scripts person. Explain in plain language, show the evidence,
and never say something is done that has not been run.

---

## 1. The one idea the whole thing is built on

**The model reads; the script decides.**

Every AI call answers a narrow question — what is this code's complexity, what
should this teaching block say, what should the animation show. Every *decision*
— what the file is called, whether to rewrite it, whether to publish — is
deterministic JavaScript with tests.

Every model output passes a mechanical gate before it is allowed near the repo:

| step | gate | lives in |
|---|---|---|
| lint | `sameShape()` — token-by-token proof the rewrite changed only names and spacing | `scripts/lib/csharp.mjs` |
| classify | the complexity ladder — the model picks from a fixed enum, the script ranks | `scripts/lib/complexity.mjs` |
| visualize | `validate()` — runs the generated object, checks steps, line numbers, panel shapes | `scripts/lib/visualizer.mjs` |

If you add a model call, add its gate. That is the house style, and it is the
reason this thing can run unattended.

---

## 2. Hard invariants — do not change these

These are not preferences. Breaking one costs money or safety.

1. **Never set `ANTHROPIC_API_KEY`** anywhere in the repo, the workflow, or the
   secrets. It outranks `CLAUDE_CODE_OAUTH_TOKEN` in non-interactive mode and
   silently moves billing from the Claude subscription to per-token. The
   "Guard" step refuses to run the model if it is set — leave that guard in.
2. **Never add `--bare` to a `claude -p` call.** Bare mode does not read
   `CLAUDE_CODE_OAUTH_TOKEN`, so it and the subscription are mutually exclusive.
   The `.claude` / `.mcp.json` guard exists to recover the safety half of what
   `--bare` would have given.
3. **The pipeline pushes with the built-in `GITHUB_TOKEN`, never a PAT.**
   Pushes made with `GITHUB_TOKEN` do not start new workflow runs. That is loop
   guard 2 of 2. A PAT would defeat it and the pipeline would trigger itself
   forever.
4. **Loop guard 1** is the `if:` on the job: skip a push run whose head commit
   starts with `chore(pipeline)`. The commit step writes exactly that prefix.
   Change one and you must change the other.
5. **Never `git add -A` at the repo root.** It once swept Debarghya's unreviewed
   work into a pipeline commit. Stage explicit paths only.
6. **Do not run destructive git on files with uncommitted work.** I lost my own
   in-progress edits once with a careless `git checkout --`.

---

## 3. What runs, in order

`.github/workflows/pipeline.yml`, one job, fourteen steps:

```
checkout (ref: main)  ->  node  ->  detect  ->  guard: no .claude/.mcp.json/API key
  ->  resolve named folders  ->  guard: run every scripts/lib/*.test.mjs
  ->  install claude  ->  choose backfill batch
  ->  LINT  ->  CLASSIFY  ->  TEACH  ->  VISUALIZE
  ->  apply  ->  commit and push (rebase + retry)  ->  summarise
```

Triggers: push on `**/submission-*`; cron `37 1 * * *` and `39 13 * * *`
(07:07 and 19:09 IST — cron is UTC, IST is UTC+5:30); manual.

Manual inputs are three boxes: `Process-Specific-folder`, `Back-Fill`
(checkbox), `Back-Fill-Limit`.

**Order matters and is load-bearing.** Lint runs *first* because the header, the
teaching block and the visualizer all name variables — rename after they are
written and every one of them describes code that no longer exists.

The four model steps are `continue-on-error: true` on purpose: one bad folder
must not discard work that already succeeded. This means **a run can be green
with work skipped** — read the job summary, not the badge.

---

## 4. `.agent/state.json` — the memory

One record per problem slug. Everything that stops the pipeline redoing work
lives here.

| key | what it is | who writes it |
|---|---|---|
| `curatedFiles`, `hasVisualizer`, `topic` | what the folder holds | apply |
| `processedSubmissions` | raw files already dealt with (incl. deleted duplicates) | classify |
| `fingerprints` / `knownFingerprints` | shape print -> curated file. A resubmission of code you already have is recognised here | apply |
| `lint` | per file: `{version, codePrint, renames}` or `{version, failed, reason}` | lint |
| `provenance` | per file: `{selfMarked, evidence, recordedFrom}`. Recorded once from the raw submission's `// my solution` marker and **never re-derived** — the marker does not survive curation | classify |
| `classification` | per file: time, space, algorithm, approachKey, correct, bruteForce | classify |
| `headerSignatures` / `teachSignatures` | a hash of the facts a header/block asserts. Unchanged signature = leave the file alone | classify / teach |
| `visualizer` | `{files, prints, builtAt}` — the code fingerprints the animation was built from | visualize |

**Signatures are the anti-churn mechanism.** Without them the nightly run
reworded every header forever and every diff was noise. If you add a fact to a
header, add it to the signature; if you add prose, do not.

`saveState()` deliberately ignores `updatedAt` when deciding whether the file
changed — bumping a timestamp every run made the pipeline commit every night
saying nothing happened.

---

## 5. Every bug found so far, and its lesson

Read this before "improving" anything. Most of these looked like sensible code.

**State and staging**
- `apply.mjs` rebuilt problem records wholesale and erased `headerSignatures`
  written one step earlier. → Always spread `...rec`.
- `index.md` was regenerated but never staged, so Pages never updated. → The
  commit step stages every path the pipeline can write, explicitly.
- The commit step was gated on `apply.outputs.changed`, which only covers the
  three files apply itself writes. A run that linted, taught and visualised
  could be discarded entirely. → Ungated; the staged-changes check is the guard.

**Handoffs between steps**
- classify renames raw submissions away, so teach could not rediscover what had
  just been processed. → classify emits `applied=<slugs>`; teach and visualize
  consume it.
- `classify` and `visualize` did not split `--slug` on commas, so a two-folder
  batch silently processed nothing and exited 0.

**Money**
- Backfill had no notion of "done" and redid the first N folders forever. →
  `atCurrentStandard` and `select-backfill.mjs`.
- A queued run checked out `github.sha` — its own trigger commit, which is
  *behind* main — so it could not see the finished work of the run ahead of it
  and redid the same folder, then conflicted with it. → `ref: main`.
- A run's push was rejected because a submission landed during the six minutes
  it spent on the model, and every model call died with the runner. → rebase and
  retry, up to three times. A conflict is reported, never forced.
- `--force` on teach spent Opus rewriting blocks that were already correct.

**Judgement encoded wrongly**
- Tiering on time alone would have promoted an O(n)/O(n) solution over an
  O(n)/O(1) one. → Rank on (time, space).
- `standing()` said "ranks above" for whatever sat at index 0 without checking
  for a tie, so two headers in one folder contradicted each other.
- Numeric collation sorts `optimal-variant-2.cs` *before* `optimal-variant.cs`,
  so a three-way tie renamed those two past each other **on every run**. → Sort
  by the slot a file already holds.
- A tie now goes to the solution Debarghya wrote (`selfMarked`), which
  deliberately outranks the incumbent so folders named the other way correct
  themselves once.

**The guard itself**
- `sameShape` treated C# *contextual* keywords (`value`, `get`, `set`, `when`,
  `where`, `async`, `await`, `yield`, `nameof`) as reserved, so a legal rename
  was refused as "id -> kw". They are ordinary identifiers.
- `sameShape` would have accepted `List<int>` -> `HashSet<int>` as a consistent
  rename — an O(n) lookup silently becoming O(1). → `fixedIdentifier()` pins the
  type after `new` and the type in a `Type name` declaration.
- The public signature is NeetCode's stub. Renaming `IsValidBST` or its `root`
  breaks the grader. → `publicSignatureNames()` pins the method name and its
  parameters **by name across the whole file**; pinning only the declaration
  would let a rewrite rename the body and leave the parameter behind.
- Re-keying the visualizer's recorded prints on rename stopped a wasteful
  rebuild — and with it the only thing that kept the animation's badges honest.
  → `renameInVisualizer()` rewrites the filenames in the HTML directly.

**Testing**
- A sabotage test that edits nothing passes silently. `refuses()` now fails when
  `before === after`.

---

## 6. Working on this repo

**Verification discipline.** Nothing here was accepted because it looked right.
Test the mechanism against a real fixture — two clones and a real rejected push,
a real conflicting rebase, a real folder from the repo. When a log arrives, read
what the steps actually did rather than the run's badge.

**Tests.** `node scripts/lib/<name>.test.mjs`, or all of them:
`for t in scripts/lib/*.test.mjs; do node "$t"; done`. 58 cases across four
files. The workflow runs the glob before the first model call, so a new test
file needs no workflow change. Pure functions only — anything that needs git or
the network gets a throwaway fixture instead.

**Line endings.** `.gitattributes` stores `.cs`, `.html` and `.md` as CRLF in the
working tree. When editing a `.md` with a script, read and write with
`newline=''` or you will reflow the whole file into one diff.

**The device sandbox** (if you are working through a linked computer) cannot
delete files, so git leaves `.git/*.lock` behind and the next git command fails
with "index.lock: File exists". `rm -f .git/*.lock` after each git call, and ask
for delete permission when you need it. Git there also has no `user.email`, so
commit with `git -c user.name=... -c user.email=...`.

**Commit messages** state what was wrong, what the evidence was, and what it
cost. That is the format Debarghya has been reading; keep it.

---

## 7. Known limitations, deliberately accepted

- **`sameShape` knows nothing about scope.** Two variables in different methods
  that share a name must be renamed identically or not at all. Pinning the
  public signature removed the common case; the rest is accepted.
- **`shapeForm` can rarely collapse a real difference** — two files differing
  only by swapping two variables' roles in a non-commutative spot, with nothing
  earlier pinning which is which.
- **A helper you declare `public` is treated as boilerplate** and its parameters
  are pinned. Make helpers `private` if you want lint to tidy them.
- **Newest-wins deletes the older duplicate**, so a comment written on an
  earlier attempt and not the later one is lost. Debarghya chose this knowingly.
- **A rebase conflict during the push loses that run's work.** Reported, not
  forced. Rare now that runs start from the tip of main.

---

## 8. Things Debarghya has asked for and had

Chronological, so you do not re-litigate settled decisions:

- Teaching block goes **below** the code, header above it.
- Free-form sections (3–9), no redundant scaffolding.
- Visualizers only for suboptimal + optimal, never for brute force when a real
  solution exists; file named `<slug>-visualizer.html`.
- `index.md` is a flat searchable list — **no pattern grouping** (asked for,
  then removed); run links open in a new tab.
- Comments in the code are his notes to himself. Lint may reword one only when a
  rename made it name a variable that no longer exists. **Deleting one fails the
  whole file.** Nothing a comment says is a reason to skip a file.
- Cron at 07:07 and 19:09 IST, two independent passes twelve hours apart.
- Three manual inputs, labels are just the names — no explanatory sentences in
  the form.
