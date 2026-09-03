# Pipeline Commands

Everything runs from the repo root:

```powershell
cd "C:\Users\debar\Source\Repos\neetcode-submissions"
```

### Two rules that cover most confusion

**1. Nothing changes unless you type `--apply`.**
Without it, every command just prints what it *would* do. Safe to run any of these.

**2. Nothing is permanent until you commit.**
`git checkout -- .` throws away anything a command did locally.

---

## Normally: do nothing

Solve problems on NeetCode. That's it.

| When | What happens |
|---|---|
| You push a submission | Every *other* pending problem gets processed. The one you just pushed to is left alone, in case you're still working on it. |
| 11:00 IST, every day | Everything left over gets processed — including that one. |

You only need the commands below when you want to do something out of the ordinary.

---

# Common tasks

## "What's waiting to be processed?"

```powershell
node scripts/detect.mjs
```

Free — no AI call. Tells you which problems have raw `submission-N.cs` files that haven't
been dealt with yet. If it says `nothing to do`, the pipeline is idle.

---

## "Process one problem right now, don't wait for 11am"

Four commands, in this order. Replace `two-integer-sum` with your problem's folder name.

```powershell
node scripts/lint.mjs      --slug two-integer-sum --apply
node scripts/classify.mjs  --slug two-integer-sum --apply
node scripts/teach.mjs     --slug two-integer-sum --apply
node scripts/visualize.mjs --slug two-integer-sum --apply
```

1. **lint** — tidies spacing and renames cryptic variables (`t` → `target`). Runs first
   on purpose: everything after it describes your code by name, so the names have to be
   final before anything is written about them.
2. **classify** — reads the code, works out the complexity, renames files to
   `optimal.cs` / `suboptimal.cs`, writes the short header at the top.
3. **teach** — writes the long study block at the bottom.
4. **visualize** — builds the animation, but *only if this problem doesn't have one*.

Then check and save:

```powershell
git --no-pager diff
git add -A
git commit -m "chore: process two-integer-sum"
git push
```

**Order matters.** `teach` needs the complexity that `classify` records. If it says
`no classification on record`, you skipped a step.

### What lint is and isn't allowed to do

Only three things: whitespace, comments, and the names of local variables and parameters.
Every rewrite is compared to the original token by token before it is written, so a model
that "improves" a comparison, drops a line, or renames a method has its whole file thrown
away rather than saved. Your own comments are protected too — they can be reworded when a
rename makes one name a variable that no longer exists, but deleting one fails the file.

If a file is refused twice, lint records the failure and stops paying to retry it. Retry it
by hand once the cause is fixed:

```powershell
node scripts/lint.mjs --slug two-integer-sum --apply --force
```

The guard itself has a test suite. It runs in the workflow before any model call, and you
can run it locally any time — it costs nothing:

```powershell
node scripts/lib/csharp.test.mjs
```

---

## "Show me what it would do, without changing anything"

Drop `--apply` off any command.

```powershell
node scripts/classify.mjs --slug two-integer-sum
node scripts/teach.mjs --slug two-integer-sum
```

`teach` without `--apply` prints the whole study block to your terminal, so you can read it
before it touches the file. `--verbose` on `classify` adds the model's reasoning.

---

## "Just do these folders, nothing else"

**Actions → NeetCode pipeline → Run workflow**, and put the folder names in `only`:

| Field | Put this |
|---|---|
| `only` | `binary-search` — or `binary-search,eating-bananas` for several |
| `force` | leave empty (see below) |
| everything else | leave empty |

That is the whole run. It does not look at new submissions, it does not touch the backfill
queue, and it ignores `limit` — you named the list, so it does all of it.

The names are the **folder names** under `Data Structures & Algorithms`. A typo stops the
run immediately, before a single AI call, and tells you what you probably meant:

```
::error::no folder named "binry-search" - did you mean "binary-search"?
```

### `force`

Leave it empty almost always. Empty means "bring this folder up to standard", so anything
already correct is left alone — which is usually the whole point, since rewriting a correct
teaching block just reworders it and costs Opus.

Put anything in the box (`yes`) and the run instead means "do it again anyway":

- lint retries a file it had given up on after two rejections
- the teaching block is rewritten even though nothing about the code changed
- **the visualizer is rebuilt, including one you built by hand**

That last one is why `force` is a separate box. Without it, a named run will never
overwrite a hand-built animation.

---

## "Bring my old problems up to the current standard" (backfill)

**Use the workflow for this**, not your laptop — it's the slowest and most expensive job.

**Actions → NeetCode pipeline → Run workflow**

| Field | Put this |
|---|---|
| `backfill` | `yes` |
| `limit` | `1` the first time. Raise it once you've read the result. |
| `only`, `force`, `exclude` | leave empty |

`only` and `backfill` are opposites: naming folders wins, and backfill is skipped entirely.

### How backfill knows what's left

It tracks what's already done, so **each run advances through the repo** — it doesn't redo
the same folders. Every run prints its position:

```
backfill: 23 folder(s), 6 already at the current standard, 17 remaining
```

- Set `limit` **higher than what's remaining** and it just does what's left and stops. No
  error, no wasted AI calls.
- Run it repeatedly and the remaining count drops each time.
- When it reaches `0 remaining`, the step does nothing. You're finished.

"Already at the current standard" means that folder has a recorded complexity *and* a header
built by the current version of the code. If I change the header format later, those folders
correctly go back to "remaining".

### Backfilling locally instead

Same thing, on your machine:

```powershell
node scripts/classify.mjs --backfill --apply --limit 3
node scripts/teach.mjs --backfill --apply --limit 3
```

Start with `--limit 1`.

---

## "Rebuild a visualizer I don't like"

`visualize` refuses to overwrite an existing one — the 23 you built by hand are protected.
To replace one deliberately, delete it first:

```powershell
Remove-Item "Data Structures & Algorithms\two-integer-sum\two-integer-sum-visualizer.html"
node scripts/visualize.mjs --slug two-integer-sum --apply
```

Don't like the result? `git checkout -- .` brings the original straight back.

---

## "Redo a header or study block that's already correct"

Normally the pipeline skips these — if the facts haven't changed, it leaves your file alone
so it doesn't reword things every night. To override that:

```powershell
node scripts/teach.mjs --slug two-integer-sum --apply --force
```

`--force` means "rewrite it even though nothing changed".

---

## "Undo what just happened"

```powershell
git checkout -- .                                        # undo everything uncommitted
git checkout -- "Data Structures & Algorithms/<slug>"    # undo just one problem
```

Already committed and pushed? The pipeline's own commits all start with `chore(pipeline)`:

```powershell
git log --oneline --grep "chore(pipeline)"
git revert <the sha>
```

---

# When something breaks

### Step 1 — find out which flag or credential is at fault

```powershell
node scripts/probe-cli.mjs
```

Makes six tiny AI calls, each adding one option to the last. **The first `FAIL` is the
problem** — ignore everything below it. It also prints your Claude version.

### Step 2 — read the full error

```powershell
type .agent\tmp\last-claude-response.json
```

The complete, unedited response from the last failed call.

### Step 3 — check you're not being billed per token

```powershell
echo "API key set? [$env:ANTHROPIC_API_KEY]"
```

Must print `[]`. If anything's inside the brackets, that key is being used **instead of**
your Claude Pro subscription and you're paying per call. Clear it:

```powershell
$env:ANTHROPIC_API_KEY = ""
```

### Step 4 — check you're logged in

```powershell
claude
```

The header must say `Claude Pro`. If it says `Not logged in`, finish the setup wizard —
theme first, login second. Then `/exit`.

### Reading a failed workflow run

Open the failing **step**, not just the run's red/green badge. The AI steps are set to keep
going after an error, so one bad folder doesn't undo work that already succeeded — which
also means **a run can show green with work skipped**. The run summary lists each step's
outcome.

---

# Flag reference

| Flag | Works on | Meaning |
|---|---|---|
| `--apply` | lint, classify, teach, visualize | Actually change files. Without it, dry run. |
| `--slug <name>` | lint, classify, teach, visualize | Just this one problem folder. Comma-separated for several. |
| `--limit <n>` | lint, classify, teach, visualize | Process at most this many folders. |
| `--backfill` | lint, classify, teach | Include old folders, skipping ones already done. |
| `--force` | lint, teach | Rewrite even if nothing changed, and retry a file lint gave up on. |
| `--verbose` | classify | Show the model's reasoning. |
| `--model <name>` | lint, classify, teach, visualize | Override the model: `sonnet`, `opus`, `haiku`. |
| `--exclude <slugs>` | detect, lint, classify | Hold these back. Comma-separated. |
| `--dry-run` | apply | Report without writing. |
| `--json` | detect | Machine-readable output. |
| `--delete-duplicates` | classify | Remove a resubmission identical to code you already have. |

Defaults worth knowing: `lint` and `classify` use **Sonnet**, `teach` and `visualize` use **Opus**.
`--slug` works on any folder, whether or not it has new submissions.

---

# One-off tools

```powershell
node scripts/apply.mjs          # regenerate the README table (free, no AI)
```

```powershell
node scripts/migrate-provenance.mjs           # report only
node scripts/migrate-provenance.mjs --write   # record it
```

Recovers who-wrote-what from the notes you wrote before the pipeline existed. Already done —
you'd only need it again if `.agent/state.json` were lost.
