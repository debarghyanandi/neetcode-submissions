#!/usr/bin/env node
/**
 * indexpage-order.test.mjs - the order the index draws its rows in.
 *
 *   - a group is drawn only when a problem in it is solved
 *   - groups run in the Blueprint sheet's order
 *   - a problem the sheet lists sits in the sheet's group, found by the sheet, not the rules
 *   - inside a group the 📝 pattern notes come first, then the rest, each in sheet order
 *   - a solved folder the sheet does not list goes last in the group its rules choose
 *
 * The fixture is invented, so these pin the behaviour without depending on the real sheet.
 *
 *   node scripts/lib/indexpage-order.test.mjs
 */

import { buildIndexHtml } from './indexpage.mjs';
import { buildSheet } from './sheet.mjs';

let pass = 0, fail = 0;
const is = (name, got, want) => {
  const same = JSON.stringify(got) === JSON.stringify(want);
  if (same) { pass++; console.log(`ok    ${name}`); }
  else { fail++; console.log(`FAIL  ${name}\n      got:  ${JSON.stringify(got)}\n      want: ${JSON.stringify(want)}`); }
};

const problem = (slug) => ({
  slug, path: `Data Structures & Algorithms/${slug}`, dir: '/missing-order-fixture',
  curatedFiles: ['optimal.cs'], pending: [], hasVisualizer: false,
});
const state = { problems: {} };

const sheet = buildSheet({
  problems: [
    { n: 1, category: 'Arrays & Hashing', title: 'Plain A', slug: 'plain-a' },
    { n: 2, category: 'Arrays & Hashing', title: 'Note B', slug: 'note-b' },
    { n: 3, category: 'Arrays & Hashing', title: 'Plain C', slug: 'plain-c' },
    { n: 4, category: 'Arrays & Hashing', title: 'Note D', slug: 'note-d' },
    { n: 5, category: 'Intervals', title: 'Plain E', slug: 'plain-e' },
    { n: 6, category: 'Advanced Graphs', title: 'Aliased F', slug: 'aliased-target' },
  ],
  notes: [
    { slug: 'note-b', pattern: 'Hashing lookups', takeaway: 'store what you have seen' },
    { slug: 'note-d', pattern: 'Prefix sums', takeaway: 'running totals' },
  ],
  aliases: { 'aliased-folder': 'aliased-target' },
});

// Deliberately scrambled, so the test proves the order comes from the sheet.
const solved = ['plain-c', 'extra-interval-thing', 'note-d', 'plain-a', 'plain-e',
  'note-b', 'aliased-folder', 'zz-unknown-thing'];
const problems = solved.map(problem);

const web = 'https://github.com/example/solutions/blob/main';
const pages = 'https://example.github.io/solutions';
const html = buildIndexHtml(problems, state, { web, pages, sheet });

const sections = [...html.matchAll(/<section class="group" id="[^"]+" data-group="([^"]+)">([\s\S]*?)<\/section>/g)]
  .map((m) => ({
    group: m[1].replace(/&amp;/g, '&'),
    rows: [...m[2].matchAll(/data-problem="[^"]*\/([^"/]+)"/g)].map((r) => r[1]),
  }));
const rowsOf = (group) => sections.find((s) => s.group === group)?.rows ?? null;

is('groups run in the sheet\'s order, and only solved groups appear',
  sections.map((s) => s.group), ['Arrays & Hashing', 'Advanced Graphs', 'Intervals', 'Other']);
is('a group with nothing solved is not drawn', html.includes('data-group="Greedy"'), false);

is('inside a group the pattern notes come first, then the rest in sheet order',
  rowsOf('Arrays & Hashing'), ['note-b', 'note-d', 'plain-a', 'plain-c']);
is('a problem the sheet lists sits in its sheet group, after the sheet items',
  rowsOf('Intervals'), ['plain-e', 'extra-interval-thing']);
is('an alias folder is placed by the sheet, not the rules',
  rowsOf('Advanced Graphs'), ['aliased-folder']);
is('an unlisted problem goes to its rule\'s group, at the bottom of it',
  rowsOf('Other'), ['zz-unknown-thing']);

is('each pattern note carries the badge', (html.match(/class="note-badge"/g) || []).length, 2);
is('the badge names the note\'s pattern', html.includes('📝 Pattern note · Hashing lookups'), true);
is('the sheet number is shown', html.includes('>#2</span>'), true);
is('the search text of a note includes its takeaway',
  /data-hay="([^"]*)"[^>]*data-problem="[^"]*\/note-d"/.exec(html)?.[1].includes('running totals') ?? false, true);

is('the left roadmap has a link per drawn group', (html.match(/class="rail-link"/g) || []).length, 4);
is('the right note shows notes solved out of the sheet total',
  html.includes('<p class="tally"><b>2</b> / 2</p>'), true);
is('the right note shows sheet problems solved out of the sheet total',
  html.includes('<p class="tally"><b>6</b> / 6</p>'), true);

// With no sheet at all the page still builds: every solved problem is filed by the rules.
const bare = buildIndexHtml(problems, state, { web, pages });
is('no sheet/ still renders, with no note badges', (bare.match(/class="note-badge"/g) || []).length, 0);

console.log(`\n${pass} passed, ${fail} failed.`);
process.exit(fail ? 1 : 0);
