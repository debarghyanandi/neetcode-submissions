#!/usr/bin/env node
/**
 * lint-rules.test.mjs - what happens to a file lint has already seen.
 *
 * The case that made this a file of its own: lint was the only model step that
 * turned one bad run into a permanent decision. It wrote `failed: true`, and
 * every run after that read the record, printed "already linted", counted the
 * file as clean and went green. Everything below is about that record being
 * read honestly - and about the retry being bounded rather than removed.
 *
 *   node scripts/lib/lint-rules.test.mjs
 */

import { lintDecision, lintWanted, LINT_FORMAT, LINT_MAX_ROUNDS } from './lint-rules.mjs';

let pass = 0, fail = 0;
const is = (name, got, want) => {
  if (got === want) { pass++; console.log(`ok    ${name}`); }
  else { fail++; console.log(`FAIL  ${name}\n      got ${JSON.stringify(got)}, wanted ${JSON.stringify(want)}`); }
};
const act = (rec, print = 'aaaa', force = false) => lintDecision(rec, print, force).action;

const PRINT = 'befda3bfc2de';
const done = { version: LINT_FORMAT, codePrint: PRINT, renames: [] };
const refused = (n) => ({ version: LINT_FORMAT, failed: true, attempts: n, print: PRINT, reason: 'token count changed' });

// ---- the ordinary cases -------------------------------------------------
is('a file nothing is known about is linted', act(undefined), 'lint');
is('a file linted under older rules is linted again', act({ version: LINT_FORMAT - 1, codePrint: PRINT }), 'lint');
is('a file already linted is left alone', act(done, PRINT), 'skip-done');

// ---- the bug this file exists for ---------------------------------------
// A failure is not a success. Whatever else happens, it must never come back
// as skip-done, because skip-done is what the run summary counts as clean.
is('a refused file is never reported as already linted', act(refused(1), PRINT) === 'skip-done', false);

// ---- the retry ----------------------------------------------------------
is('refused once, so it is tried again on the next run', act(refused(1), PRINT), 'lint');
is('refused twice, so it is given up on', act(refused(2), PRINT), 'skip-refused');
is('and stays given up on', act(refused(9), PRINT), 'skip-refused');
is('the rounds it has already burned are reported', lintDecision(refused(1), PRINT).rounds, 1);

// The bound is the point: without it, a file nobody will ever fix is paid for
// on every run forever - which is what the original `failed: true` was for.
is('the bound is a bound', act(refused(LINT_MAX_ROUNDS), PRINT), 'skip-refused');

// ---- editing the file resets it -----------------------------------------
// New code has never been refused, whatever happened to the code it replaced.
is('given up on, but the code has changed since', act(refused(9), 'different'), 'lint');
is('and that is a clean slate, not a continuation', lintDecision(refused(9), 'different').rounds, 0);

// ---- a record from before `print` existed --------------------------------
is('no print recorded, so it is judged on rounds alone',
   act({ version: LINT_FORMAT, failed: true, attempts: 2, reason: 'x' }, 'anything'), 'skip-refused');
is('no print and rounds left over, so it is retried',
   act({ version: LINT_FORMAT, failed: true, reason: 'x' }, 'anything'), 'lint');

// ---- --force ------------------------------------------------------------
is('force lints a file that is already done', act(done, PRINT, true), 'lint');
is('force lints a file that was given up on', act(refused(9), PRINT, true), 'lint');
is('force starts the count over', lintDecision(refused(9), PRINT, true).rounds, 0);

// ---- the queue agrees with the loop -------------------------------------
// select-backfill.mjs asks lintWanted; lint.mjs asks lintDecision. If they ever
// disagree, backfill skips a folder lint would have worked on, or queues one it
// will not touch. Same function, so they cannot.
is('the queue wants a file that has never been linted', lintWanted(undefined, null), true);
is('the queue wants a file with a retry left', lintWanted(refused(1), null), true);
is('the queue leaves a given-up file alone', lintWanted(refused(2), null), false);
is('the queue leaves a finished file alone', lintWanted(done, null), false);

console.log(`\n${pass} passed, ${fail} failed.`);
process.exit(fail ? 1 : 0);
