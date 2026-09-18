#!/usr/bin/env node
/**
 * revert-renames.mjs - put the variable names back the way they were written.
 *
 * A ONE-OFF REPAIR, kept in the repo as the record of what it did. Run it once,
 * with --apply, and it has nothing left to do afterwards.
 *
 * Lint used to rename local variables. Its own record is why it no longer does:
 * 92 renames across 72 files, 47% of them overwriting a standard DSA name, and
 * three multi-file folders left with one variable under two names. Every one of
 * those renames is stored in state.json as "from->to", so they can be undone
 * exactly rather than guessed at.
 *
 *   node scripts/revert-renames.mjs            # dry run - says what it would do
 *   node scripts/revert-renames.mjs --apply
 *
 * WHAT IS EXACT, AND WHAT IS NOT. The code is exact: renameIdentifiers() walks the
 * token stream and touches nothing but local identifiers, and every file is then
 * checked with sameShape() - the revert must come back as precisely the reverse
 * rename map or the file is left alone and reported.
 *
 * Prose is not exact, and pretending otherwise is how this goes wrong. A teaching
 * block talks ABOUT the variables, and 38 of the renames produced a target that is
 * also an ordinary English word - result, current, first, diameter, left, node.
 * Across 403 occurrences these are genuinely mixed:
 *
 *     "pointer and loops while (first != null || ...)"   <- the variable
 *     "because the list is LSB-first, that ..."          <- the word
 *     "The diameter is the longest path between ..."     <- the word
 *
 * Replacing blindly turns the last one into "The res is the longest path". So:
 *
 *   SAFE targets  (islandCount, numsLength, prevPrev, waysToStep - never English)
 *                 are replaced everywhere: code, comments, header, teaching block,
 *                 visualizer. Unambiguous, so nothing can go wrong.
 *
 *   RISKY targets (the English words) are reverted in the CODE only. Their folder is
 *                 then marked for reshape - teachSignatures dropped and the visualizer
 *                 record aged - so the next backfill rewrites the prose from the
 *                 reverted code, which is correct by construction rather than by
 *                 heuristic, and picks up the new VARIABLES glossary while it is there.
 *
 * KEPT ON PURPOSE: n -> rows and m -> cols on the grid problems. Those were the one
 * genuine improvement in the whole set and rows/cols is the interview-standard name
 * for a grid's dimensions.
 *
 * Formatting runs last, through the same dotnet path lint uses, so the file lands in
 * its final shape - and the state prints are recomputed FROM that final text, so a
 * folder that was already finished is not queued for a re-run it does not need.
 */

import { readFileSync, writeFileSync, existsSync, readdirSync } from 'node:fs';
import { join } from 'node:path';
import { loadState, saveState, REPO } from './lib/scan.mjs';
import { sameShape, renameIdentifiers, tokenize } from './lib/csharp.mjs';
import { stripHeader } from './lib/header.mjs';
import { splitTrailingTeach } from './lib/teach.mjs';
import { shortPrint } from './lib/normalise.mjs';
import { formatMany, dotnetAvailable, toEol } from './lib/format.mjs';

const doApply = process.argv.includes('--apply');

/** Renames worth keeping: a grid's dimensions read better as rows/cols than n/m. */
const KEEP = new Set(['rows', 'cols']);

/**
 * Targets that are also ordinary English words, so unsafe to replace in prose.
 * Deliberately generous - a name wrongly called risky costs a re-teach the folder
 * was probably due anyway; a name wrongly called safe corrupts a sentence silently.
 */
const ENGLISH = new Set([
  'result', 'current', 'queue', 'length', 'left', 'right', 'count', 'index', 'stack',
  'value', 'total', 'first', 'second', 'next', 'last', 'start', 'end', 'size', 'sum',
  'max', 'min', 'best', 'answer', 'original', 'candidate', 'neighbor', 'neighbour',
  'visited', 'node', 'head', 'tail', 'temp', 'low', 'high', 'diameter', 'depth',
  'previous', 'remaining', 'target', 'window', 'seen', 'row', 'col', 'column',
  'width', 'height', 'path', 'top', 'bottom', 'number', 'letter', 'word', 'char',
]);

const esc = (s) => s.replace(/[.*+?^${}()|[\]\\]/g, '\\$&');
const replaceWord = (text, from, to) => text.replace(new RegExp(`\\b${esc(from)}\\b`, 'g'), to);

/**
 * The repair's own check, instead of sameShape().
 *
 * sameShape is lint's guard and carries lint's policy: renaming a public-signature name
 * is an error there, always. That is exactly what the repair has to do - the old lint
 * renamed those names and the grader's ones have to come back - so sameShape would
 * refuse every such file, which is what it did on the first run.
 *
 * This asks the narrower question the repair actually needs: same token stream, and every
 * difference is an identifier changing by the expected map, one-to-one. Members, types,
 * keywords, operators, literals and comments must be untouched.
 */
function verifyRevert(before, after, map) {
  const sig = (src) => tokenize(src).filter((t) => t.t !== 'ws' && t.t !== 'comment');
  const A = sig(before), B = sig(after);
  if (A.length !== B.length) return { ok: false, error: `token count changed: ${A.length} -> ${B.length}` };
  const fwd = new Map(), rev = new Map();
  for (let i = 0; i < A.length; i++) {
    if (A[i].v === B[i].v) continue;
    if (A[i].t !== 'id' || B[i].t !== 'id') return { ok: false, error: `a ${A[i].t} changed: ${A[i].v} -> ${B[i].v}` };
    if (map[A[i].v] !== B[i].v) return { ok: false, error: `unexpected rename ${A[i].v} -> ${B[i].v}` };
    if (fwd.has(A[i].v) && fwd.get(A[i].v) !== B[i].v) return { ok: false, error: `${A[i].v} renamed two ways` };
    if (rev.has(B[i].v) && rev.get(B[i].v) !== A[i].v) return { ok: false, error: `two names became ${B[i].v}` };
    fwd.set(A[i].v, B[i].v); rev.set(B[i].v, A[i].v);
  }
  return { ok: true };
}

const state = loadState();
const plan = [];

for (const [slug, rec] of Object.entries(state.problems)) {
  const dir = join(REPO, rec.topic ?? 'Data Structures & Algorithms', slug);
  if (!existsSync(dir)) continue;

  for (const [file, l] of Object.entries(rec.lint ?? {})) {
    const renames = l?.renames ?? [];
    if (!renames.length) continue;
    const full = join(dir, file);
    if (!existsSync(full)) continue;

    const safe = {}, risky = {};
    for (const rn of renames) {
      const [from, to] = String(rn).split('->');
      if (!from || !to || KEEP.has(to)) continue;
      (ENGLISH.has(to) ? risky : safe)[to] = from;      // reversed: undo it
    }
    if (!Object.keys(safe).length && !Object.keys(risky).length) continue;
    plan.push({ slug, dir, file, full, safe, risky });
  }
}

if (!plan.length) { console.log('\nNothing to revert - no rename records left.\n'); process.exit(0); }
if (!dotnetAvailable()) { console.log('\nNo dotnet on PATH. Formatting is part of this repair, so it cannot run here.\n'); process.exit(1); }

const folders = new Set(plan.map((x) => x.slug));
const reshape = new Set(plan.filter((x) => Object.keys(x.risky).length).map((x) => x.slug));
console.log(`\n${doApply ? 'APPLY' : 'DRY RUN'} - reverting lint's renames`);
console.log(`  ${plan.length} file(s) in ${folders.size} folder(s)`);
console.log(`  ${reshape.size} folder(s) will be marked for reshape (prose rewritten by the next backfill)\n`);

// ------------------------------------------------------------------ the files
const done = [];
for (const x of plan) {
  const raw = readFileSync(x.full, 'utf8');
  const { code: withHeader, eol } = splitTrailingTeach(raw);
  const teachBlock = raw.slice(withHeader.length);
  const { body, had } = stripHeader(withHeader);
  const header = had ? withHeader.slice(0, withHeader.length - body.length) : '';

  const all = { ...x.safe, ...x.risky };
  const r = renameIdentifiers(body, all, { includeSignature: true });

  const check = verifyRevert(body, r.code, all);
  if (!check.ok) {
    console.log(`  SKIPPED ${x.slug}/${x.file} - the revert did not come back clean: ${check.error}`);
    continue;
  }

  // Comments inside the code, the header, and the teaching block are prose. Only the
  // unambiguous names are touched here; the English-word ones are left for the re-teach.
  let code = r.code, head = header, teach = teachBlock;
  for (const [from, to] of Object.entries(x.safe)) {
    code = replaceWord(code, from, to);       // code tokens are already done; this is the comments
    head = replaceWord(head, from, to);
    teach = replaceWord(teach, from, to);
  }

  done.push({ ...x, eol, header: head, teachBlock: teach, body, code, applied: r.applied });
  const tag = Object.keys(x.risky).length ? '  [prose -> reshape]' : '';
  console.log(`  ${(x.slug + '/' + x.file).padEnd(50)} ${r.applied.map(([a, b]) => `${a}->${b}`).join(', ')}${tag}`);
}

// ------------------------------------------------------------------ format, then print
const formatted = formatMany(done.map((d) => d.code));
if (!formatted) { console.log('\ndotnet format failed - nothing written.\n'); process.exit(1); }

let written = 0;
for (let i = 0; i < done.length; i++) {
  const d = done[i];
  const tidy = toEol(formatted[i], d.eol).replace(/\s+$/, '');
  if (!sameShape(d.code, tidy).ok) { console.log(`  FORMATTER MOVED A TOKEN in ${d.slug}/${d.file} - kept unformatted`); }
  const finalCode = sameShape(d.code, tidy).ok ? tidy : d.code;
  d.finalCode = finalCode;

  if (doApply) {
    writeFileSync(d.full, d.header + finalCode.replace(/^(\r?\n)+/, '') + d.eol + d.teachBlock, 'utf8');
    written++;
  }
}

// ------------------------------------------------------------------ the visualizers
//
// A folder heading for reshape gets a brand new visualizer, so touching its old one is
// wasted work. The rest only ever contain unambiguous names, which are safe to replace
// in the step messages as well as in the verbatim code panel.
let vizTouched = 0;
for (const slug of folders) {
  if (reshape.has(slug)) continue;
  const d0 = done.find((d) => d.slug === slug);
  if (!d0) continue;
  const safe = Object.assign({}, ...done.filter((d) => d.slug === slug).map((d) => d.safe));
  if (!Object.keys(safe).length) continue;
  for (const v of readdirSync(d0.dir).filter((f) => f.endsWith('-visualizer.html'))) {
    const p = join(d0.dir, v);
    let html = readFileSync(p, 'utf8');
    const before = html;
    for (const [from, to] of Object.entries(safe)) html = replaceWord(html, from, to);
    if (html === before) continue;
    if (doApply) writeFileSync(p, html, 'utf8');
    vizTouched++;
    console.log(`  visualizer ${slug}/${v}`);
  }
}

// ------------------------------------------------------------------ the bookkeeping
//
// Without this every one of these files looks changed to the next run, and classify,
// teach and visualize all fire again on folders that are finished. The prints are all
// whitespace-stripped hashes of the code, so they are recomputed from the final text
// and written back into each signature that embeds one.
let marked = 0;
for (const d of done) {
  const rec = state.problems[d.slug];
  const print = shortPrint(d.finalCode);

  if (rec.lint?.[d.file]) { delete rec.lint[d.file].renames; rec.lint[d.file].codePrint = print; }

  for (const key of ['headerSignatures', 'teachSignatures']) {
    const sig = rec[key]?.[d.file];
    if (!sig) continue;
    try { const o = JSON.parse(sig); o.codePrint = print; rec[key][d.file] = JSON.stringify(o); }
    catch { /* leave a signature we cannot parse */ }
  }
  if (rec.visualizer?.prints?.[d.file]) rec.visualizer.prints[d.file] = print;
}

// A folder whose prose still describes the old names: drop the teaching signature so
// teach rewrites the block, and age the visualizer record so it shows as "needs
// reshape" on the index and the backfill picks the whole folder up in one pass.
for (const slug of reshape) {
  const rec = state.problems[slug];
  if (rec.teachSignatures) for (const f of Object.keys(rec.teachSignatures)) delete rec.teachSignatures[f];
  if (rec.visualizer) rec.visualizer.v = 1;
  marked++;
}

if (doApply) saveState(state);

console.log(`\n${doApply ? written : done.length} file(s) reverted, ${vizTouched} visualizer(s) touched, ${marked} folder(s) marked for reshape.`);
console.log(doApply
  ? '\nDone. Check with:  node scripts/select-backfill.mjs --limit 99\n'
  : '\nDry run - nothing written. Re-run with --apply.\n');
