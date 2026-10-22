#!/usr/bin/env node
/**
 * sheet.test.mjs - the Blueprint sheet the index is ordered by.
 *
 * Two kinds of check. The first runs on the real files in sheet/, and checks only
 * what the index depends on: that every note and alias points at a problem that
 * exists, and that every category has a group to be drawn in. It never checks a
 * count, because the sheet will grow, and a count here would stop the pipeline the
 * day a problem was added. The second runs on a small invented sheet, so the
 * matching rules are pinned without depending on the data.
 *
 * The pipeline runs every scripts/lib/*.test.mjs before any model call, so a note
 * or alias that points at nothing fails there, not in a published page.
 *
 *   node scripts/lib/sheet.test.mjs
 */

import { dirname, join } from 'node:path';
import { fileURLToPath } from 'node:url';
import { buildSheet, loadSheet, matchSheet } from './sheet.mjs';
import { GROUPS } from './patterns.mjs';

const REPO = join(dirname(fileURLToPath(import.meta.url)), '..', '..');
const real = loadSheet(REPO);

let pass = 0, fail = 0;
const is = (name, got, want) => {
  if (got === want) { pass++; console.log(`ok    ${name}`); }
  else { fail++; console.log(`FAIL  ${name}\n      got:  ${JSON.stringify(got)}\n      want: ${JSON.stringify(want)}`); }
};

// ---- the real sheet/ files ------------------------------------------------
is('sheet/ holds problems and pattern notes', real.count > 0 && real.noteCount > 0, true);

const orphanNotes = [...real.noteBySlug.keys()].filter((s) => !real.bySlug.has(s));
is('every pattern note is a problem on the sheet', orphanNotes.join(', '), '');

const orphanAliases = Object.entries(real.aliases)
  .filter(([, target]) => !real.bySlug.has(target))
  .map(([folder, target]) => `${folder} -> ${target}`);
is('every alias points at a problem on the sheet', orphanAliases.join(', '), '');

const shadowed = Object.keys(real.aliases)
  .filter((folder) => real.bySlug.has(folder) && real.aliases[folder] !== folder);
is('no alias hides a sheet problem of the same name', shadowed.join(', '), '');

const undrawn = [...real.categoryCounts.keys()].filter((c) => !GROUPS.includes(c));
is('every sheet category has a group the index can draw', undrawn.join(', '), '');

const positions = [...real.bySlug.values()].map((e) => e.n);
is('sheet positions are unique', new Set(positions).size === positions.length, true);

// ---- an invented sheet, so the rules are pinned without the data ----------
const s = buildSheet({
  problems: [
    { n: 1, category: 'Stack', title: 'Valid Parentheses', slug: 'valid-parentheses' },
    { n: 2, category: 'Stack', title: 'Min Stack', slug: 'min-stack' },
  ],
  notes: [{ slug: 'min-stack', pattern: 'Auxiliary stack', takeaway: 'track the minimum' }],
  aliases: { 'validate-parentheses': 'valid-parentheses' },
});

is('a sheet slug matches itself', matchSheet(s, 'min-stack').n, 2);
is('an alias matches the problem it names', matchSheet(s, 'validate-parentheses').sheetSlug, 'valid-parentheses');
is('an alias keeps the sheet title', matchSheet(s, 'validate-parentheses').title, 'Valid Parentheses');
is('a folder the sheet does not list matches nothing', matchSheet(s, 'quantum-rope'), null);
is('a pattern note comes with its problem', matchSheet(s, 'min-stack').note.pattern, 'Auxiliary stack');
is('a problem without a note has no note', matchSheet(s, 'valid-parentheses').note, null);
is('an Object prototype name does not match', matchSheet(s, 'constructor'), null);
is('categories are counted', s.categoryCounts.get('Stack'), 2);
is('a folder with no sheet/ directory is an empty sheet', loadSheet('/no/such/repo').count, 0);

console.log(`\n${pass} passed, ${fail} failed.`);
process.exit(fail ? 1 : 0);
