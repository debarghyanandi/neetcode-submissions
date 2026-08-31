#!/usr/bin/env node
/**
 * detect.mjs - READ ONLY. Writes nothing, commits nothing, calls no model.
 *
 * Answers one question: which problem folders have raw NeetCode submissions
 * that have not been processed yet?
 *
 * One code path. The push run and the nightly run differ only in exclusions:
 *   push : --exclude-changed-since <before-sha>   skip what this push touched
 *   cron : (no flags)                             drain everything
 *
 *   node scripts/detect.mjs
 *   node scripts/detect.mjs --exclude two-integer-sum,minimum-stack
 *   node scripts/detect.mjs --exclude-changed-since <sha> --json
 */

import { appendFileSync, readFileSync } from 'node:fs';
import { basename } from 'node:path';
import { fileURLToPath } from 'node:url';
import { REPO, loadState, scanRepo, pendingOnly, foldersChangedSince } from './lib/scan.mjs';

function parseArgs(argv) {
  const opts = { exclude: new Set(), changedSince: null, json: false };
  for (let i = 0; i < argv.length; i++) {
    const a = argv[i];
    if (a === '--exclude') {
      String(argv[++i] ?? '').split(',').map((s) => s.trim()).filter(Boolean)
        .forEach((s) => opts.exclude.add(basename(s)));
    } else if (a === '--exclude-changed-since') {
      opts.changedSince = argv[++i] ?? null;
    } else if (a === '--json') {
      opts.json = true;
    } else if (a === '--help' || a === '-h') {
      console.log(readFileSync(fileURLToPath(import.meta.url), 'utf8').split('*/')[0]);
      process.exit(0);
    } else {
      console.error(`detect: unknown argument ${a}`);
      process.exit(2);
    }
  }
  return opts;
}

const opts = parseArgs(process.argv.slice(2));
for (const slug of foldersChangedSince(opts.changedSince)) opts.exclude.add(slug);

const all = pendingOnly(scanRepo(loadState()));
const excluded = all.filter((p) => opts.exclude.has(p.slug));
const selected = all.filter((p) => !opts.exclude.has(p.slug));

const slim = (p) => ({
  topic: p.topic, slug: p.slug, path: p.path,
  curatedFiles: p.curatedFiles, hasVisualizer: p.hasVisualizer, pending: p.pending,
});

const report = {
  scannedAt: new Date().toISOString(),
  repo: REPO,
  excludedSlugs: [...opts.exclude],
  counts: { pendingTotal: all.length, excluded: excluded.length, selected: selected.length },
  selected: selected.map(slim),
  excluded: excluded.map((p) => ({ path: p.path, pending: p.pending.map((s) => s.file) })),
};

if (opts.json) {
  console.log(JSON.stringify(report, null, 2));
} else {
  console.log(`\nscanned  ${REPO}`);
  console.log(`excluded ${report.excludedSlugs.length ? report.excludedSlugs.join(', ') : '(nothing)'}`);
  console.log(`pending  ${all.length} folder(s) -> ${selected.length} selected, ${excluded.length} held back\n`);
  if (selected.length === 0) console.log('  nothing to do.\n');
  for (const p of selected) {
    console.log(`  ${p.path}`);
    console.log(`      ${p.curatedFiles.length ? `already curated: ${p.curatedFiles.join(', ')}` : 'never curated'}${p.hasVisualizer ? ' + visualizer.html' : ''}`);
    for (const s of p.pending) {
      console.log(`      ${s.file.padEnd(18)} ${String(s.bytes).padStart(6)}B  ${s.fingerprint}${s.duplicateOfCurated ? '   [DUPLICATE of curated code]' : ''}`);
    }
    console.log('');
  }
  for (const p of report.excluded) console.log(`  (held back) ${p.path}: ${p.pending.join(', ')}`);
  console.log('');
}

if (process.env.GITHUB_OUTPUT) {
  appendFileSync(process.env.GITHUB_OUTPUT, `count=${selected.length}\n`);
  appendFileSync(process.env.GITHUB_OUTPUT, `slugs=${selected.map((p) => p.slug).join(',')}\n`);
}
