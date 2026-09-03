#!/usr/bin/env node
/**
 * resolve-slugs.mjs - check that every folder named on the command line exists.
 *
 * The "only" input on the workflow lets you point the pipeline at named folders.
 * A typo in that box is invisible to the scripts themselves: classify prints
 * `no folder with slug "binry-search"` and exits 0, teach then has nothing to
 * do, and the run finishes green having done nothing at all. So the names are
 * checked here, first, and a bad one stops the run before a model is called.
 *
 *   node scripts/resolve-slugs.mjs "binary-search, koko-eating-bananas"
 */

import { scanRepo, loadState } from './lib/scan.mjs';

const wanted = process.argv.slice(2).join(',').split(',').map((s) => s.trim()).filter(Boolean);

if (!wanted.length) { console.error('resolve-slugs: nothing named'); process.exit(1); }

const known = new Map(scanRepo(loadState()).map((p) => [p.slug, p]));

/** Levenshtein, capped - just enough to say "did you mean". */
function near(a, b) {
  const m = a.length, n = b.length;
  let prev = Array.from({ length: n + 1 }, (_, j) => j);
  for (let i = 1; i <= m; i++) {
    const cur = [i];
    for (let j = 1; j <= n; j++) {
      cur[j] = Math.min(prev[j] + 1, cur[j - 1] + 1, prev[j - 1] + (a[i - 1] === b[j - 1] ? 0 : 1));
    }
    prev = cur;
  }
  return prev[n];
}

const missing = [];
for (const slug of wanted) {
  const p = known.get(slug);
  if (p) {
    const files = [...p.curatedFiles, ...p.pending.map((s) => s.file)];
    console.log(`  ${slug.padEnd(32)} ${files.length} file(s): ${files.join(', ') || '(empty)'}`);
  } else {
    missing.push(slug);
  }
}

if (missing.length) {
  console.log('');
  for (const slug of missing) {
    const suggestion = [...known.keys()]
      .map((k) => [near(slug, k), k])
      .sort((a, b) => a[0] - b[0])
      .filter(([d]) => d <= Math.max(3, Math.floor(slug.length / 3)))[0];
    console.log(`::error::no folder named "${slug}"${suggestion ? ` - did you mean "${suggestion[1]}"?` : ''}`);
  }
  console.log(`\n${known.size} folders exist. Names are the folder names under "Data Structures & Algorithms".`);
  process.exit(1);
}

console.log(`\n${wanted.length} folder(s) named, all found. Nothing else will be touched this run.`);
