#!/usr/bin/env node
/**
 * lint.mjs - format the solution code. Nothing else.
 *
 * THIS STEP CALLS NO MODEL. It used to: Haiku was asked to tidy spacing AND rename
 * local variables, and sameShape() checked it had changed nothing else. The renaming
 * is gone, deliberately, and the evidence for removing it was its own record - 92
 * renames across 72 files, read back out of state.json:
 *
 *   - 43 of the 92 (47%) overwrote a standard DSA name: n, l, r, m, dp, res, q,
 *     curr, prev2, node, a, b. That is the vocabulary interviewers speak and every
 *     editorial uses. `dp` is the clearest signal in a dynamic-programming solution
 *     and it was being replaced with things like `maxAmount` and `waysToStep`.
 *   - 7 were pure noise: curr -> current, res -> result, q -> queue, n -> length.
 *   - 3 of 22 multi-file folders ended up with one variable under two names, which
 *     is worst exactly where it hurts most. find-target-in-rotated-sorted-array got
 *     l -> left in one file and l -> low in the other; is-anagram got s -> original
 *     and s -> first. Revising a folder meant holding two vocabularies for one idea.
 *
 * And it contradicted the rule directly above it in the old prompt: comments may
 * never be deleted, because they are the author's notes for later. The variables
 * those notes are about were being rewritten anyway.
 *
 * These files are a record of what was submitted to NeetCode. Where a name really is
 * opaque, the fix is the VARIABLES section in the teaching block - a glossary beside
 * the code, additive and reversible - not a rewrite of the code, which is neither.
 *
 * So what is left is formatting, which is a function:
 *
 *   dotnet format whitespace --folder   deterministic, free, about two seconds,
 *                                       configured by .editorconfig, and incapable
 *                                       of changing a token (see lib/format.mjs).
 *
 * The formatter is still not trusted: every result goes through sameShape() before
 * it is written, so a formatter that moves a token is refused exactly as a model
 * that did would have been, and the original is kept.
 *
 * Lint still runs FIRST, before classify. Formatting is invisible to every print in
 * state.json - they are all computed with whitespace stripped - so the ordering no
 * longer matters for correctness, but a file should be in its final shape before
 * anything describes it.
 *
 *   node scripts/lint.mjs --slug two-integer-sum          # dry run, shows the diff
 *   node scripts/lint.mjs --slug two-integer-sum --apply
 *   node scripts/lint.mjs --apply --limit 3               # folders with raw submissions
 *   node scripts/lint.mjs --apply --backfill --limit 3    # every folder not yet formatted
 */

import { readFileSync, writeFileSync, appendFileSync } from 'node:fs';
import { join } from 'node:path';
import { loadState, saveState, scanRepo, pendingOnly, foldersChangedSince } from './lib/scan.mjs';
import { stripHeader } from './lib/header.mjs';
import { splitTrailingTeach } from './lib/teach.mjs';
import { sameShape } from './lib/csharp.mjs';
import { shortPrint } from './lib/normalise.mjs';
import { LINT_FORMAT, lintWanted } from './lib/lint-rules.mjs';
import { report, group, endGroup } from './lib/report.mjs';
import { formatMany, dotnetAvailable, toEol } from './lib/format.mjs';

const argv = process.argv.slice(2);
const arg = (n, d = null) => (argv.includes(n) ? argv[argv.indexOf(n) + 1] : d);
const has = (n) => argv.includes(n);

const onlyRaw = arg('--slug');
const only = onlyRaw ? onlyRaw.split(',').map((s) => s.trim()).filter(Boolean) : null;
const limit = Number(arg('--limit', '0')) || 0;
const doApply = has('--apply');
const backfill = has('--backfill');
// --force: format again even where the record says it is already done. Cheap, since
// nothing here is paid for; it exists so a changed .editorconfig can be rolled out.
const force = has('--force');

// ---------------------------------------------------------------- run

const state = loadState();
const everything = scanRepo(state);

const needsLint = (p) => p.curatedFiles.concat(p.pending.map((s) => s.file))
  .some((f) => lintWanted(state.problems[p.slug]?.lint?.[f]));

let targets;
if (only) {
  targets = everything.filter((p) => only.includes(p.slug));
} else if (backfill) {
  const all = everything.filter((p) => p.curatedFiles.length || p.pending.length);
  targets = force ? all : all.filter(needsLint);
  console.log(`\nbackfill: ${all.length} folder(s), ${all.length - targets.length} already formatted, ${targets.length} remaining`);
} else {
  targets = pendingOnly(everything);
}
// Same exclusion contract as detect and classify: on a push run, leave alone
// the folder that was just pushed to.
const exclude = new Set();
for (const v of argv.flatMap((a, i) => (a === '--exclude' ? [argv[i + 1]] : [])))
  String(v ?? '').split(',').map((x) => x.trim()).filter(Boolean).forEach((x) => exclude.add(x));
for (const slug of foldersChangedSince(arg('--exclude-changed-since'))) exclude.add(slug);
if (exclude.size) {
  const before = targets.length;
  targets = targets.filter((p) => !exclude.has(p.slug));
  console.log(`excluding ${[...exclude].join(', ')} (${before - targets.length} held back)`);
}

if (limit) targets = targets.slice(0, limit);

if (!targets.length) { console.log('\nNothing to format.\n'); process.exit(0); }

console.log(`\n${doApply ? 'APPLY' : 'DRY RUN'} - format, ${targets.length} folder(s)`);
if (!dotnetAvailable()) {
  console.log('  NO DOTNET ON PATH - nothing can be formatted\n');
  console.log('::warning::dotnet is not available; lint did nothing');
  process.exit(0);
}
console.log('  dotnet format whitespace, rules in .editorconfig · no model call\n');

let changed = 0, clean = 0, refused = 0;
const touchedSlugs = new Set();

for (const p of targets) {
  group(p.path);
  // Superseded submissions are deleted by classify, which runs next. Formatting one
  // leaves a record keyed to a filename nothing will ever re-key.
  const files = [...p.curatedFiles, ...p.pending.filter((s) => !s.supersededBy).map((s) => s.file)];
  for (const s of p.pending.filter((x) => x.supersededBy)) {
    console.log(`  ${s.file.padEnd(22)} skipped - superseded by ${s.supersededBy}`);
  }

  // Read the folder, then format the whole of it in ONE dotnet call - it pays a few
  // seconds of SDK start-up per invocation, which is worth batching away.
  const prepared = files.map((file) => {
    const full = join(p.dir, file);
    const raw = readFileSync(full, 'utf8');
    const { code: withHeader, eol } = splitTrailingTeach(raw);
    const teachBlock = raw.slice(withHeader.length);
    const { body, had } = stripHeader(withHeader);
    const header = had ? withHeader.slice(0, withHeader.length - body.length) : '';
    const rec = state.problems[p.slug]?.lint?.[file];
    return { file, full, eol, teachBlock, header, body, rec, wanted: force || lintWanted(rec) };
  });

  const todo = prepared.filter((x) => x.wanted);
  for (const x of prepared.filter((x) => !x.wanted)) {
    console.log(`  ${x.file.padEnd(22)} already formatted`);
    report('lint', p.slug, 'skipped', `${x.file}: already formatted`);
    clean++;
  }
  if (!todo.length) { endGroup(); continue; }

  const formatted = formatMany(todo.map((x) => x.body));
  if (!formatted) {
    console.log('  dotnet format failed - nothing written');
    report('lint', p.slug, 'failed', 'dotnet format could not be run');
    endGroup();
    continue;
  }

  for (let i = 0; i < todo.length; i++) {
    const x = todo[i];
    const tidy = toEol(formatted[i], x.eol).replace(/\s+$/, '');

    // The formatter is not trusted. Whitespace is all it may change, and sameShape()
    // is exactly the thing that knows the difference between that and an edit.
    const check = sameShape(x.body, tidy);
    if (!check.ok) {
      console.log(`  ${x.file.padEnd(22)} REFUSED - the formatter changed code, not just spacing: ${check.errors[0]}`);
      console.log(`::warning::dotnet format altered tokens in ${p.slug}/${x.file}; the original was kept`);
      report('lint', p.slug, 'refused', `${x.file}: dotnet format changed more than whitespace - ${check.errors[0]}`);
      refused++;
      continue;
    }

    const same = tidy.trim() === x.body.trim();
    console.log(`  ${x.file.padEnd(22)} ${same ? 'nothing to change' : 'reformatted'}`);
    if (!same && !doApply) {
      console.log(tidy.split('\n').slice(0, 12).map((l) => '      | ' + l).join('\n'));
    }

    if (doApply) {
      writeFileSync(x.full, x.header + tidy.replace(/^(\r?\n)+/, '') + x.eol + x.teachBlock, 'utf8');
      const prec = state.problems[p.slug] ?? (state.problems[p.slug] = {});
      (prec.lint ?? (prec.lint = {}))[x.file] = { version: LINT_FORMAT, codePrint: shortPrint(tidy) };
      report('lint', p.slug, 'ok', `${x.file}: ${same ? 'nothing to change' : 'reformatted'}`);
      if (!same) { changed++; touchedSlugs.add(p.slug); }
      else clean++;
    } else if (!same) changed++;
  }
  endGroup();
}

if (doApply) saveState(state);
console.log(`\n${changed} file(s) reformatted, ${clean} already correct${refused ? `, ${refused} refused` : ''}.\n`);
if (process.env.GITHUB_OUTPUT) {
  appendFileSync(process.env.GITHUB_OUTPUT, `changed=${changed}\n`);
  appendFileSync(process.env.GITHUB_OUTPUT, `slugs=${[...touchedSlugs].join(',')}\n`);
}
// A refusal means the formatter is no longer whitespace-only, which invalidates the
// whole design of this step. Fail loudly; the workflow guard should have caught it.
process.exit(refused ? 1 : 0);
