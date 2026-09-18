#!/usr/bin/env node
/**
 * format.test.mjs - the deterministic formatting step.
 *
 * Most of what matters here is checked against the real `dotnet format`, so these
 * tests SKIP when the SDK is absent rather than pretend. What they do assert always:
 * line endings survive, and a missing formatter degrades to null instead of throwing
 * or returning half an answer.
 *
 * The load-bearing test is the last one. Spacing is only safe to hand to an external
 * tool because sameShape() checks its work - a formatter that changes a token is
 * refused exactly like a model that does. If that ever stops holding, lint would
 * start refusing every file it touched.
 *
 *   node scripts/lib/format.test.mjs
 */

import { formatMany, dotnetAvailable, toEol } from './format.mjs';
import { sameShape } from './csharp.mjs';

let pass = 0, fail = 0, skip = 0;
const is = (name, got, want) => {
  if (got === want) { pass++; console.log(`ok    ${name}`); }
  else { fail++; console.log(`FAIL  ${name}\n      got ${JSON.stringify(got)}, wanted ${JSON.stringify(want)}`); }
};
const skipped = (name, why) => { skip++; console.log(`skip  ${name}  (${why})`); };

// ---- line endings -------------------------------------------------------
is('lf stays lf', toEol('a\nb', '\n'), 'a\nb');
is('lf becomes crlf', toEol('a\nb', '\r\n'), 'a\r\nb');
is('crlf becomes lf', toEol('a\r\nb', '\n'), 'a\nb');
is('crlf stays crlf, not doubled', toEol('a\r\nb', '\r\n'), 'a\r\nb');
is('mixed endings are normalised to one', toEol('a\r\nb\nc', '\r\n'), 'a\r\nb\r\nc');

// ---- nothing to do ------------------------------------------------------
is('an empty batch is an empty batch, with no dotnet call', JSON.stringify(formatMany([])), '[]');

// The file that broke house-robber. Braceless `if`, K&R braces, no spaces around
// the operators in the index expressions.
const UGLY = [
  'public class Solution {',
  '    public int Rob(int[] nums) {',
  '        int n = nums.Length;',
  '        if(n == 1)',
  '        return nums[0];',
  '        int [] dp = new int [n+1];',
  '        for(int i = 2; i < n; i++){',
  '            dp[i] = Math.Max(nums[i] + dp[i-2], dp[i-1]);',
  '        }',
  '        return dp[n-1];',
  '    }',
  '}',
].join('\n');

if (!dotnetAvailable()) {
  skipped('a batch degrades to null with no dotnet', 'no dotnet on PATH');
  is('and says so rather than throwing', formatMany([UGLY]), null);
} else {
  const out = formatMany([UGLY]);
  is('a batch comes back the same length', out?.length, 1);

  const got = out[0];
  // THE one that matters. Everything else about this design rests on it.
  is('formatting changes not one token', sameShape(UGLY, got).ok, true);
  is('and therefore reports no renames', sameShape(UGLY, got).renames.length, 0);

  // csharp_prefer_braces is a STYLE rule, and `whitespace` does not run style rules.
  // If this ever fails, the SDK has changed and lint must stop using it.
  is('the braceless if is still braceless', /if \(n == 1\)\s*\r?\n\s*return nums\[0\];/.test(got), true);

  // What it SHOULD have done.
  is('K&R braces moved to Allman', /class Solution\s*\r?\n\{/.test(got), true);
  is('the control keyword got its space', got.includes('if (n == 1)'), true);
  is('the array type lost its stray spaces', got.includes('int[] dp = new int[n + 1]'), true);
  is('index arithmetic got spaced', got.includes('dp[i - 1]'), true);
  is('nothing was left trailing', /[ \t]\r?\n/.test(got), false);

  // Batching is the whole reason formatMany takes an array: dotnet costs a few
  // seconds of SDK start-up per call. So prove the SECOND file is formatted too,
  // and not just carried along.
  const UGLY2 = [
    'public class T {',
    '    public int F(int[] a) {',
    '        int s=0;',
    '        for(int i=0;i<a.Length;i++)',
    '            s+=a[i];',
    '        return s;',
    '    }',
    '}',
  ].join('\n');

  const many = formatMany([UGLY, UGLY2]);
  is('several files come back in order', many?.length, 2);
  is('the first is still the first', many?.[0]?.includes('Rob'), true);
  is('the second changed too', many?.[1] !== UGLY2, true);
  is('and not one of its tokens moved', sameShape(UGLY2, many[1]).ok, true);
  is('its braces went Allman as well', /class T\s*\r?\n\{/.test(many[1]), true);
  is('its operators got their spaces', many[1].includes('int s = 0;'), true);
  is('its for-header got spaced', /for \(int i = 0; i < a\.Length; i\+\+\)/.test(many[1]), true);
  // The brace claim again, in a different shape. A braceless `for` body is as
  // untouched as a braceless `if` body - because `whitespace` runs no style rules.
  is('and its braceless for body stayed braceless', /\)\s*\r?\n\s*s \+= a\[i\];/.test(many[1]), true);

  // Worth recording, because it looks like a bug the first time you see it.
  // `.editorconfig` sets csharp_preserve_single_line_blocks = true (the .NET
  // default), so a type or member written entirely on one line is LEFT on one
  // line and its brace is not moved to its own line. That is the setting doing
  // its job - it is what stops `public int Count { get; set; }` being exploded
  // across four lines - not the formatter failing to run.
  const ONELINE = 'public class U { public int F() { return 1; } }';
  const kept = formatMany([ONELINE]);
  is('a single-line block is preserved, brace and all', kept?.[0]?.trim(), ONELINE);
}

console.log(`\n${pass} passed, ${fail} failed${skip ? `, ${skip} skipped` : ''}.`);
process.exit(fail ? 1 : 0);
