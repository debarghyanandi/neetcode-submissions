#!/usr/bin/env node
/**
 * teach.test.mjs - reading the teach model's plain-text reply.
 *
 * The reply used to be forced into shape by --json-schema. It is now plain text with "@@" markers,
 * so this parser is the only thing enforcing the fixed sections and their order. A reply it
 * accepts wrongly becomes a teaching block with a hole in it; one it refuses wrongly costs a retry.
 *
 *   node scripts/lib/teach.test.mjs
 */
import { parseTeachText, toSections, buildTeachingBlock } from './teach.mjs';

let pass = 0, fail = 0;
const ok = (name, cond, extra = '') => { if (cond) { pass++; console.log(`ok    ${name}`); } else { fail++; console.log(`FAIL  ${name}${extra ? `\n      ${extra}` : ''}`); } };

const GOOD = `Sure, here it is.
@@ PATTERN
Post-order DFS - height up, diameter in a field
@@ VARIABLES
diameter  the best edge count seen so far
node      the subtree root being measured
@@ WHY THIS PATTERN
The path bends at one node.
@@ BRUTE FORCE
Height from every node is O(n^2).
@@ INVARIANT
Height returns the node count below.
@@ KEY DETAIL: NODES UP, EDGES OUT
left + right counts edges.
@@ WATCH OUT
diameter is never reset.
@@ FOLLOW-UP
Q: Without recursion?
A: Explicit stack; more code.
Q: Return the path?
A: Track the bend node;
extra state per frame.
@@ TRIGGER
Best value over all nodes from both subtrees.
@@ C# NOTE
Pass the best by ref.`;

const g = parseTeachText(GOOD);
ok('a well-formed reply is read, text before the first marker ignored', g.out && !g.errors.length, g.errors.join('; '));
ok('the key detail keeps its title', g.out?.keyDetails[0]?.title === 'NODES UP, EDGES OUT');
ok('follow-ups pair each Q with its A, joining a wrapped answer', g.out?.followUps.length === 2 && g.out.followUps[1].answer === 'Track the bend node; extra state per frame.');
ok('CRLF line endings read the same', !!parseTeachText(GOOD.replace(/\n/g, '\r\n')).out);
ok('no key detail at all is fine', !!parseTeachText(GOOD.replace(/@@ KEY DETAIL[^\n]*\n[^\n]*\n/, '')).out);

const block = g.out ? buildTeachingBlock({ pattern: g.out.pattern, sections: toSections(g.out, { status: 'Optimal' }) }, { source: 's', status: 'Optimal', time: 'O(n)', space: 'O(n)' }) : '';
const heads = block.split('\n').filter((l) => /^[A-Z#][A-Z#, -]+$/.test(l) && !/^=+$/.test(l));
ok('the block comes out in the fixed order',
  JSON.stringify(heads) === JSON.stringify(['VARIABLES', 'WHY THIS PATTERN', 'BRUTE FORCE', 'INVARIANT', 'NODES UP, EDGES OUT', 'WATCH OUT', 'FOLLOW-UP AN INTERVIEWER WILL ASK', 'TRIGGER', 'C# NOTE', 'COMPLEXITY']),
  JSON.stringify(heads));

// The glossary is why lint stopped renaming: a name that needs explaining gets a line
// here instead of being rewritten in the author's code.
ok('the variables glossary is read, one line per name',
  g.out?.variables === 'diameter  the best edge count seen so far\nnode      the subtree root being measured', JSON.stringify(g.out?.variables));
ok('and its alignment survives into the block', block.includes('diameter  the best edge count seen so far'));
ok('it comes before any section that uses the names',
  block.indexOf('VARIABLES') < block.indexOf('WHY THIS PATTERN'));

const bad = (name, text, mention) => {
  const r = parseTeachText(text);
  ok(name, !r.out && r.errors.some((e) => e.includes(mention)), r.errors.join('; ') || 'accepted');
};
bad('a missing section is refused', GOOD.replace(/@@ INVARIANT\n[^\n]*\n/, ''), 'missing "@@ INVARIANT"');
bad('and so is a reply with no glossary', GOOD.replace(/@@ VARIABLES\n[^\n]*\n[^\n]*\n/, ''), 'missing "@@ VARIABLES"');
bad('an empty section is refused', GOOD.replace('diameter is never reset.', ''), 'is empty');
bad('one follow-up is not enough', GOOD.replace(/Q: Return the path\?\nA: Track the bend node;\nextra state per frame.\n/, ''), 'need 2 to 4');
bad('three key details are too many', GOOD.replace('@@ WATCH OUT', '@@ KEY DETAIL: A\nx\n@@ KEY DETAIL: B\ny\n@@ WATCH OUT'), 'at most 2');
bad('sections out of order are refused', GOOD.replace(/(@@ TRIGGER\n[^\n]*\n)(@@ C# NOTE\n[^\n]*)/, '$2\n$1').replace(/\n$/, ''), 'out of order');
bad('an invented marker is refused', GOOD.replace('@@ WATCH OUT', '@@ WATCH OUT\nx\n@@ SUMMARY'), 'unknown marker');
bad('a JSON reply is refused, not half-read', JSON.stringify({ pattern: 'x', whyThisPattern: 'y' }), 'missing "@@ PATTERN"');

console.log(`\n${pass} passed, ${fail} failed.`);
process.exit(fail ? 1 : 0);
