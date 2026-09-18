#!/usr/bin/env node
/**
 * lint-rules.test.mjs - whether a file still wants a formatting pass.
 *
 * This file used to be twenty tests about a retry ladder: how many runs could attempt
 * a file the model kept refusing, when a refusal became permanent, and the bug where a
 * recorded FAILURE was read back as "already linted" and counted clean. All of that
 * existed because a model rewrote the code. Lint calls no model now, so there is
 * nothing to refuse and nothing to retry - the question is only "formatted under the
 * current rules, or not".
 *
 * What still matters is that lint.mjs and select-backfill.mjs ask it the SAME way.
 *
 *   node scripts/lib/lint-rules.test.mjs
 */

import { lintWanted, LINT_FORMAT } from './lint-rules.mjs';

let pass = 0, fail = 0;
const is = (name, got, want) => {
  if (got === want) { pass++; console.log(`ok    ${name}`); }
  else { fail++; console.log(`FAIL  ${name}\n      got ${JSON.stringify(got)}, wanted ${JSON.stringify(want)}`); }
};

is('a file nothing is known about wants a pass', lintWanted(undefined), true);
is('so does one with an empty record', lintWanted({}), true);
is('a file formatted under the current rules does not', lintWanted({ version: LINT_FORMAT, codePrint: 'abc' }), false);
is('one formatted under older rules does', lintWanted({ version: LINT_FORMAT - 1, codePrint: 'abc' }), true);

// Records written by the old model-rewrite lint carry version 1 and, sometimes,
// `failed: true` or a `renames` list. They are not formatted records, and the version
// check alone has to be enough to pick them up - there is no longer any code that
// knows what `failed` meant.
is('an old model-lint record is picked up by version alone',
  lintWanted({ version: 1, codePrint: 'abc', renames: ['n->length'] }), true);
is('including one the old lint had given up on',
  lintWanted({ version: 1, failed: true, attempts: 2, reason: 'token count changed' }), true);

// LINT_FORMAT is the whole rollout mechanism for .editorconfig changes: bump it and
// every folder goes back into the backfill queue. If it ever stops being a number
// that check silently passes for everything.
is('the format version is a number', typeof LINT_FORMAT, 'number');
is('and it is past the model-rewrite era', LINT_FORMAT > 1, true);

console.log(`\n${pass} passed, ${fail} failed.`);
process.exit(fail ? 1 : 0);
