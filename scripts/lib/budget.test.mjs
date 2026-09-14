#!/usr/bin/env node
/**
 * budget.test.mjs - the "did the account run out" rule, and the marker.
 *
 * This exists because the failure it guards against is INVISIBLE. A run that
 * stops early because the subscription hit its limit looks, in the Actions
 * list, exactly like a run that had nothing to do: a green tick. The only
 * thing separating those two readings is the regex below and a file on disk.
 *
 *   node scripts/lib/budget.test.mjs
 */

import { mkdtempSync, rmSync, existsSync } from 'node:fs';
import { tmpdir } from 'node:os';
import { join } from 'node:path';

const dir = mkdtempSync(join(tmpdir(), 'budget-'));
process.env.PIPELINE_BUDGET_STOP = join(dir, 'stop.json');
delete process.env.PIPELINE_REPORT;

const { isOutOfBudget, stop, stopped, OUT_OF_BUDGET } = await import('./budget.mjs');

let pass = 0, fail = 0;
const is = (name, got, want) => {
  const g = JSON.stringify(got), wt = JSON.stringify(want);
  if (g === wt) { pass++; console.log(`ok    ${name}`); }
  else { fail++; console.log(`FAIL  ${name}\n      got:  ${g}\n      want: ${wt}`); }
};
const ok = (name, cond) => is(name, !!cond, true);

// ---- the wordings actually seen from the CLI ------------------------------
ok('the session-limit refusal that started all this',
  isOutOfBudget(new Error("You've hit your session limit · resets 7:40pm (UTC)")));
ok('the five-hour usage limit', isOutOfBudget(new Error('Claude usage limit reached')));
ok('an API-key account running dry', isOutOfBudget(new Error('Your credit balance is too low')));
ok('a plain string works as well as an Error', isOutOfBudget('rate limit exceeded'));

// ---- what must NOT be read as the account running out ---------------------
// These are faults in the work or the setup. Treating one as a budget stop
// would halt the whole run over a single bad folder, which is the opposite of
// what continue-on-error is for.
for (const msg of [
  'validation rejected both attempts',
  'Invalid API key · fix external/ANTHROPIC_API_KEY',
  'exit status 1: command not found: claude',
  'max_turns reached before an answer',
  'API Error: 500 Internal Server Error',
]) is(`"${msg.slice(0, 34)}..." is not a budget stop`, isOutOfBudget(new Error(msg)), false);

ok('nothing is not a budget stop', !isOutOfBudget(undefined) && !isOutOfBudget(''));
ok('the pattern is exported for anyone who needs to explain it', OUT_OF_BUDGET instanceof RegExp);

// ---- the marker, which is how one step tells the next ---------------------
is('nothing recorded yet', stopped(), null);

stop('classify', "You've hit your session limit · resets 7:40pm (UTC)\nsome trailing noise");
const rec = stopped();
is('the step that ran out is recorded', rec.step, 'classify');
is('only the first line of the refusal is kept - it carries the reset time',
  rec.message, "You've hit your session limit · resets 7:40pm (UTC)");
ok('and when', typeof rec.at === 'string' && rec.at.includes('T'));

// The first step to run out is the one that explains why the rest did nothing.
// A later step overwriting it would blame the messenger.
stop('visualize', 'Claude usage limit reached');
is('the first writer wins', stopped().step, 'classify');

// ---- no report file: local runs record nothing and must not throw ---------
delete process.env.PIPELINE_BUDGET_STOP;
{
  const fresh = await import(`./budget.mjs?nofile=${Date.now()}`);
  is('with nowhere to write, there is nothing to read', fresh.stopped(), null);
  fresh.stop('lint', 'Claude usage limit reached');
  is('and recording is a no-op rather than a crash', fresh.stopped(), null);
  is('the rule itself still works with no marker configured',
    fresh.isOutOfBudget(new Error('Claude usage limit reached')), true);
}

rmSync(dir, { recursive: true, force: true });
ok('the test cleaned up after itself', !existsSync(dir));

console.log(`\n${pass} passed, ${fail} failed.`);
process.exit(fail ? 1 : 0);
