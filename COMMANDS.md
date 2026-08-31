# Pipeline Commands

Quick reference. Run everything from the repo root:

```powershell
cd "C:\Users\debar\Source\Repos\neetcode-submissions"
```

**Golden rule:** every script is a dry run unless you pass `--apply`. Nothing is committed
until you commit it, so `git checkout -- .` undoes any local run.

---

## The workflow (GitHub Actions)

**Actions → NeetCode pipeline → Run workflow**

| Input | Default | What it does |
|---|---|---|
| `exclude` | *empty* | Comma-separated slugs to hold back: `two-integer-sum,minimum-stack` |
| `backfill` | *empty* | Any value = also process folders with no new submissions. Empty = off |
| `limit` | `5` | Max folders this run |

Automatic triggers — nothing to type:

| Trigger | Behaviour |
|---|---|
| Push a `submission-*` file | Processes every pending folder **except** the one just pushed to |
| 11:00 IST daily | Processes everything pending, including that one |

Backfill is manual-only. Push and schedule events carry no inputs, so it cannot be set from them.

---

## Local commands

### See what's pending — free, no model call

```powershell
node scripts/detect.mjs
node scripts/detect.mjs --json
node scripts/detect.mjs --exclude two-integer-sum
node scripts/detect.mjs --exclude-changed-since <sha>
node scripts/detect.mjs --help
```

### Classify + rename + banner header — Sonnet

```powershell
node scripts/classify.mjs --slug two-integer-sum                    # dry run
node scripts/classify.mjs --slug two-integer-sum --apply            # do it
node scripts/classify.mjs --apply --limit 3                         # pending folders only
node scripts/classify.mjs --apply --backfill --limit 3              # include old folders
node scripts/classify.mjs --slug two-integer-sum --verbose          # + the reasoning
node scripts/classify.mjs --slug two-integer-sum --model opus       # override the model
node scripts/classify.mjs --apply --exclude minimum-stack
node scripts/classify.mjs --apply --exclude-changed-since <sha>
node scripts/classify.mjs --apply --delete-duplicates               # remove identical resubmissions
```

`--slug` works on any folder, pending or not. Without it, only folders with unprocessed
submissions are touched (unless `--backfill`).

### Teaching block — Opus

```powershell
node scripts/teach.mjs --slug two-integer-sum                       # dry run, prints the block
node scripts/teach.mjs --slug two-integer-sum --apply
node scripts/teach.mjs --apply --limit 3
node scripts/teach.mjs --backfill --slug two-integer-sum --apply    # rewrite even if up to date
node scripts/teach.mjs --slug two-integer-sum --apply --model sonnet
```

Needs a classification on record first — run `classify --apply` on that folder if it says
`no classification on record`.

### Visualizer — Opus

```powershell
node scripts/visualize.mjs --slug two-integer-sum                   # dry run
node scripts/visualize.mjs --slug two-integer-sum --apply
node scripts/visualize.mjs --apply --limit 3
node scripts/visualize.mjs --slug two-integer-sum --apply --model sonnet
```

**Refuses to overwrite an existing visualizer.** To regenerate one, delete it first:

```powershell
Remove-Item "Data Structures & Algorithms\two-integer-sum\two-integer-sum-visualizer.html"
node scripts/visualize.mjs --slug two-integer-sum --apply
```

### README index + state — free, no model call

```powershell
node scripts/apply.mjs
node scripts/apply.mjs --dry-run
```

---

## Debugging

```powershell
node scripts/probe-cli.mjs
```

Adds one CLI flag at a time on top of a bare call. **The first `FAIL` is the culprit** —
ignore everything after it. Also prints your `claude --version`.

```powershell
type .agent\tmp\last-claude-response.json
```

The full unabridged response from the last failed model call.

```powershell
echo "API key set? [$env:ANTHROPIC_API_KEY]"
```

Must print `[]`. Anything else means you're being billed per token instead of using your
subscription.

```powershell
claude
```

Opens the interactive CLI. The header must say `Claude Pro`. If it says `Not logged in`,
finish the setup wizard — theme first, login after.

---

## Undo

```powershell
git checkout -- .                                        # discard everything uncommitted
git checkout -- "Data Structures & Algorithms/<slug>"    # discard one folder
git checkout <sha> -- <path>                             # restore a file from history
```

The pipeline's own commits are prefixed `chore(pipeline)`, so they're easy to find and revert:

```powershell
git log --oneline --grep "chore(pipeline)"
git revert <sha>
```

---

## Routine sequences

**Bring one old folder fully up to standard:**

```powershell
node scripts/classify.mjs --slug <slug> --apply
node scripts/teach.mjs --backfill --slug <slug> --apply
node scripts/apply.mjs
git add -A
git commit -m "chore: bring <slug> up to standard"
git push
```

**After the pipeline commits (it pushes, so you're behind):**

```powershell
git pull
```

**If `git pull` refuses because of local changes:**

```powershell
git stash
git pull
git stash pop
```

---

## One-off tools

```powershell
node scripts/migrate-provenance.mjs                      # report only
node scripts/migrate-provenance.mjs --write              # record it
node scripts/migrate-provenance.mjs --ref <sha> --write  # read from a different commit
```

Recovers who-wrote-what from annotations predating the pipeline. Already run — you only need
this again if `state.json` is lost.
