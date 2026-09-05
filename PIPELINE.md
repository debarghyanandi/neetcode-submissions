# The NeetCode Submissions Pipeline

**What this is:** an automated system that takes the raw code NeetCode dumps into this
repository and turns it into study material — properly named files, complexity headers,
teaching notes, and interactive visualizers — without me doing it by hand every time.

This document explains what was built, how each piece works, and *why* it was built that
way. It assumes you know how to use git and can read C#, but assumes nothing about AI,
GitHub Actions, or Node.js. Every term is explained the first time it appears.

---

## Part 1 — The problem this solves

### What NeetCode gives me

When I solve a problem on [neetcode.io](https://neetcode.io) and submit it, NeetCode's
GitHub Sync pushes a commit into this repository. The file looks like this:

```
Data Structures & Algorithms/minimum-size-subarray-sum/submission-0.cs
Data Structures & Algorithms/minimum-size-subarray-sum/submission-1.cs
```

That's it. A number, an extension, no context. If I solve the same problem three times
trying to improve it, I get `submission-0`, `submission-1`, `submission-2` — and nothing
tells me which one was the good one.

### What I actually want

```
Data Structures & Algorithms/minimum-size-subarray-sum/
    optimal.cs                                  ← the best solution
    suboptimal.cs                               ← the slower one, kept for contrast
    minimum-size-subarray-sum-visualizer.html   ← an animation of both
```

Each `.cs` file carrying a header that says at a glance what it is, and a longer teaching
block at the bottom explaining why the approach works.

### What I was doing before

Opening a chat with Claude, pasting the files in, asking it to rename them, write the
headers, build a visualizer, and commit. Every single time. It worked, but it was manual,
inconsistent between sessions, and I had to remember to do it.

### What this pipeline does

The same thing, automatically, triggered by my own submissions.

---

## Part 2 — The vocabulary, explained

If you already know these, skip to Part 3.

**GitHub Actions** — GitHub can run programs for you when things happen in your repo. You
describe what to run in a YAML file inside `.github/workflows/`. GitHub reads it, spins up
a fresh temporary Linux computer (a "runner"), does what the file says, then throws the
computer away. It's free for public repos within generous limits.

**A workflow trigger** — the event that starts a run. Ours has three: someone pushes code,
a scheduled time arrives, or I click a button.

**cron** — the standard way to write a repeating schedule. `37 1 * * *` means
"minute 37, hour 1, every day of month, every month, every day of week" — i.e. 01:37, daily.
**Cron in GitHub Actions is always in UTC**, so subtract 5h30m from the IST time you want:
01:37 UTC is 07:07 in India (UTC+5:30).

**Node.js / `.mjs` files** — Node is a program that runs JavaScript outside a browser.
The pipeline's scripts are JavaScript. The `.mjs` extension just tells Node "this file uses
modern `import` syntax". You run one with `node scripts/detect.mjs`.

**The Claude Code CLI** — Claude, as a command-line program. You type
`claude -p "your question"` and it prints an answer. The `-p` means "print mode": answer
once and exit, rather than opening an interactive chat. This is how a *script* talks to a
model.

**A JSON Schema** — a machine-readable description of a shape. For example: *"an object
with a field called `time` whose value must be one of exactly these eleven strings."* You
hand it to the Claude CLI with `--json-schema`, and the answer comes back conforming to it.
This matters enormously, and Part 5 explains why.

**Idempotent** — a fancy word for "running it twice does nothing extra". If you run the
pipeline again on an unchanged repo, it must make zero changes. Almost every bug we found
was a failure of this property.

**A commit / a diff** — a saved snapshot of changes, and the list of what changed. When the
pipeline "commits", it saves its own work into the repo's history, exactly as you would.

---

## Part 3 — The shape of the whole thing

Here is the entire pipeline, in order:

```
   ┌─────────────────────────────────────────────────────────────┐
   │  TRIGGER                                                    │
   │  • I push a submission     • 07:07 / 19:09 IST  • I click Run │
   └───────────────────────────┬─────────────────────────────────┘
                               ▼
   1. DETECT      Which folders have submission-N.cs files that
                  haven't been processed?              (no AI, instant)
                               ▼
   2. GUARD       Refuse to run if the repo contains anything that
                  could hijack the model.               (no AI, instant)
                               ▼
   3. CLASSIFY    Read each solution. What's its time and space
                  complexity? Is it brute force?        (AI — Sonnet)
                               ▼
   4. RANK        Given those complexities, decide the filenames:
                  optimal.cs / optimal-variant.cs /
                  suboptimal.cs                        (no AI — pure rules)
                               ▼
   5. RENAME      git mv the files into their new names, write the
                  short banner header.                  (no AI)
                               ▼
   6. TEACH       Write the long study block below the code.
                                                        (AI — Opus)
                               ▼
   7. VISUALIZE   Build an interactive animation, but only for a
                  problem that doesn't already have one. (AI — Opus)
                               ▼
   8. INDEX       Regenerate the table in README.md.     (no AI)
                               ▼
   9. COMMIT      Save everything and push.              (no AI)
```

**The most important thing about that diagram** is how little of it is AI. Six of the nine
steps are ordinary code. Only three call a model, and each of those asks a narrow question.

---

## Part 4 — The single most important design decision

> **The model reads. The script decides.**

This is worth understanding properly, because it's the difference between a pipeline you
can trust and one you have to babysit.

### The naive version (what I did *not* build)

Hand the whole folder to an AI agent and say *"rename these files sensibly and write good
headers."* This works, sort of. But:

- You cannot predict what it will do.
- You cannot test it, because the answer is different every time.
- When it does something odd, there's nothing to inspect — no rule was broken, because
  there were no rules.

### The version that got built

Split the job at the point where judgement ends and arithmetic begins.

**The model's job — reading.** Look at this C# code. What's its worst-case time complexity?
Pick **one value from this fixed list**:

```
O(1)  O(log n)  O(sqrt n)  O(n)  O(n log k)  O(n log n)
O(n * k)  O(n^2)  O(n^2 log n)  O(n^3)  O(2^n)  O(n!)  other
```

It genuinely cannot answer "roughly linear-ish" — the JSON Schema rejects anything not on
the list. That's the constraint doing the work.

**The script's job — deciding.** Given those values, the filenames follow from rules that
live in `scripts/lib/complexity.mjs` and never change:

1. Rank every solution by time complexity, then by space complexity.
2. The best one is `optimal.cs`.
3. Anything tied on **both** time and space is `optimal-variant.cs`.
4. Everything else is `suboptimal.cs`.
5. A file already called `optimal.cs` that's still best **keeps its name** — otherwise the
   git history churns for no reason.
6. If any complexity came back as `other`, **refuse to name anything** and leave the folder
   for a human.

Rule 6 matters more than it looks. When the classifier met a bounded min-heap, the honest
answer was O(n log k), which wasn't on the list at the time. It answered `other` and the
script refused, instead of rounding to O(n log n) and producing a confident wrong answer.
I added the missing rung and it worked. **A model that can say "doesn't fit" is worth far
more than one that always produces something.**

### Why this is testable

Because rules 1–6 are ordinary code, I can test them without calling a model at all:

```
new O(n) beats incumbent O(n²) optimal.cs  →  submission-3.cs → optimal.cs
                                              optimal.cs     → suboptimal.cs
two solutions both O(n)/O(n)               →  optimal.cs, optimal-variant.cs
suboptimal pushed AFTER optimal            →  optimal.cs keeps its name
unrankable complexity                      →  refuses
```

All of those run in milliseconds and cost nothing.

---

## Part 5 — Each script, and what it's for

Everything lives in `scripts/`. Total: about 1,900 lines of JavaScript, plus a 710-line
HTML template.

### `scripts/lib/` — the shared pieces

| File | What it does |
|---|---|
| `scan.mjs` | The single source of truth for "what's in this repo and what's unprocessed". Every other script imports this. |
| `normalise.mjs` | Strips C# comments and whitespace, then hashes the result. Two files that differ only in formatting produce the same hash. |
| `complexity.mjs` | The complexity ladder and the naming rules. |
| `header.mjs` | Builds and replaces the short banner at the top of a file. |
| `teach.mjs` | Builds and replaces the long teaching block at the bottom. |
| `visualizer.mjs` | Splices a generated definition into the HTML template — and validates it. |

**Why `scan.mjs` exists separately:** originally `detect` and `apply` each had their own copy
of "what counts as pending". Two copies of one rule is how a pipeline starts lying to you —
the dry run says one thing and the real run does another. One file, imported twice.

**Why `normalise.mjs` is more careful than it looks:** it's a small state machine, not a
regular expression, because `string s = "// not a comment";` and `char c = '"';` defeat
every regex you'd reach for first. Getting it wrong makes the duplicate detection silently
unreliable, and a silently unreliable check is worse than none. It was tested by copying a
solution, deleting all its comments, mangling its indentation, and confirming the hash was
unchanged.

### `scripts/detect.mjs` — what needs doing

Read-only. Writes nothing, calls no model. It answers one question: which problem folders
contain `submission-N.cs` files that haven't been processed?

The clever bit is that **there's only one code path** for two different behaviours:

```bash
# Push run — skip the folder I just pushed to
node scripts/detect.mjs --exclude-changed-since <sha>

# Nightly run — drain everything, including that folder
node scripts/detect.mjs
```

Why skip the folder I just pushed to? Because I might not be finished. If I submit three
times in a row to the same problem, I don't want the pipeline curating attempt one while
I'm still writing attempt two. The nightly run picks it up later, once it's gone quiet.

Originally this was going to be two separate scripts — "process the previous problem" on
push, and "process the leftover" on cron. Two scripts means two sets of bugs. One script
with one flag does both, and the flag falls out of the trigger naturally.

### `scripts/classify.mjs` — the reading step

Calls Claude once per folder with all the solution files, gets back the structured
classification, applies the naming rules, renames with `git mv` (so history follows), and
writes the banner header.

**Two-phase rename.** When a new solution demotes the old one, two files swap names. A
single-pass rename destroys one of them — you move A to B, and B is gone. So it renames
everything to temporary names first, then to the final names. Tested by forcing a real
swap and confirming both file bodies survived byte-identical.

### `scripts/teach.mjs` — the study block

Calls Claude (Opus) once per *file* to write the long block that goes below the code. This
is where the bigger model earns its keep, because the task is open-ended.

The model chooses its own section headings — `WHY THIS PATTERN`, `INVARIANT`,
`WHY THE RESET LOSES NOTHING`, `WATCH OUT`, whatever fits. But it's told exactly which facts
already appear in the banner above, so it doesn't repeat them, and the complexity section is
generated by the script rather than the model, so the two can never disagree.

One instruction in that prompt is worth quoting:

> Do not assert performance folklore about the runtime, the JIT, or the compiler — if you
> cannot show it from the code, leave it out.

That's there because my hand-written example header contained the line *"Math.Max on ints is
branchless after JIT"* — a confident, plausible, unverifiable claim. A free-form notes
section attracts that kind of thing, and it lands in the material I revise from.

### `scripts/visualize.mjs` — the animation

The 23 visualizers I already had share **one design system exactly** — same paper-and-pencil
palette, same fonts, same layout, no external resources. Two different problems turned out
to be **89% byte-identical**.

So the pipeline doesn't ask a model to match a style it can't see. Instead:

1. Everything shared was extracted verbatim into `scripts/templates/visualizer.chassis.html`
   — 32KB of design system and playback engine with a single marker where the
   problem-specific part goes.
2. The model generates **only** that part: a `PROBLEM` object with the input parsing, and a
   `simulate()` function that produces the animation frames.
3. The script splices it in.

A generated visualizer is visually identical to the hand-built ones **by construction**, not
by instruction. The extraction was verified by putting the original piece back and checking
the result was byte-identical to the file it came from.

**It refuses to overwrite an existing visualizer.** The 23 already here were built and
checked by hand. Regenerating them would trade work that's known good for work that merely
passes validation.

### `scripts/lib/visualizer.mjs` — the part that matters most

A broken visualizer still *looks* like a finished 40KB file. So this doesn't inspect the
generated code, it **runs** it: parses it, feeds it its own default input, simulates every
solution, and checks every frame.

It was verified by deliberately breaking a working definition six different ways and
confirming each was caught:

| Injected fault | Caught as |
|---|---|
| Step highlights a line number outside its own code | `lines has 99, outside code lines 1..6` |
| `simulate()` throws part-way | `simulate() threw: boom` |
| A required field missing | `PROBLEM.title is missing` |
| Input parser rejects its own default | `parse() rejects its own default input` |
| Runaway loop generating frames | out-of-memory, reported |
| Syntax error | `the definition does not even run` |

The first one is the interesting failure. An out-of-range line number doesn't crash
anything — the visualizer just highlights nothing and reads as *dull*. Nobody reports a dull
visualizer as broken. That's exactly the class of fault that survives casual review, and
it's why validation runs the code rather than reading it.

### `scripts/apply.mjs` and `scripts/migrate-provenance.mjs`

`apply.mjs` regenerates the index table in `README.md` and updates `.agent/state.json`. No AI.

`migrate-provenance.mjs` is a one-off that solved a subtle problem — see Part 7.

---

## Part 6 — How it remembers things: `.agent/state.json`

A single JSON file, committed to the repo, recording per problem:

| Key | Meaning |
|---|---|
| `curatedFiles` | Which solution files exist |
| `processedSubmissions` | Raw submissions already dealt with |
| `fingerprints` | Hashes of solutions seen before, for duplicate detection |
| `provenance` | **Who wrote each file** — see Part 7 |
| `headerSignatures` | What the header was generated from |
| `classification` | Complexity, algorithm, and whether it's brute force |

### Why `headerSignatures` exists — the churn problem

Ask a model the same question twice and you get slightly different wording. Run one:

> *maintains window sum incrementally by adding nums[right] and subtracting nums[left]...*

Run two, same unchanged file:

> *two pointers maintain window sum incrementally by adding nums[right]...*

Both correct. Both different. And because the pipeline regenerates headers when it processes
a folder, **the nightly run would have reworded my files every single night, forever** — a
commit every day that changes nothing.

The fix: record a *signature* of everything the header is built from — filename, complexity,
algorithm, approach, the marker, the ranking — but **not the prose**. A header is rewritten
only when that signature moves. Identical facts with different wording produce the same
signature and no rewrite.

The signature also carries a format version number. Without it, when I later *changed the
header layout*, not a single existing file would have been updated — the facts hadn't
changed, so every file would politely have declined to be rewritten.

---

## Part 7 — The bugs, and what each one taught

This is the most useful section, because the bugs were more instructive than the features.

### 1. The contradiction — provenance can't be re-derived

**Symptom.** The banner said `YOU SOLVED THIS YOURSELF`. The teaching block, generated
seconds later from the same file, said `No '//My solution' marker in the source`.

**First diagnosis.** I mark my own solutions in the NeetCode editor by typing
`//My solution`, so the pipeline detects it with a regex. But the *generated header quotes
that text* — `marked '//My solution'` — and the classifier was reading past the banner into
the teaching block below. An annotation was manufacturing the very evidence it claimed to
report.

**The real problem, which was worse.** The marker lives in the raw NeetCode submission. When
I curated `submission-2` into `optimal.cs`, I rewrote the code, and the marker didn't
survive. My banner recorded it; the file no longer contained it.

So **absence of the marker in a curated file doesn't mean "not yours" — it means the evidence
was edited away.** Re-deriving it would have relabelled 12 of my own solutions as unmarked.

**The fix.** Provenance is now *recorded once*, at the only moment it's knowable — when a raw
submission is processed — and stored in `state.json`. It's carried across renames and never
re-derived. `migrate-provenance.mjs` seeded it for the existing 34 files by reading my own
annotations from before the pipeline existed: 12 mine, 22 reference, 0 unknown.

It's also **tri-state** now. Unknown reports as unknown, not as a denial. A header that
flatly claims I didn't write something I did is worse than one that stays quiet.

**The lesson.** Some facts can only be observed at one moment. Record them then, or lose them.

### 2. The state that erased itself

`classify.mjs` wrote the header signatures. `apply.mjs` ran immediately afterwards in the
same job, rebuilt each record from scratch, and **dropped every key it didn't produce
itself**. Written and deleted in the same job. Zero of 23 folders had a signature after two
runs that both showed green.

The visible symptom would have appeared weeks later as unexplained nightly commits.

**The lesson.** Two green runs and correct-looking output proved nothing. The defect was in
state nobody looks at.

### 3. The empty object that would have committed itself

`teach.mjs` read a value with a create-if-missing default, which stamped an empty
`teachSignatures: {}` onto every folder it merely *looked at*. That's a change to
`state.json`, which in the workflow becomes a commit — for work that never happened. Same
nightly-noise failure as the churn problem, arriving by a completely different route.

### 4. `--bare` versus the subscription

Early on I was told to use `--bare` in CI (it skips loading local config, making runs
reproducible) *and* to authenticate with `CLAUDE_CODE_OAUTH_TOKEN` so runs bill against my
Claude Pro subscription rather than per-token API credits.

Checking the documentation properly:

> Bare mode does not read `CLAUDE_CODE_OAUTH_TOKEN`. If your script passes `--bare`,
> authenticate with `ANTHROPIC_API_KEY` or an `apiKeyHelper` instead.

**They're mutually exclusive.** That combination would have either failed to authenticate or
quietly moved me onto per-token billing.

`--bare` was dropped. The safety it provided was recovered a different way — see Part 8.

### 5. The one where nothing happened

`visualize.mjs` failed with `max_turns` having written nothing. A "turn" is one round of the
model acting; the limit stops runaway loops.

The cause: `teach.mjs` tells the model *"you have no tools and no filesystem access"*. I
never carried that line into `visualize.mjs`. So it kept trying to read files, kept being
denied, and burned its entire turn budget on refused requests.

**The lesson.** A safety limit firing is a symptom. The limit wasn't wrong; something was
wasting the budget.

### 6. The bug that only existed because a diagnostic was bad

The first three failures all reported the same useless message, because my error handler
printed the *command that was run* and threw away the actual error. Once it printed the real
reason, the cause was obvious in one run: `Not logged in`.

**The lesson.** Time spent making failures legible pays for itself immediately.

---

## Part 8 — Safety

### It cannot trigger itself

A pipeline that commits on push can re-trigger itself and loop forever, burning quota. Two
independent guards:

1. **The token.** The pipeline commits using GitHub's built-in `GITHUB_TOKEN`. GitHub
   deliberately does not start new workflow runs from commits made with it. *Swapping in a
   personal access token defeats this — never do it.*
2. **The message.** Its commits are prefixed `chore(pipeline)`, and the workflow refuses to
   act on a commit whose message starts with that.

Verified by pushing test submissions and confirming exactly one run each time.

### It cannot be hijacked by the repo

Without `--bare`, `claude -p` will execute hooks from a `.claude/settings.json` and connect
servers from a `.mcp.json` **found in the checked-out repository, with no confirmation
prompt**. Since `--bare` costs the subscription, a guard step recovers the safety half:

```bash
for path in .claude .mcp.json; do
  if [ -e "$path" ]; then echo "::error::$path exists; refusing to run the model"; bad=1; fi
done
if [ -n "${ANTHROPIC_API_KEY:-}" ]; then
  echo "::error::ANTHROPIC_API_KEY is set; it outranks the subscription token. Refusing."
  bad=1
fi
```

Neither file belongs in this repo, so their appearance is either a mistake or an attack.
Refuse either way. The same step refuses to run if `ANTHROPIC_API_KEY` is present, because it
outranks the subscription token and would silently switch to paid billing.

### It cannot cost more than expected

- Every AI step is capped with `--limit`, so a runaway backfill can't process 23 folders.
- The nightly run only processes genuinely unprocessed submissions. On a quiet night it
  detects nothing, installs nothing, calls no model, and commits nothing.
- Backfill is manual only. `push` and `schedule` events carry no `inputs` object at all, so
  the backfill flag is structurally impossible to set from them. Not a guard — an impossibility.

---

## Part 9 — Operating it

### Day to day

Nothing. Solve problems on NeetCode. The pipeline handles the rest.

- **When I push a submission:** every *other* pending folder gets processed. The one I just
  touched is left alone in case I'm still working on it.
- **07:07 and 19:09 IST, daily:** everything left over gets processed, including that folder.

Two runs, twelve hours apart — not a run and a backup. Whatever the morning one leaves, the
evening one picks up, and vice versa. Both sit outside 23:00–06:00, when I study, so neither
competes with me for the same subscription usage. The odd minutes are deliberate too: GitHub
queues scheduled workflows at low priority, and :00, :15, :30 and :45 are where everyone
else's schedules pile up.

### Running it by hand

**Actions → NeetCode pipeline → Run workflow.** Three optional inputs:

| Input | Effect |
|---|---|
| `exclude` | Comma-separated slugs to hold back |
| `backfill` | Also process folders with no new submissions — for bringing old work up to standard |
| `limit` | Max folders this run (default 5) |

### Running a single step locally

```bash
node scripts/detect.mjs                                  # what's pending? (free)
node scripts/classify.mjs --slug two-integer-sum         # dry run, no changes
node scripts/classify.mjs --slug two-integer-sum --apply # do it
node scripts/teach.mjs --backfill --slug two-integer-sum --apply
node scripts/visualize.mjs --slug two-integer-sum --apply
```

Everything is reversible with `git checkout -- .` until committed.

### When something breaks

1. **Read the failing step's log**, not just the run's red or green badge. The AI steps are
   `continue-on-error`, so one bad folder doesn't strand renames that already succeeded —
   which also means the run can go green with work skipped.
2. **`node scripts/probe-cli.mjs`** — adds one CLI flag at a time on top of a bare call, so
   the first failure names the culprit. This is what found the `Not logged in` problem in one
   run after three failures that all looked identical.
3. **`.agent/tmp/last-claude-response.json`** — the full unabridged response from the last
   failed call.

---

## Part 10 — What it costs

Estimates from real runs, on a Claude Pro subscription:

| Step | Model | Roughly |
|---|---|---|
| Classify a folder | Sonnet | ~3 turns |
| Teaching block, per file | Opus | ~2 turns |
| Visualizer, per problem | Opus | ~2 turns, the largest single call |

The dollar figures the CLI prints are **client-side estimates of API pricing**, not bills.
On a subscription they're not charged; treat them as a relative cost signal only.

Classification runs on Sonnet deliberately. It emits six fields, two of them constrained to a
fixed list, ranked by code I can read — there's very little room for a larger model to be
better, and Sonnet reproduced my hand-made `optimal` / `optimal-variant` split on the hardest
folder in the repo. Opus is saved for the teaching blocks and visualizers, where the task is
open-ended and judged by eye.

One real cost worth knowing: because `--bare` had to be dropped, every call also loads my
local Claude configuration — roughly 28,000 tokens of context that have nothing to do with
ranking a sliding window. It caches, so repeat calls are much cheaper, but it isn't free.
That's the price of using the subscription instead of paying per token.

---

## Part 11 — The file map

```
.github/workflows/pipeline.yml     the workflow: triggers, steps, guards
.gitattributes                     line-ending rules (see below)
.gitignore                         build noise, node_modules, scratch

.agent/state.json                  everything the pipeline remembers
.agent/tmp/                        scratch, not committed

scripts/
  detect.mjs                       what needs doing            (no AI)
  classify.mjs                     complexity + ranking + rename (Sonnet)
  teach.mjs                        the study block             (Opus)
  visualize.mjs                    the animation               (Opus)
  apply.mjs                        README index + state        (no AI)
  migrate-provenance.mjs           one-off provenance recovery (no AI)
  probe-cli.mjs                    debugging tool              (tiny AI calls)
  lib/
    scan.mjs                       one definition of "pending"
    normalise.mjs                  comment-stripping + hashing
    complexity.mjs                 the ladder and the naming rules
    header.mjs                     the short banner
    teach.mjs                      the long block
    visualizer.mjs                 splicing + validation
  templates/
    visualizer.chassis.html        32KB of design, lifted verbatim
```

### A note on `.gitattributes`

Windows ends lines with two characters (CRLF); Linux uses one (LF). The pipeline runs on
Linux, I work on Windows. Without a rule, every file the pipeline touched would come back as
a whole-file rewrite.

The rule splits by file type: `.cs`, `.html` and `.md` check out as CRLF so they look right
in Visual Studio; `.mjs`, `.yml` and `.sh` check out as LF because a shell script with a
stray carriage-return after `#!/bin/bash` is an unrunnable file. Git stores LF for everything.

A one-time `git add --renormalize` fixed the 57 files that already had CRLF stored.

---

## Part 12 — What I'd tell someone in one minute

I automated the boring half of my DSA revision workflow, and the interesting part was
deciding what *not* to automate.

Every AI call in the pipeline answers a narrow question with a constrained answer — pick a
complexity from this list of twelve, write a study note about this specific function. Nothing
open-ended, nothing that reaches for tools. All the *decisions* — which file becomes
`optimal.cs`, whether a header needs rewriting, whether a visualizer is safe to publish — are
ordinary code I can read, test, and disagree with.

That's what makes it debuggable. When it named a file wrongly, I could point at the exact
rule that was wrong (it ranked on time and ignored space) and fix it in five lines. If the
whole thing had been one agent told to "organise my repo sensibly", I'd have had nothing to
point at.

The other half of the lesson is that **most of the real bugs weren't in the AI parts at
all**. They were in state management, line endings, error reporting, and one place where two
steps disagreed about a fact. The AI did the reading job well throughout. The scaffolding
around it is what needed six rounds of fixing.
