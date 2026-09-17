#!/usr/bin/env node
/**
 * visualize.mjs - MILESTONE 4b. Builds <slug>-visualizer.html for a problem
 * that does not have one.
 *
 * It NEVER overwrites an existing visualizer. The 23 already in this repo were
 * built and checked by hand; regenerating them would trade work that is known
 * good for work that merely validates.
 *
 * Only the PROBLEM object is generated. The surrounding 32KB - palette, fonts,
 * panels, transport, keyboard handling - is spliced verbatim from the chassis,
 * so a new visualizer matches the existing ones exactly rather than
 * approximately.
 *
 *   node scripts/visualize.mjs --slug two-integer-sum          # dry run
 *   node scripts/visualize.mjs --slug two-integer-sum --apply
 *   node scripts/visualize.mjs --apply --limit 3
 *   node scripts/visualize.mjs --apply --backfill --limit 3   # replaces existing ones
 */

import { readFileSync, writeFileSync, existsSync, appendFileSync } from 'node:fs';
import { join } from 'node:path';
import { execFileSync } from 'node:child_process';
import { loadState, saveState, scanRepo, REPO } from './lib/scan.mjs';
import { stripHeader } from './lib/header.mjs';
import { splitTrailingTeach } from './lib/teach.mjs';
import { shortPrint } from './lib/normalise.mjs';
import { splice, validate, selectForVisualizer, loadChassis } from './lib/visualizer.mjs';
import { catalogueSection, contractSection, required, VISUALIZER_FORMAT } from './lib/shapes.mjs';
import { report, reportCost, group, endGroup } from './lib/report.mjs';
import { isOutOfBudget, stop as budgetStop, announce as announceBudget, haltIfStopped } from './lib/budget.mjs';
import { effortArgs, usageOf, usageLine, addUsage, leanArgs } from './lib/usage.mjs';
import { mkdirSync } from 'node:fs';

const argv = process.argv.slice(2);
const arg = (n, d = null) => (argv.includes(n) ? argv[argv.indexOf(n) + 1] : d);
const has = (n) => argv.includes(n);

const onlyRaw = arg('--slug');
const only = onlyRaw ? onlyRaw.split(',').map((x) => x.trim()).filter(Boolean) : null;
const limit = Number(arg('--limit', '0')) || 0;
const doApply = has('--apply');
// Opus at medium effort since 2026-09-17. Sonnet at high effort held this slot before it, and was
// correct, but spent 5-8 minutes and ~39,000 thinking tokens to get there. Opus at medium reaches
// the same place in well under two minutes on ~1,700 thinking tokens, for roughly the same money -
// measured on binary-tree-diameter: Sonnet high $0.22, Opus medium $0.31 (and $0.31 included a
// wasted cache write; see lib/usage.mjs). The wall-clock is the real win on a multi-folder run.
//
// Opus medium's one weakness was fidelity: it paraphrased the code panel, inventing comments and
// reflowing a statement, while every other check still passed. The prompt now states that "code"
// is the file copied verbatim and validate() enforces it against the real source, so that class of
// drift fails the build instead of shipping quietly.
//
// If validation rejects the first attempt, attempt 2 is a REPAIR on Opus, not a fresh build. It
// gets the rejected definition and the exact errors, and returns the same object with only those
// fixed - no worked example, no long requirements. A fresh Opus build threw away an answer that
// was 95% right and cost ~$0.50 and 3-8 minutes; a repair is a small, scoped task. There is no
// third attempt: a failed repair fails the folder loudly.
// An explicit --model is respected on both attempts.
const modelGiven = argv.includes('--model');
const model = arg('--model', 'opus');
const RETRY_MODEL = 'opus';
// The repair's effort. Unset = Opus's own default (high): a repair is the one place extra thinking
// is worth paying for, because attempt 1 already failed.
const repairEffort = arg('--repair-effort');
// Local test of the repair alone: start from an existing definition (a visualizer .html or a .js
// holding `const PROBLEM = ...`) instead of paying for attempt 1. If that definition passes, one
// deliberate error is injected so there is something to repair. Dry runs only.
const repairFrom = arg('--repair-from');
// No --effort means the model's own default. Only ever set by hand, for comparisons.
const effort = arg('--effort', modelGiven ? null : 'medium');
const backfill = has('--backfill');
// Also pick up any curated folder with no visualizer at all. Nothing is in that state
// normally; it is how a folder gets finished after a failed run stopped before visualize.
const unfinished = has('--unfinished');

const SCHEMA = {
  type: 'object',
  additionalProperties: false,
  properties: {
    problemSource: {
      type: 'string',
      description: 'JavaScript source for exactly one statement: const PROBLEM = { ... }; Nothing else - no imports, no markdown fence, no commentary.',
    },
  },
  required: ['problemSource'],
};

/** The contract, taken from the chassis itself rather than described from memory. */
function contract() {
  const c = loadChassis();
  const helpers = c.slice(c.indexOf('<script>') + 8, c.indexOf('/*__PROBLEM__*/'));
  return helpers.trim();
}

/** Lift the PROBLEM definition out of a finished visualizer. */
function definitionFrom(dir, slug) {
  return definitionFromFile(join(dir, `${slug}-visualizer.html`));
}

function definitionFromFile(path) {
  const src = readFileSync(path, 'utf8').split(/\r?\n/);
  const s = src.findIndex((l) => /^const PROBLEM = \{/.test(l));
  if (s < 0) return null;
  let depth = 0, end = -1;
  for (let i = s; i < src.length && end < 0; i++) {
    for (const ch of src[i]) { if (ch === '{') depth++; else if (ch === '}' && --depth === 0) { end = i; break; } }
  }
  return end < 0 ? null : src.slice(s, end + 1).join('\n');
}

const FALLBACK_EXAMPLE = { slug: 'buy-and-sell-crypto', dir: join(REPO, 'Data Structures & Algorithms', 'buy-and-sell-crypto') };

/**
 * A worked example, chosen for SHAPE where one exists.
 *
 * This used to be one hard-coded array problem handed to every folder, under
 * the instruction to match it exactly. That is most of why every visualizer in
 * the repo came out array-shaped: a tree problem was being shown a row of tiles
 * and told to copy it. The panels were only half the cause; this was the rest.
 *
 * So: prefer a finished visualizer that shares an ENFORCED structure with this
 * problem - the first tree one built becomes the example for every tree problem
 * after it, which makes the step quietly better as the backfill proceeds. When
 * nothing shares a shape, fall back to the array problem and SAY it is a
 * different shape, so its panel choice is not read as a template.
 */
function pickExample(slug, structures, state, all) {
  const want = new Set(required(structures).map((r) => r.structure));
  if (want.size) {
    const scored = all
      .filter((p) => p.slug !== slug && p.hasVisualizer && state.problems[p.slug]?.visualizer)
      .map((p) => {
        const cls = state.problems[p.slug]?.classification ?? {};
        const have = new Set(Object.values(cls).flatMap((c) => c.structures ?? []));
        return { p, hits: [...want].filter((x) => have.has(x)).length };
      })
      .filter((x) => x.hits > 0)
      .sort((a, b) => b.hits - a.hits || a.p.slug.localeCompare(b.p.slug));
    for (const { p } of scored) {
      const def = definitionFrom(p.dir, p.slug);
      if (def) return { source: def, from: p.slug, sameShape: true };
    }
  }
  const def = definitionFrom(FALLBACK_EXAMPLE.dir, FALLBACK_EXAMPLE.slug);
  return { source: def, from: FALLBACK_EXAMPLE.slug, sameShape: false };
}

/**
 * Attempt 2: fix a rejected definition rather than rebuild it.
 *
 * Deliberately shorter than instructions(): the rejected answer already follows the voice, the
 * panel conventions and the worked example, so those are not sent again. The helper contract and
 * the structure rules stay, because a fix must still use the real panel API.
 */
function repairInstructions(slug, sols, structures, previous, errors) {
  return [
    `You are REPAIRING the PROBLEM definition for the NeetCode problem "${slug}". It was written for an`,
    'existing visualizer and it failed validation. It is almost right.',
    '',
    'These helper functions and panel constructors already exist. Use them; do not redefine them:',
    '```', contract(), '```',
    '',
    `The solutions, in order (each "lines" index is 1-based into THAT solution's own "code" array):`,
    ...sols.map((s) => `  - ${s.file}: ${s.time} time / ${s.space} space, ${s.algorithm}` +
      ((s.structures ?? []).length ? `; made of: ${s.structures.join(', ')}` : '')),
    '',
    contractSection(structures),
    '',
    'The definition that failed:',
    '```', previous, '```',
    '',
    'The validator found exactly these problems:',
    ...errors.map((e) => `  - ${e}`),
    '',
    'Fix these problems and nothing else. Keep every panel, step, message and piece of wording as it is,',
    'unless fixing a listed problem requires changing it. Do not redesign, shorten or rewrite the animation.',
    'Plain-text fields are escaped on the way in: write < and & as themselves, never as entities.',
    'Output only the full corrected statement: const PROBLEM = { ... };',
    '',
    'You have no tools and no filesystem access. The solution code follows below, and everything else',
    'you need is above. Do not attempt to read, list or search files - answer directly.',
  ].join('\n');
}

function instructions(slug, sols, structures, example, feedback) {
  return [
    `Write the PROBLEM definition for the NeetCode problem "${slug}", to be spliced into an existing visualizer.`,
    '',
    'These helper functions and panel constructors already exist. Use them; do not redefine them:',
    '```', contract(), '```',
    '',
    // The example teaches the OBJECT - its fields, its register, how much
    // detail a msg carries. It must not teach the panel choice unless it
    // genuinely shares a shape, or every problem inherits the last one's
    // drawing, which is exactly how this step came to render trees as rows.
    example.sameShape
      ? `Here is a complete, working definition for "${example.from}", which is built from the same kind of` +
        '\nstructure as this one. Match its voice and its level of detail, and treat its panel choices as a' +
        '\nsound starting point - though yours should follow this problem\'s code, not copy that one\'s:'
      : `Here is a complete, working definition for "${example.from}". Match the SHAPE OF THE OBJECT, the` +
        '\nvoice and the level of detail. Do NOT copy its choice of panels - it is a differently shaped' +
        '\nproblem, and its panels are right for it and probably wrong for yours:',
    '```', example.source, '```',
    '',
    `Build one entry in "solutions" for each of these ${sols.length} solution file(s), in this order,`,
    'faithfully animating what that code actually does - not a tidier algorithm you would prefer:',
    ...sols.map((s) => `  - ${s.file}: ${s.time} time / ${s.space} space, ${s.algorithm}. badge should end with "${s.file}".` +
      ((s.structures ?? []).length ? `\n      made of: ${s.structures.join(', ')}` : '')),
    '',
    contractSection(structures),
    '',
    catalogueSection(structures),
    '',
    'Requirements:',
    '- Draw each structure as the thing it IS. A tree has edges, a stack is a bucket you push onto and',
    '  pop off, a linked list is boxes joined by arrows, a matrix is a grid. A row of boxes is the right',
    '  drawing for an array and the wrong one for everything else.',
    '- Every step\'s "lines" must be 1-based indices into THAT solution\'s own "code" array. A line',
    '  number outside it highlights nothing and the visualizer silently reads as broken.',
    '- "code" is the reader\'s own file, not a retelling of it. Copy the lines of that solution file',
    '  character for character, in order, starting at the first line of code below the stripped',
    '  header. Do not add explanatory comments, do not reword or drop the comments that are there,',
    '  do not reflow one statement across two lines or join two onto one, and do not rename anything.',
    '  You may stop early at a natural end, but every line you emit must appear verbatim in the file.',
    '  A reader following a line number here opens that file and expects to land on the same line.',
    '- parse() must accept its own default input value.',
    '- Keep the default input small enough that the whole run is watchable - well under 60 steps.',
    '- "msg" is HTML; <b>, <code> and <em> are available. Explain WHY the step happens.',
    '- Everything else is PLAIN TEXT and is escaped on the way in - a chip\'s text, a stack frame\'s',
    '  sub, a node\'s value, a pill\'s k and v. Write < and & as themselves there, never as &lt; or',
    '  &amp;, or the entity arrives on screen character by character.',
    '- "blurb" must fit five rendered lines: keep it under 450 characters of visible text, and',
    '  under 300 if you can. The existing visualizers in this repo average about 210. It is the',
    '  one-paragraph reason the approach works, not a summary of the teaching block.',
    '- Use `scale` to keep a panel on one screen rather than letting it scroll sideways.',
    '- Output only the statement: const PROBLEM = { ... };',
    '',
    'You have no tools and no filesystem access. The solution code follows below, and everything else',
    'you need is above. Do not attempt to read, list or search files - answer directly.',
    ...(feedback ? ['', 'Your previous attempt failed validation. Fix exactly these:', ...feedback.map((e) => `  - ${e}`)] : []),
  ].join('\n');
}

// Windows caps a whole command line at 32,767 characters, and this prompt - the chassis
// contract plus a complete worked example - is longer than that. Passed as an argument it
// failed locally with "spawnSync claude ENAMETOOLONG" before any model was called (Linux CI
// allows far more, so it only ever broke on Windows). So the instructions travel on stdin
// with the code, and the argument is one short line. stdin allows 10MB.
const STDIN_POINTER = 'Your full instructions come first on stdin, followed by the solution code. Follow the instructions exactly.';

function ask(prompt, code, useModel = model, useEffort = effort) {
  const args = ['-p', STDIN_POINTER, '--output-format', 'json', '--json-schema', JSON.stringify(SCHEMA),
                '--permission-mode', 'dontAsk', '--max-turns', '20', '--model', useModel, ...effortArgs(useEffort), ...leanArgs()];
  let raw;
  try {
    raw = execFileSync('claude', args, {
      input: `${prompt}\n\n===== SOLUTION CODE =====\n\n${code}`,
      encoding: 'utf8', maxBuffer: 64 * 1024 * 1024, stdio: ['pipe','pipe','pipe'],
    });
  } catch (e) {
    let env = null; try { env = JSON.parse(String(e.stdout ?? '')); } catch { /* not JSON */ }
    const detail = env
      ? [env.terminal_reason, env.subtype, env.num_turns != null ? `${env.num_turns} turns used` : null,
         env.result ? String(env.result).slice(0, 300) : null].filter(Boolean).join(' · ')
      : String(e.stderr || e.message).slice(0, 300);
    throw new Error(`claude failed (exit ${e.status}): ${detail}`);
  }
  const env = JSON.parse(raw);
  if (!env.structured_output?.problemSource) throw new Error(`no problemSource (result: ${String(env.result).slice(0,200)})`);
  // Models like fences even when told not to.
  const src = env.structured_output.problemSource.replace(/^\s*```(?:javascript|js)?\s*/i, '').replace(/```\s*$/, '').trim();
  return { src, cost: env.total_cost_usd, turns: env.num_turns, usage: usageOf(env) };
}

// ---------------------------------------------------------------- run

/**
 * The account ran out, not the definition.
 *
 * "You've hit your session limit" is not a bad PROBLEM object - it is the same
 * answer every remaining folder will get, so continuing wastes a call per
 * folder and, worse, reports each one as `failed` beside the genuine
 * validation failures. A run once ended "2 written, 2 failed" when nothing was
 * wrong with either of the two: the subscription had simply reset-time on it.
 */
let outOfBudget = null;

const state = loadState();
const all = scanRepo(state);
let targets = all;
const stranded = (p) => p.curatedFiles.length > 0 && !existsSync(join(p.dir, `${p.slug}-visualizer.html`));
if (only) targets = targets.filter((p) => only.includes(p.slug) || (unfinished && stranded(p)));
else if (unfinished) targets = targets.filter(stranded);
/** What the code looks like now, ignoring comments and whitespace. */
const codePrints = (p, files) => Object.fromEntries(files.map((f) => [
  f, shortPrint(stripHeader(splitTrailingTeach(readFileSync(join(p.dir, f), 'utf8')).code).body),
]));

const skipped = [];
targets = targets.filter((p) => {
  if (!existsSync(join(p.dir, `${p.slug}-visualizer.html`))) return true;
  if (backfill) return true;

  // A visualizer names the variables in the code it animates. lint runs over
  // EVERY file in a folder, curated ones included, so resubmitting to a problem
  // you had already curated can rename variables the existing visualizer still
  // shows. Refusing to overwrite would leave it describing code that is gone.
  //
  // So: rebuild when the code has changed since the visualizer was built. A
  // visualizer with no record of what it was built from is one of the hand-made
  // ones - left alone on an ordinary run, and backfill's job.
  const rec = state.problems[p.slug]?.visualizer;
  if (!rec?.prints) {
    if (only) {
      console.log(`\n${p.path} has a hand-built visualizer - not overwritten. --backfill replaces it (tick Back-Fill in the workflow).\n`);
      skipped.push([p.slug, 'hand-built visualizer left alone - tick Back-Fill to replace it']);
    }
    return false;
  }
  const now = codePrints(p, Object.keys(rec.prints).filter((f) => existsSync(join(p.dir, f))));
  const changed = JSON.stringify(now) !== JSON.stringify(rec.prints);
  if (!changed && only) {
    console.log(`\n${p.path} visualizer is current - the code it animates has not changed.\n`);
    skipped.push([p.slug, 'current - the code it animates has not changed']);
  }
  return changed;
});

// Deciding NOT to rebuild is a result, and the summary has a column for this
// step. Without these the table showed "not run" for a folder the step had
// looked at and made a correct call on - the one reading that most invites a
// pointless second run. Only reported for folders you named: on an ordinary
// run this filter drops every folder that is already fine, and thirty rows
// saying "nothing needed" is not a summary.
for (const [slug, why] of skipped) report('visualize', slug, 'skipped', why);
if (limit) targets = targets.slice(0, limit);

if (!targets.length) { console.log('\nNothing to build - every problem already has a visualizer.\n'); process.exit(0); }

console.log(`\n${doApply ? 'APPLY' : 'DRY RUN'} - visualizers, model ${model}, effort ${effort ?? 'default'}, ${targets.length} folder(s)\n`);
if (haltIfStopped('visualize', targets.map((p) => p.slug))) process.exit(1);

let failures = 0, wrote = 0;

for (const p of targets) {
  group(p.path);
  const cls = state.problems[p.slug]?.classification ?? {};
  const { chosen, dropped, unclassified } = selectForVisualizer(p.curatedFiles, cls);
  if (dropped.length) console.log(`  dropped (brute force, real solutions exist): ${dropped.join(', ')}`);
  if (unclassified.length) console.log(`  unclassified, run classify --apply first: ${unclassified.join(', ')}`);
  if (!chosen.length) { console.log('  nothing to visualise'); report('visualize', p.slug, 'failed', unclassified.length ? 'no classification on record' : 'nothing to visualise'); failures++; endGroup(); continue; }
  console.log(`  visualising: ${chosen.join(', ')}`);

  const sols = chosen.map((f) => ({ file: f, ...cls[f] }));

  // What the chosen solutions are made of, unioned. The union rather than the
  // intersection: a problem whose recursive version uses a call stack and whose
  // iterative one does not must still show the frames on the recursive tab, and
  // the per-solution override is how the other tab says it does not need them.
  const structures = [...new Set(chosen.flatMap((f) => cls[f]?.structures ?? []))];
  if (structures.length) {
    console.log(`  made of: ${structures.join(', ')}`);
    const req = required(structures);
    if (req.length) console.log(`  must be drawn: ${req.map((r) => r.structure).join(', ')}`);
  } else {
    // Not a failure: every folder classified before the structures field
    // existed lands here. Say so plainly rather than silently dropping to the
    // old behaviour, because the old behaviour is the bug being fixed.
    console.log('  no structures on record - run classify --backfill --apply first for shape enforcement');
  }

  const example = pickExample(p.slug, structures, state, all);
  if (!example.source) { console.log('  cannot read the worked example'); report('visualize', p.slug, 'failed', 'worked example unreadable'); failures++; endGroup(); continue; }
  console.log(`  example: ${example.from}${example.sameShape ? ' (same shape)' : ' (different shape - voice only)'}`);

  // The same stripped bodies twice: joined for the model on stdin, and kept per
  // file so validate() can check the code panel is that file copied verbatim.
  const bodies = chosen.map((f) => stripHeader(splitTrailingTeach(readFileSync(join(p.dir, f), 'utf8')).code).body);
  const code = chosen
    .map((f, i) => `===== FILE: ${f} =====\n${bodies[i]}`)
    .join('\n\n');

  let result = null, feedback = null, spend = 0, used = null;
  // The definition validation last rejected. When set, the next attempt repairs it.
  let rejected = null;
  if (repairFrom) {
    if (doApply) { console.log('  --repair-from is for dry runs only'); process.exit(1); }
    let src = /\.html?$/i.test(repairFrom) ? definitionFromFile(repairFrom) : readFileSync(repairFrom, 'utf8');
    let errs = validate(src, structures, sols.map((s) => s.structures ?? []), bodies).errors;
    if (!errs.length) {
      // Nothing wrong with it: break one step's line number so the repair has a real job to do.
      src = src.replace(/lines\s*:\s*\[/, 'lines:[999, ');
      errs = validate(src, structures, sols.map((s) => s.structures ?? []), bodies).errors;
      console.log(`  --repair-from: definition was valid; injected a bad line number -> ${errs.length} error(s)`);
    }
    rejected = { src, errors: errs };
  }

  for (let attempt = rejected ? 2 : 1; attempt <= 2 && !result; attempt++) {
    let r;
    const repairing = !!rejected;
    const useModel = repairing && !modelGiven ? RETRY_MODEL : model;
    const useEffort = repairing && !modelGiven ? repairEffort : effort;
    try {
      if (repairing) {
        console.log(`  attempt ${attempt}: repairing on ${useModel}${useEffort ? ` (effort ${useEffort})` : ''} - ${rejected.errors.length} error(s) to fix`);
        r = ask(repairInstructions(p.slug, sols, structures, rejected.src, rejected.errors.slice(0, 25)), code, useModel, useEffort);
      } else {
        r = ask(instructions(p.slug, sols, structures, example, feedback), code, useModel, useEffort);
      }
    }
    catch (e) {
      console.log(`  attempt ${attempt} FAILED: ${e.message}`);
      // Out of budget is a property of the account, not of this folder. Record
      // it and let the loop below stop, rather than asking the same question
      // once per remaining folder and getting the same refusal each time.
      if (isOutOfBudget(e)) { outOfBudget = e.message; budgetStop('visualize', e.message); break; }
      // Retry a run that simply ran out of room; do not retry an auth or
      // configuration failure, which will fail identically the second time.
      if (!repairing && /max_turns|overloaded|rate_limit|timeout/i.test(e.message)) {
        feedback = ['Your previous attempt ran out of room before finishing. Answer immediately, in one reply, with no preamble.'];
        continue;
      }
      break;
    }
    spend += r.cost ?? 0;
    used = addUsage(used, r.usage);
    reportCost('visualize', p.slug, r.cost, r.usage);
    console.log(`  attempt ${attempt}: $${(r.cost ?? 0).toFixed(4)} · ${r.turns} turns · ${usageLine(r.usage)}`);
    // Each solution is checked against its OWN file's structures - see structuresFor.
    const v = validate(r.src, structures, sols.map((s) => s.structures ?? []), bodies);
    if (!v.errors.length) {
      result = r;
      if (repairing) console.log('      repaired - validation passes');
      for (const [k, n] of Object.entries(v.stats)) console.log(`      ${k}: ${n}`);
      // A waiver is a claim that a structure is not really there. Print it:
      // it is the one thing here that is accepted on the model's say-so, so it
      // should be the one thing that is impossible to miss in the log.
      for (const w of v.waived ?? []) console.log(`      WAIVED  ${w}`);
    } else {
      console.log(`  attempt ${attempt} rejected by validation (${v.errors.length} error(s)):`);
      v.errors.slice(0, 6).forEach((e) => console.log(`      ${e}`));
      rejected = { src: r.src, errors: [...new Set(v.errors)] };
      // Kept locally (gitignored) so a repair can be re-run with --repair-from without paying again.
      try {
        mkdirSync(join(REPO, '.agent', 'tmp'), { recursive: true });
        writeFileSync(join(REPO, '.agent', 'tmp', `visualize-rejected-${p.slug}.js`), r.src, 'utf8');
      } catch { /* diagnostics only */ }
    }
  }

  if (!result) {
    if (outOfBudget) {
      console.log(`  stopping: the account is out of budget, not the definition`);
      report('visualize', p.slug, 'skipped', 'stopped - out of budget, nothing wrong with this folder');
      endGroup();
      break;
    }
    console.log(`  giving up on ${p.slug}`);
    report('visualize', p.slug, 'failed', 'validation rejected the build and the repair');
    failures++;
    endGroup();
    continue;
  }
  console.log(`  validated  ·  $${spend.toFixed(4)} · ${result.turns} turns`);
  console.log(`  total ${usageLine(used)}`);

  if (!doApply) {
    // Kept on disk (gitignored) so two models' animations can be opened side by side.
    const tryDir = join(REPO, '.agent', 'tmp', 'try');
    mkdirSync(tryDir, { recursive: true });
    const saved = join(tryDir, `${p.slug}-visualizer.${model}-${effort ?? 'default'}.html`);
    writeFileSync(saved, splice(result.src), 'utf8');
    console.log(`  saved for comparison: ${saved}`);
  }

  if (doApply) {
    const out = join(p.dir, `${p.slug}-visualizer.html`);
    writeFileSync(out, splice(result.src), 'utf8');
    // Record what it was built from, so a later code change is detectable.
    const prec = state.problems[p.slug] ?? (state.problems[p.slug] = {});
    prec.visualizer = {
      v: VISUALIZER_FORMAT,
      files: chosen, prints: codePrints(p, chosen),
      structures, builtAt: new Date().toISOString(),
    };
    console.log(`  wrote ${p.slug}-visualizer.html`);
    report('visualize', p.slug, 'ok', `${chosen.join(' + ')}, validated`);
    wrote++;
  }
  endGroup();
}

if (doApply && wrote) saveState(state);
if (outOfBudget) announceBudget(outOfBudget);
console.log(`${wrote} written, ${failures} failed${outOfBudget ? ', rest not attempted' : ''}.\n`);
if (process.env.GITHUB_OUTPUT) appendFileSync(process.env.GITHUB_OUTPUT, `wrote=${wrote}\n`);
process.exit(failures || outOfBudget ? 1 : 0);
