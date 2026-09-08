# CODE-TOUR.md — the pipeline, line by line, from zero

This assumes you have never written a script. It starts with what a script even
is and ends with you able to open any file in `scripts/` and follow it.

`PIPELINE.md` explains *what the system does*. This explains *how the code
does it*. Read them in that order if you have not already.

You do not need to read this in one sitting. Part 2 is the only part you must
read in order; after that you can jump to whichever file you are curious about.

---

# Part 0 — The vocabulary, once

**A script** is a text file of instructions. Nothing more. `classify.mjs` is a
file you could open in Notepad. It is not compiled or installed; a program
called **Node** reads it top to bottom and does what it says.

**Node** is that program. `node scripts/detect.mjs` means "Node, read this file
and do it". It is to JavaScript what the .NET runtime is to C#.

**`.mjs`** is just a file extension meaning "this JavaScript file uses the modern
`import` / `export` style". Nothing deeper.

**A module** is a file that hands other files some of its contents. A file says
`export function foo()` — "anyone may use my `foo`" — and another says
`import { foo } from './thing.mjs'` — "give me theirs". That is the whole idea.

**The two folders:**

- `scripts/*.mjs` — **commands.** Things you run. Each is one job: detect, lint,
  classify, teach, visualize, apply, summarise.
- `scripts/lib/*.mjs` — **libraries.** Things commands borrow from. No library
  ever runs on its own; they hold the shared knowledge (what a C# token is, what
  the complexity ladder is, how a header is shaped).

**Why the split matters.** Early on, `lint.mjs` needed one constant from another
command file. Importing from a command *ran that whole command*, model calls and
all. That is why `LINT_FORMAT` lives alone in `lib/lint-rules.mjs`. A library
must be safe to import; a command must not be imported.

---

# Part 1 — The shape of the whole thing

NeetCode pushes raw files called `submission-0.cs`, `submission-1.cs`. Six steps
turn them into a curated folder.

```
   submission-0.cs                     optimal.cs
   submission-1.cs      ────────►      suboptimal.cs
   (raw, from NeetCode)                <slug>-visualizer.html
```

| # | command | model | what it does |
|---|---|---|---|
| 1 | `detect.mjs` | none | which folders have raw submissions waiting |
| 2 | `lint.mjs` | Sonnet | tidy spacing and variable names |
| 3 | `classify.mjs` | Sonnet | work out complexity, rename files, write the header |
| 4 | `teach.mjs` | Opus | write the study block at the bottom of each file |
| 5 | `visualize.mjs` | Opus | build the animation |
| 6 | `apply.mjs` | none | regenerate `README.md`, `index.md`, `.agent/state.json` |

**Lint is first, and that is not arbitrary.** The header, the teaching block and
the visualizer all *name variables*. Rename a variable after they are written
and all three are describing code that no longer exists. So: names are settled
first, everything else describes the settled names.

**One sentence to keep in your head for the rest of this document:**

> The model reads. The script decides.

The AI is asked narrow questions — *what is this complexity*, *what should this
block say*. It is never asked *what should this file be called* or *should we
rewrite this*. Those are ordinary code, in files you can read, with tests.

---

# Part 2 — Enough JavaScript to read every file here

Only the constructs this codebase actually uses. Skim it, then refer back.

### Values and names

```js
const limit = 5;        // a name that never changes afterwards
let count = 0;          // a name you intend to change
count = count + 1;      // changing it
```
Use `const` unless you must reassign. This codebase almost always uses `const`.

### Text

```js
const name = 'optimal.cs';                  // plain text
const msg  = `wrote ${name} to disk`;       // backticks let you drop values in
```
Those are **backticks**, not quotes. `${...}` means "put the value here". C#'s
`$"wrote {name}"` is the same idea.

### Lists

```js
const files = ['optimal.cs', 'suboptimal.cs'];
files.length        // 2
files[0]            // 'optimal.cs'   - counting starts at 0
```

Four list operations do most of the work in this repo:

```js
files.filter(f => f.endsWith('.cs'))    // keep only the ones that pass the test
files.map(f => f.toUpperCase())         // turn each one into something else
files.some(f => f === 'optimal.cs')     // is at least one true?  -> true/false
files.sort((a, b) => a.localeCompare(b))// put them in order
```

`f => ...` is a **tiny throwaway function**: "given `f`, produce this". The C#
equivalent is `f => ...` too — LINQ's `Where`, `Select`, `Any`, `OrderBy`.

`sort` takes a comparison that returns a **negative number if `a` comes first**,
positive if `b` does, zero if neither. `a - b` sorts numbers ascending. You will
see `||` chaining several comparisons: *"order by this; if tied, by that"*.

### Records

```js
const rec = { time: 'O(n)', space: 'O(1)' };
rec.time                       // 'O(n)'
const { time, space } = rec;   // pull both out into their own names at once
const next = { ...rec, space: 'O(n)' };  // a copy, with space replaced
```

That last line — `...rec` — is called **spreading**, and it caused a real bug
here. `apply.mjs` used to rebuild a record from scratch instead of spreading,
which silently deleted the `headerSignatures` another step had just written.

### Sets and maps

```js
const seen = new Set(['a', 'b']);  seen.has('a');   // true — a bag of unique values
const map  = new Map();  map.set('a', 1);  map.get('a');   // a lookup table
```

### Choices

```js
if (x) { ... } else { ... }
const label = ok ? 'yes' : 'no';       // short if/else that produces a value
const n = maybe ?? 5;                  // use maybe, or 5 if it is null/absent
const v = rec?.lint?.version;          // dig in safely; null rather than a crash
```

`?.` matters everywhere in this codebase, because most state records are
partial: a folder may have no `lint` key at all, and asking for `.version` of
nothing would otherwise crash the run.

### Functions

```js
function add(a, b) { return a + b; }      // the long form
const add = (a, b) => a + b;              // the same thing, short form
export function add(a, b) { ... }         // ...and other files may use it
```

### Patterns (regular expressions)

```js
/^submission-(\d+)\.([A-Za-z0-9]+)$/
```

A pattern for matching text. Read it piece by piece:

| piece | meaning |
|---|---|
| `^` | start of the text |
| `submission-` | those exact characters |
| `(\d+)` | one or more digits, **captured** so we can read it back |
| `\.` | a literal dot (a bare `.` means "any character", so it is escaped) |
| `([A-Za-z0-9]+)` | one or more letters/digits, captured |
| `$` | end of the text |

So it matches `submission-12.cs` and captures `12` and `cs`. It does not match
`optimal.cs`. **That single line is how the pipeline tells NeetCode's files from
yours** — it is `SUBMISSION_RE` in `lib/scan.mjs`.

### Files and the outside world

```js
import { readFileSync, writeFileSync, existsSync } from 'node:fs';
const text = readFileSync('a.cs', 'utf8');   // read the whole file as text
writeFileSync('a.cs', text, 'utf8');         // write it back
```

`Sync` means "finish before moving on". Fine here — these scripts do one thing
at a time and simplicity is worth more than speed.

```js
import { execFileSync } from 'node:child_process';
execFileSync('git', ['mv', from, to], { cwd: dir });
```

Run another program. Note the arguments are a **list**, not one string. That is
deliberate and it is a safety property: a filename containing a space or a
quote is passed as one argument and can never be read as a command.

---

# Part 3 — The libraries

## `lib/scan.mjs` — what is in the repo, and what is waiting

The single source of truth for "what exists and what is pending". Everything
else asks this file rather than looking at the disk itself. Two implementations
of the same question is how a pipeline starts contradicting itself.

```js
export const SUBMISSION_RE = /^submission-(\d+)\.([A-Za-z0-9]+)$/;
```

**The most important line in the repo.** NeetCode writes `submission-<n>.<ext>`.
Anything else in a problem folder is ours. Every "is this raw or curated?"
decision reduces to this pattern.

```js
export function loadState() {
  if (!existsSync(STATE_PATH)) return { version: 1, problems: {} };
  try { ... } catch (e) {
    console.error(`scan: ${STATE_PATH} is unreadable (...) - treating as empty`);
    return { version: 1, problems: {} };
  }
}
```

Read `.agent/state.json`, the pipeline's memory. If it is missing or corrupt,
carry on with an empty memory rather than crashing. Worst case the pipeline
redoes work; it never refuses to run.

`saveState()` writes only if the **substance** changed, and deliberately ignores
the timestamp. Bumping `updatedAt` every run made the file differ every run,
which made the workflow commit every night forever — a repo full of commits
saying nothing happened.

```js
export function foldersChangedSince(sha) {
  out = execFileSync('git', ['diff', '--name-only', sha, 'HEAD'], ...);
  for (const line of out.split('\n')) {
    if (p && SUBMISSION_RE.test(basename(p))) slugs.add(basename(dirname(p)));
  }
}
```

"Which problem folders got a submission since this commit?" On a push run the
answer is held back — you may still be submitting to that problem. `basename` is
the file part of a path, `dirname` the folder part, so `dirname` of the changed
file is the slug.

`scanRepo()` walks every topic and slug and returns one record per problem. The
part worth understanding is how it decides a submission is a duplicate:

```js
const curatedPrints = {};
for (const f of curated) curatedPrints[solutionPrint(...)] = f;
const known = new Set([...Object.keys(curatedPrints), ...Object.keys(rec.fingerprints ?? {})]);
```

Fingerprint everything already curated here, plus everything ever curated here
(from state). A resubmission matching one of those is code you already have.

```js
const claimedBy = new Map();
for (const sub of [...pending].sort((a, b) => b.index - a.index)) {
  if (sub.duplicateOfCurated) continue;
  if (claimedBy.has(sub.print)) sub.supersededBy = claimedBy.get(sub.print);
  else claimedBy.set(sub.print, sub.file);
}
```

Walk the pending submissions **newest first** (`b.index - a.index` is descending)
and let the newest claim each shape. An older submission whose shape is already
claimed is superseded and will be deleted. This is why, if you submit twice, the
newest wins — and why your notes should be in the submission you send last.

## `lib/normalise.mjs` — what makes two files "the same"

```js
export function normalise(src) { return stripComments(src).replace(/\s+/g, ''); }
export function fingerprint(src) { return sha256(normalise(src)); }
```

Strip comments, strip all whitespace, hash what is left. Two files differing
only in formatting or commentary produce the same fingerprint.

```js
export function solutionPrint(src, ext = 'cs') {
  if (ext !== 'cs') return fingerprint(src);
  try { return sha256('shape1|' + shapeForm(src)); } catch { return fingerprint(src); }
}
```

The identity of a **solution**, not a file. It uses `shapeForm` (next section),
which also ignores variable *names*. That matters because **lint renames
variables**: without it, resubmitting code lint had already tidied would read as
a brand new solution. The `catch` falls back to the plain text hash — strictly
safer, because it can miss a duplicate but never invent one.

## `lib/csharp.mjs` — the guard that stands between the model and your code

The most important file here. 263 lines, four exported functions.

### `tokenize(src)` — chop C# into pieces

```js
if (/\s/.test(c)) { ... out.push({ t: 'ws', v: ... }); }
if (c === '/' && src[i + 1] === '/') { ... out.push({ t: 'comment', ... }); }
if (/[A-Za-z_$]/.test(c)) {
  const v = src.slice(i, j);
  out.push({ t: KEYWORDS.has(v) ? 'kw' : 'id', v });
}
out.push({ t: 'op', v: c });
```

Walk the text one character at a time and emit labelled pieces: `ws` whitespace,
`comment`, `str`, `char`, `num`, `kw` keyword, `id` identifier, `op` operator.
`int left = 0;` becomes `kw(int) ws id(left) ws op(=) ws num(0) op(;)`.

Note `KEYWORDS.has(v) ? 'kw' : 'id'` — a name is a keyword only if it is in the
list. That list once wrongly included C#'s *contextual* keywords (`value`,
`get`, `where`, `await`…), which are legal identifiers, and a perfectly good
rename was refused for four runs as `id -> kw`.

### `publicSignatureNames(toks)` — what NeetCode wrote

```js
if (!(toks[i].t === 'kw' && toks[i].v === 'public')) continue;
```
Find each `public`. Then walk forward to its `(`, abandoning if you hit `{`, `;`
or `=` first — that is a field or a property, not a method.

```js
if (toks[open - 1]?.t === 'id') fixed.add(toks[open - 1].v);   // the method name
```
The identifier just before `(` is the method's name.

```js
if (next && next.t === 'op' && (next.v === ',' || next.v === ')')) fixed.add(t.v);
```
Inside the parameter list, a parameter *name* is the identifier that closes its
slot: `int[] nums,` or `TreeNode root)`. Depth is tracked so a generic argument
inside the list cannot be mistaken for one.

For `public bool IsValidBST(TreeNode root)` that returns `{IsValidBST, root}`.
Those names are pinned **everywhere in the file**, not just at the signature —
pinning only the declaration would let a rewrite rename `root` in the body and
leave the parameter behind, which is a broken file.

### `fixedIdentifier(toks, i, boilerplate)` — names nobody may rename

```js
if (boilerplate.has(toks[i].v)) return 'boilerplate';   // NeetCode's stub
if (prev.t === 'op' && prev.v === '.') return 'member';  // x.Length
if (prev.t === 'kw' && prev.v === 'class') return 'type';// class Solution
if (prev.t === 'kw' && prev.v === 'new') return 'type';  // new Queue<int>()
if (next.t === 'id') return 'type';                      // TreeNode node
```

The last two are worth more than they look. Without them,
`List<int> seen` → `HashSet<int> seen` is a perfectly consistent one-to-one
rename of every token — and an O(n) lookup silently becoming O(1).

### `sameShape(before, after)` — the proof

```js
if (A.length !== B.length) return { ok: false, ... };
```
Different number of meaningful tokens? Something other than names and spacing
was edited. Refuse immediately.

```js
if (a.t !== b.t) errors.push(`token ${i} changed kind ...`);
if (a.t !== 'id') { if (a.v !== b.v) errors.push(...); continue; }
```
Walk both files in step. Kinds must match. Anything that is not an identifier —
keywords, operators, numbers, strings — must be **identical**. A flipped `<=`,
a changed literal, a dropped statement: all caught here.

```js
if (fwd.has(a.v) && fwd.get(a.v) !== b.v) errors.push(`${a.v} renamed inconsistently`);
if (rev.has(b.v) && rev.get(b.v) !== a.v) errors.push(`two different names both became ${b.v}`);
fwd.set(a.v, b.v); rev.set(b.v, a.v);
```
Two lookup tables, in both directions. `fwd` catches one name becoming two;
`rev` catches two names collapsing into one. That is what makes it a *one-to-one*
renaming rather than "some names changed".

```js
if (commentsAfter < commentsBefore) errors.push(`comment(s) deleted`);
```
Your comments are yours. They may be reworded — a rename can make a comment name
a variable that no longer exists — but losing one fails the whole file.

### `shapeForm(src)` — the same solution, however it is written

```js
if (fixedIdentifier(toks, i, boilerplate)) { out.push(t.v); continue; }
if (!seen.has(t.v)) seen.set(t.v, `$${seen.size}`);
out.push(seen.get(t.v));
```

Replace every renameable identifier with the order it first appears. `l/r/m` and
`left/right/mid` both become `$0/$1/$2`, so they hash identically. Pinned names
stay literal, which is what keeps a queue and a stack apart.

## `lib/complexity.mjs` — which file gets to be `optimal.cs`

```js
export const COMPLEXITY = ['O(1)', 'O(log n)', ..., 'O(n!)', 'other'];
export const rank = (c) => { const i = COMPLEXITY.indexOf(c); return i === -1 ? COMPLEXITY.length : i; };
```

A ladder, best first. `rank` is just the position. **This is the safety device
for classification**: the model must pick a rung from this exact list, and the
*script* does the ranking. The model never decides which solution is better.
A complexity the ladder has no rung for is refused, not guessed — and the run
prints what it was so a rung can be added.

```js
const cmp = (a, b) => rank(a.time) - rank(b.time) || rank(a.space) - rank(b.space);
const bestTier = sorted.filter((s) => rank(s.time) === bt && rank(s.space) === bs);
```

Order by time, and on a tie by space. The **best tier** is everything equal on
*both* — genuinely interchangeable. Ranking on time alone would have promoted an
O(n)/O(n) solution over an O(n)/O(1) one.

```js
const tierOrder = [...bestTier].sort((a, b) =>
  (a.selfMarked ? 0 : 1) - (b.selfMarked ? 0 : 1) ||
  slot(a.file) - slot(b.file) ||
  a.file.localeCompare(b.file, undefined, { numeric: true }));
```

Read the `||` chain as *"first by this; if tied, by that"*:

1. **A solution you wrote wins.** Equal on both axes means nothing separates them
   but authorship, and this is your study log.
2. **Then the slot a file already holds**, so a curated folder keeps its names.
   This replaced a plain filename sort, because numeric collation puts
   `optimal-variant-2.cs` *before* `optimal-variant.cs` — which renamed those two
   past each other on every single run, forever.
3. **Then filename.** Arbitrary, but stable.

## `lib/header.mjs` — the block above the code

```js
export function headerSignature(name, sol, selfMark, ranked) {
  return JSON.stringify({ v: HEADER_FORMAT, name, codePrint: sol.codePrint ?? null,
    time, space, algorithm, approachKey, correct, selfMark, standing: standing(name, ranked) });
}
```

**The anti-churn mechanism, and the pattern to copy.** A header is rewritten only
when this string changes. It contains every *fact* the header asserts and none
of the prose — so the nightly run does not reword your headers forever.

`codePrint` is in there for a specific reason: without it, a lint rename leaves
the header describing variables that no longer exist, because none of the facts
changed and nothing would be rewritten.

```js
export function standing(name, ranked) {
  if (me === 0) return tied(ranked[0], next) ? `ties with ...` : `ranks above ...`;
```

The one header line that makes a claim about *another file*. It used to say
"ranks above" for whatever sat at index 0 without checking for a tie — so two
headers in one folder flatly contradicted each other.

## `lib/visualizer.mjs` — building and checking the animation

`validate()` actually **runs** the generated `PROBLEM` object and checks it: the
steps exist, every line number is inside the file, each panel has the shape its
type requires, the blurb is under 450 characters. A model that produces something
plausible but broken is caught here rather than by you clicking Play.

`selectForVisualizer()` drops the brute force when a real solution exists — you
asked for that explicitly.

`renameInVisualizer()` follows a rename into the badge each panel shows. One pass
with the old names longest-first, so a straight swap between two names cannot be
applied twice and a three-way rotation still resolves.

## `lib/report.mjs` — how the summary table gets its data

```js
export function report(step, slug, status, detail) {
  if (!FILE) return;
  try { appendFileSync(FILE, [step, slug, status, detail].join('\t') + '\n'); }
  catch { /* reporting must never break the run */ }
}
```

Each step appends one line per folder to a temp file; `summarise.mjs` renders
them as the table. Note the empty `catch`: a reporting failure must never take
down a run that was otherwise fine.

---

# Part 4 — The commands

Every command shares the same skeleton. Learn it once and all six read the same:

```js
const argv = process.argv.slice(2);                       // 1. the flags you typed
const arg  = (n, d = null) => (argv.includes(n) ? argv[argv.indexOf(n) + 1] : d);
const has  = (n) => argv.includes(n);

const doApply = has('--apply');                           // 2. what they mean
const only    = arg('--slug')?.split(',').map(s => s.trim());

const state = loadState();                                // 3. read the world
let targets = scanRepo(state);
if (only) targets = targets.filter(p => only.includes(p.slug));

for (const p of targets) { ... }                          // 4. do the work

if (doApply) saveState(state);                            // 5. remember it
process.exit(failures ? 1 : 0);                           // 6. 0 = fine, 1 = something failed
```

`process.argv` is the words you typed. `slice(2)` drops `node` and the filename.
`process.exit(1)` is how a step tells GitHub Actions it failed.

**`--apply` is the safety catch.** Without it every command computes exactly what
it would do and prints it, touching nothing. That is why you can run any of them
without fear.

### How a model call is made — `lint.mjs`, and the same shape in all four

```js
const args = ['-p', INSTRUCTIONS,
              '--output-format', 'json',
              '--json-schema', JSON.stringify(SCHEMA),
              '--permission-mode', 'dontAsk',
              '--max-turns', '12',
              '--model', model];
raw = execFileSync('claude', args, { input: code, encoding: 'utf8', maxBuffer: 32 * 1024 * 1024 });
```

| flag | why |
|---|---|
| `-p` | one prompt, no conversation |
| `--json-schema` | the answer must fit a shape we defined. This is what makes the reply *parseable* rather than prose |
| `--permission-mode dontAsk` | nobody is sitting at the terminal |
| `--max-turns` | a runaway call cannot burn your subscription |
| `input: code` | the file goes in on standard input, never on the command line |

`--bare` is deliberately absent: bare mode does not read
`CLAUDE_CODE_OAUTH_TOKEN`, so it and your subscription are mutually exclusive.

```js
catch (e) {
  let env = null; try { env = JSON.parse(String(e.stdout ?? '')); } catch {}
  throw new Error(`claude failed (exit ${e.status}): ${env ? ... : String(e.stderr).slice(0, 200)}`);
}
```

Early on this printed the *command* rather than the error, and a whole evening
went into a failure whose actual message was `Not logged in`. Now it digs the
real reason out of the response.

### `lint.mjs` — the retry loop

```js
for (let attempt = 1; attempt <= 2 && !result; attempt++) {
  const r = ask(body, feedback);
  const check = sameShape(body, r.code);
  if (check.ok) result = { ...r, renames: check.renames };
  else feedback = [...check.errors, 'Every distinct variable must keep a distinct name...'];
}
```

Two attempts. If `sameShape` refuses, the **exact refusal reasons are handed
back to the model** as feedback for the second try. Refused twice, the file is
left untouched and recorded as failed so the next run does not pay for it again
— `--force` is the way back.

### `classify.mjs` — the rename, done safely

```js
const plan = assignNames(res.solutions.map(s => ({ ...s, selfMarked: !!marks.get(s.file) })));
```
The model's classifications go in; `assignNames` — ordinary tested code — decides
the filenames. Note `selfMarked` is attached here: the model is never asked whose
solution it is. That comes from `provenance`, recorded once from the raw
submission's marker and never re-derived, because the marker does not survive
curation.

The rename itself goes **through temporary names**. If `optimal.cs` must become
`optimal-variant.cs` and vice versa, renaming them one at a time would have the
first overwrite the second. Two phases — everything to a temp name, then temp
names to their finals — makes a swap safe.

Then everything keyed by filename is re-keyed onto the new names: `provenance`,
`classification`, `lint`, `visualizer.prints`, and the filenames written inside
the visualizer HTML. Each of those was a separate bug when it was missing.

### `teach.mjs` and `visualize.mjs`

Same skeleton, Opus instead of Sonnet, and the same signature trick: a block is
rewritten only when `teachSignature` changes.

`splitTrailingTeach()` finds the existing block at the bottom of a file. It
requires a full-width `=` rule before it will claim a `/* ... */` block belongs
to the pipeline — otherwise a long comment you wrote could be eaten.

`visualize` builds only when there is no visualizer, or when the recorded code
prints no longer match the file. A visualizer with **no** record is one of your
23 hand-built ones and is never touched without `--backfill`.

### `apply.mjs` — the free step

No model. Regenerates the README table and `index.md` from state, prunes records
for folders that no longer exist, and reports whether anything changed. Run it
any time; it costs nothing.

---

# Part 5 — `.github/workflows/pipeline.yml`

A **workflow** is a YAML file telling GitHub what to run and when. YAML is
indentation-based: nesting is meaning, and a stray space is a syntax error.

```yaml
on:
  push:
    paths: ['**/submission-*']       # only when a submission file changes
  schedule:
    - cron: '37 1 * * *'             # 07:07 IST
  workflow_dispatch:                 # the Run workflow button
```

`cron` is `minute hour day month weekday`, **in UTC**. IST is UTC+5:30, so
subtract 5h30m from the time you want. The odd minutes are deliberate: GitHub
queues scheduled workflows at low priority and `:00`/`:15`/`:30`/`:45` are where
everyone else's pile up.

```yaml
concurrency:
  group: neetcode-pipeline
  cancel-in-progress: false
```
Only one run at a time; the rest queue rather than being cancelled.

```yaml
if: github.event_name != 'push' || !startsWith(github.event.head_commit.message, 'chore(pipeline)')
```
**Loop guard 1 of 2**: ignore the pipeline's own commits. Guard 2 is that those
commits are pushed with the built-in `GITHUB_TOKEN`, whose pushes do not start
workflow runs. Swapping in a personal access token would defeat guard 2 — do not.

```yaml
- uses: actions/checkout@v7
  with:
    ref: main
    fetch-depth: 0
```
`ref: main` is the fix for the most expensive bug found. The default is
`github.sha`, the commit that *triggered* the run — which on a queued run is
*behind* main, so the run could not see what the run ahead of it had just
finished and would redo the same folder, then conflict with it.
`fetch-depth: 0` gets the full history, which `detect` needs to diff.

```yaml
continue-on-error: true
```
On each of the four model steps. One bad folder must not discard work that
already succeeded — which also means **a run can be green with work skipped**.
Read the summary, not the badge.

```bash
pushed=0
for attempt in 1 2 3; do
  if git push; then pushed=1; break; fi
  git fetch origin main
  if ! git rebase origin/main; then git rebase --abort || true; exit 1; fi
done
```
A run takes minutes; a submission landing in that window used to make the push
non-fast-forward and kill every model call it had made. Rebase onto whatever
arrived and try again. A conflict is reported, never forced.

---

# Part 6 — Reading a run yourself

**Start at the summary, not the badge.** The job summary is the table produced by
`summarise.mjs`: one row per folder, one column per step, plus "Needs attention".

| icon | meaning |
|---|---|
| ✅ | done |
| ⏭️ | looked at, nothing needed |
| ⚠️ | refused — needs a decision from you |
| ❌ | failed |
| · | not run |

Then open the *step* that interests you and read what it actually printed. The
useful lines:

```
excluded valid-binary-search-tree                    <- held back; you may still be submitting
pending  2 folder(s) -> 1 selected, 1 held back
  submission-0.cs  spacing only  ·  $0.0508          <- lint changed formatting only
  submission-1.cs  renames: res->result, q->queue    <- and these names
    submission-0.cs -> optimal.cs                    <- classify's naming decision
  optimal.cs  up to date                             <- signature unchanged, nothing rewritten
```

**Run any of it yourself, safely.** Nothing changes without `--apply`:

```powershell
node scripts/detect.mjs                              # free, no AI
node scripts/apply.mjs                               # free, regenerates the tables
node scripts/lint.mjs --slug two-integer-sum         # dry run, shows the diff
for t in scripts/lib/*.test.mjs; do node "$t"; done  # 58 tests, free
```

`git checkout -- .` throws away anything a dry run's `--apply` twin did locally.

---

# Part 7 — If you want to change something

1. **Find the decision.** Filenames → `lib/complexity.mjs`. What lint may rewrite
   → `lib/csharp.mjs`. When a header is rewritten → `headerSignature`. What is
   pending → `lib/scan.mjs`.
2. **Write the test first**, in `lib/<name>.test.mjs`. The workflow runs every
   `scripts/lib/*.test.mjs` before the first model call, so a new file is picked
   up automatically.
3. **Prove it against something real** — a folder from the repo, or a throwaway
   git fixture. Nearly every bug in this pipeline looked correct in the code and
   only showed up when run.
4. **Dry run, then `--apply`, then read the diff.**

The rule that has held throughout: if you add a model call, add the mechanical
gate that checks it. The model reads; the script decides.
