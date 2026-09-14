/**
 * budget.mjs - one place that knows what "the account ran out" looks like.
 *
 * A scheduled run at 2am found one folder, linted it, committed the renames,
 * and then hit "You've hit your session limit - resets 7:40pm". classify died,
 * teach and visualize saw nothing to do and said so cheerfully, and the job
 * went GREEN. The repo was left half-processed and the run said it was fine.
 * That is the failure this file exists to stop: running out of budget is not a
 * bad folder, but it is also not a success, and the run must say so.
 *
 * Three separate things, deliberately:
 *
 *   isOutOfBudget()  - is this exception the account rather than the work? The
 *                      same question in four scripts, answered in one place, so
 *                      a new wording of the refusal is one edit.
 *   stop()           - leave a marker the LATER steps can see. The steps run in
 *                      separate processes, so a variable cannot carry this.
 *   stopped()        - read it. summarise.mjs uses this to fail the run.
 *
 * The marker sits beside $PIPELINE_REPORT rather than in an env var of its own,
 * because the workflow file that sets those is edited by hand and I would
 * rather not need a workflow change to ship a script change. No report file
 * (a local run) means no marker, and every script still prints its own STOPPED
 * block, so nothing here is load-bearing for a human watching a terminal.
 */

import { writeFileSync, readFileSync, existsSync } from 'node:fs';
import { report } from './report.mjs';

/**
 * Deliberately broad. A false positive costs one wrongly-stopped run, which is
 * cheap and obvious; a false negative costs one model call per remaining folder
 * and reports each of them as `failed` next to the genuine failures.
 */
export const OUT_OF_BUDGET = /session limit|usage limit|rate limit exceeded|quota|credit balance|insufficient/i;

export const isOutOfBudget = (err) =>
  OUT_OF_BUDGET.test(String(err && err.message ? err.message : err));

const marker = () =>
  process.env.PIPELINE_BUDGET_STOP ||
  (process.env.PIPELINE_REPORT ? `${process.env.PIPELINE_REPORT}.budget` : null);

/**
 * Record that a step stopped because the account is out of budget.
 * First writer wins: the first step to run out is the one that explains why
 * everything after it did nothing.
 *
 * @param step    lint | classify | teach | visualize
 * @param message the refusal, verbatim - it carries the reset time
 */
export function stop(step, message) {
  const f = marker();
  if (!f || existsSync(f)) return;
  try {
    writeFileSync(f, JSON.stringify({
      step,
      message: String(message).split('\n').find((l) => l.trim()) ?? String(message),
      at: new Date().toISOString(),
    }), 'utf8');
  } catch { /* a marker that cannot be written must not take the run down */ }
}

/** @returns {{step: string, message: string, at: string} | null} */
export function stopped() {
  const f = marker();
  if (!f || !existsSync(f)) return null;
  try { return JSON.parse(readFileSync(f, 'utf8')); } catch { return null; }
}

/**
 * The block every step prints when it gives up. Same words everywhere on
 * purpose: the folders that did not run are not broken, and the next thing to
 * do is wait, not debug.
 */
export function announce(message) {
  console.log(`\nSTOPPED - ${String(message).trim()}`);
  console.log('Out of budget, not a bad folder. Nothing is wrong with the ones that did not run;');
  console.log('name them again after the reset and they will go through.\n');
}

/**
 * Called by a step before it asks for anything. If an EARLIER step in this run
 * already ran out, there is no point buying the same refusal again: four steps
 * each discovering the limit for themselves is four wasted calls and four
 * folders wrongly marked failed. The folders this step was about to touch are
 * recorded as skipped so the summary table shows them as untouched rather than
 * as never-run.
 *
 * @returns true when the caller should stop immediately.
 */
export function haltIfStopped(step, slugs = []) {
  const prior = stopped();
  if (!prior) return false;
  console.log(`\n${step} did not start - ${prior.step} already ran out of budget in this run.`);
  for (const slug of slugs) {
    report(step, slug, 'skipped', `not attempted - out of budget (${prior.step} stopped first)`);
  }
  announce(prior.message);
  return true;
}
