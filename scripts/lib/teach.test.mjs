#!/usr/bin/env node
/**
 * teach.test.mjs - reading the teach model's plain-text reply, and rendering the note.
 *
 * The reply is plain text with "@@" markers, so this parser is the only thing enforcing the
 * fixed sections, their order, and which sections a file's role allows. A reply it accepts
 * wrongly becomes a note with a hole in it; one it refuses wrongly costs a retry.
 *
 *   node scripts/lib/teach.test.mjs
 */
import { parseTeachText, buildTeachingBlock, lengthProblem, roleFor, BLOCK_LIMIT, TEACH_INSTRUCTIONS } from './teach.mjs';

let pass = 0, fail = 0;
const ok = (name, cond, extra = '') => { if (cond) { pass++; console.log(`ok    ${name}`); } else { fail++; console.log(`FAIL  ${name}${extra ? `\n      ${extra}` : ''}`); } };

const FULL = `Sure, here it is.
@@ PROBLEM
Bars of width 1 with heights height[i]. Return
how much rain water is trapped. E.g. [4,2,0,3,2,5] -> 9.
@@ PATTERN
Two Pointers (converging) + running max
@@ IDEA
Water on a bar = min(left wall, right wall) - its height.
Move the lower side; its own max already decides its water.
@@ EXAMPLE
height = [4,2,0,3,2,5]
water per bar = [0,2,4,1,2,0] -> 9
@@ COMPLEXITY
Time: each index is visited once by l or r
Space: only four integers
@@ PATH TO OPTIMAL
Brute force - O(n^2) - scan both sides per bar
Prefix / suffix max arrays - O(n) space - suboptimal.cs
Two pointers - O(1) space - this file
@@ KEYWORDS
two pointers, prefix max, suffix max, monotonic stack
@@ WATCH OUT
- Moving the taller side is the classic bug: its water depends on a wall you have not seen yet.
- lMax must update only when the bar is not below it.
@@ FOLLOW-UP
Q: Solve it with a stack?
A: Monotonic decreasing stack, water in layers;
O(n) space.
Q: A 2-D grid?
A: Min-heap from the border; O(mn log mn).
Q: Why is moving the lower side safe?
A: The far side already has a wall at least as tall.
@@ TRIGGER
The answer at each index depends on the max to its left AND right.`;

const SHORT = `@@ PROBLEM
Bars of width 1 with heights height[i]. Return the trapped water.
@@ PATTERN
Prefix max + suffix max arrays
@@ IDEA
lMax[i] and rMax[i] hold the tallest wall on each side.
@@ EXAMPLE
height = [4,2,0,3,2,5] -> 9
@@ COMPLEXITY
Time: three linear passes
Space: two arrays of size n
@@ WATCH OUT
- Empty input: lMax[0] = height[0] throws.`;

// ---- roles ----------------------------------------------------------------
ok('optimal.cs gets the full note', roleFor('optimal.cs') === 'full');
ok('a variant gets the short one', roleFor('optimal-variant.cs') === 'short');
ok('and so does a suboptimal file', roleFor('suboptimal-2.cs') === 'short');

// ---- parsing --------------------------------------------------------------
const f = parseTeachText(FULL, 'full');
ok('a well-formed full reply is read, text before the first marker ignored', f.out && !f.errors.length, f.errors.join('; '));
ok('follow-ups pair each Q with its A, joining a wrapped answer',
  f.out?.followUps.length === 3 && f.out.followUps[0].answer === 'Monotonic decreasing stack, water in layers; O(n) space.', JSON.stringify(f.out?.followUps));
ok('COMPLEXITY is read as two reasons', f.out?.complexity.time === 'each index is visited once by l or r' && f.out?.complexity.space === 'only four integers');
ok('CRLF line endings read the same', !!parseTeachText(FULL.replace(/\n/g, '\r\n'), 'full').out);
const s = parseTeachText(SHORT, 'short');
ok('a well-formed short reply is read', s.out && !s.errors.length, s.errors.join('; '));

const bad = (name, text, role, mention) => {
  const r = parseTeachText(text, role);
  ok(name, !r.out && r.errors.some((e) => e.includes(mention)), r.errors.join('; ') || 'accepted');
};
bad('a missing section is refused', FULL.replace(/@@ TRIGGER\n[^\n]*/, ''), 'full', 'missing "@@ TRIGGER"');
bad('the full note needs its PROBLEM', FULL.replace(/@@ PROBLEM\n[^\n]*\n/, ''), 'full', 'missing "@@ PROBLEM"');
bad('a short note may not grow follow-ups back', SHORT + '\n@@ FOLLOW-UP\nQ: x?\nA: y.\nQ: z?\nA: w.', 'short', 'does not belong');
bad('nor a keywords line', SHORT.replace('@@ WATCH OUT', '@@ KEYWORDS\nx\n@@ WATCH OUT'), 'short', 'does not belong');
bad('the short note needs its PROBLEM too - a file must stand alone', SHORT.replace(/@@ PROBLEM\n[^\n]*\n/, ''), 'short', 'missing "@@ PROBLEM"');
bad('an empty section is refused', FULL.replace('Space: only four integers', '').replace('Time: each index is visited once by l or r', ''), 'full', 'is empty');
bad('COMPLEXITY without its two lines is refused', FULL.replace('Space: only four integers', 'and a bit of space'), 'full', '"Space: <reason>"');
bad('two follow-ups are not enough', FULL.replace(/Q: A 2-D grid\?\nA: [^\n]*\n/, ''), 'full', 'need 3 or 4');
bad('sections out of order are refused', FULL.replace(/(@@ KEYWORDS\n[^\n]*\n)(@@ WATCH OUT)/, '$2').replace('@@ TRIGGER', '@@ KEYWORDS\ntwo pointers\n@@ TRIGGER'), 'full', 'out of order');
bad('an invented marker is refused', FULL.replace('@@ WATCH OUT', '@@ SUMMARY\nx\n@@ WATCH OUT'), 'full', 'unknown marker');
bad('the old C# NOTE section is gone', FULL + '\n@@ C# NOTE\nuse long', 'full', 'unknown marker');
bad('a JSON reply is refused, not half-read', JSON.stringify({ pattern: 'x' }), 'full', 'missing "@@ PATTERN"');

// ---- rendering ------------------------------------------------------------
const ctxFull = { role: 'full', time: 'O(n)', space: 'O(1)' };
const block = buildTeachingBlock(f.out, ctxFull);
const heads = block.split('\n').filter((l) => /^[A-Z][A-Z -]+$/.test(l));
ok('the full note comes out in the fixed order',
  JSON.stringify(heads) === JSON.stringify(['IDEA', 'EXAMPLE', 'COMPLEXITY', 'PATH TO OPTIMAL', 'KEYWORDS', 'WATCH OUT', 'FOLLOW-UP AN INTERVIEWER WILL ASK', 'TRIGGER']), JSON.stringify(heads));
ok('PROBLEM and PATTERN sit in the banner', /^ PROBLEM : Bars of width 1/m.test(block) && /^ PATTERN : Two Pointers/m.test(block));
ok('a line break the model left inside PROBLEM does not survive', /heights height\[i\]\. Return how much/.test(block.replace(/\n {12}/g, ' ')) && !/Return\n/.test(block), block.split('\n').slice(0, 6).join('\n'));
ok('the banner no longer carries SOURCE or STATUS', !/SOURCE|STATUS/.test(block));
ok('COMPLEXITY prints the recorded values beside the reasons',
  /^ {2}Time {2}O\(n\) +each index is visited once by l or r$/m.test(block) && /^ {2}Space O\(1\) +only four integers$/m.test(block), block);
ok('the example keeps its lines', block.includes('  height = [4,2,0,3,2,5]\n  water per bar = [0,2,4,1,2,0] -> 9'));
ok('a watch-out point wraps under its text, not its dash', /\n  - Moving the taller side[^\n]*\n {4}\S/.test(block), block);
ok('no line runs past the rule', block.split('\n').every((l) => l.length <= 80), block.split('\n').filter((l) => l.length > 80).join('\n'));
ok('the block still ends in a rule and */', block.endsWith('='.repeat(80) + '\n*/'));
ok('patterns.mjs can still read the PATTERN line', /^[^A-Za-z\n]*PATTERN\s*:\s*(.+)$/m.exec(block)?.[1] === 'Two Pointers (converging) + running max');
ok('the sample full note fits its limit', lengthProblem(block, 'full') === null, `${block.split('\n').length} lines`);

const sblock = buildTeachingBlock(s.out, { role: 'short', time: 'O(n)', space: 'O(n)' });
const sheads = sblock.split('\n').filter((l) => /^[A-Z][A-Z -]+$/.test(l));
ok('the short note has only its own sections', JSON.stringify(sheads) === JSON.stringify(['IDEA', 'EXAMPLE', 'COMPLEXITY', 'WATCH OUT']), JSON.stringify(sheads));
ok('and it restates the problem, so the file stands alone', /^ PROBLEM : Bars of width 1/m.test(sblock));
ok('the sample short note fits its limit', lengthProblem(sblock, 'short') === null, `${sblock.split('\n').length} lines`);

const long = buildTeachingBlock({ ...s.out, idea: Array(40).fill('A sentence that goes on.').join('\n') }, { role: 'short', time: 'O(n)', space: 'O(n)' });
ok('an over-long note is reported with its size and the limit', (lengthProblem(long, 'short') ?? '').includes(`limit is ${BLOCK_LIMIT.short}`));
ok('a closing comment in the prose cannot end the block early',
  !buildTeachingBlock({ ...s.out, idea: 'x */ y' }, { role: 'short', time: 'O(1)', space: 'O(1)' }).slice(0, -3).includes('*/'));

// ---- the prompt -------------------------------------------------------------
const pFull = TEACH_INSTRUCTIONS({ role: 'full', slug: 'trapping-rain-water', file: 'optimal.cs', siblings: [{ name: 'optimal.cs', time: 'O(n)', space: 'O(1)' }, { name: 'suboptimal.cs', time: 'O(n)', space: 'O(n)' }] });
const pShort = TEACH_INSTRUCTIONS({ role: 'short', slug: 'trapping-rain-water', file: 'suboptimal.cs', siblings: [] });
ok('the prompt names the problem and lists the siblings', pFull.includes('"trapping-rain-water"') && pFull.includes('suboptimal.cs') && pFull.includes('O(n) time / O(n) space'));
ok('the full prompt asks for every full section', ['PROBLEM', 'PATH TO OPTIMAL', 'KEYWORDS', 'FOLLOW-UP', 'TRIGGER'].every((m) => pFull.includes(`@@ ${m}`)));
ok('the short prompt asks for the problem', pShort.includes('@@ PROBLEM'));
ok('but for none of the whole-problem sections', ['PATH TO OPTIMAL', 'KEYWORDS', 'FOLLOW-UP', 'TRIGGER'].every((m) => !pShort.includes(`@@ ${m}`)));
ok('the limits are about 30% under the 73-line median they replace', BLOCK_LIMIT.full <= 52 && BLOCK_LIMIT.full >= 48);

console.log(`\n${pass} passed, ${fail} failed.`);
process.exit(fail ? 1 : 0);
