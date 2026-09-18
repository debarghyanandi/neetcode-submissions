/**
 * The version of the lint rules, and the decision of what to do with a file
 * lint has already seen.
 *
 * Lives in its own module rather than in lint.mjs because lint.mjs is a CLI
 * script: importing a constant from it runs the whole thing, model calls and
 * all. Anything two scripts both need belongs under lib/.
 *
 * Bump LINT_FORMAT when the rules in lint.mjs change, so files linted under the
 * old rules are revisited by the next backfill.
 */
export const LINT_FORMAT = 1;

/**
 * How many RUNS may attempt a file that keeps being refused. Each run is worth
 * two model calls (an attempt and one retry with the rejection fed back), so
 * this is a ceiling of four calls per file, ever.
 *
 * Why more than one run at all. Lint used to write `failed: true` the first
 * time a file was refused twice, and never look at it again. Every other model
 * step in this pipeline does the opposite: classify leaves the folder pending,
 * teach and visualize leave no signature, and all three are picked up by the
 * next run automatically. Lint was the one step where a bad afternoon became a
 * permanent decision.
 *
 * Why not unlimited. The original reasoning was sound as far as it went: two
 * calls that end in the same rejection will end in it again, and a file nobody
 * ever fixes would be paid for on every run forever. The answer is a bound, not
 * a life sentence.
 */
export const LINT_MAX_ROUNDS = 2;

/**
 * What to do with a file, given what state.json remembers about it.
 *
 *   lint          - call the model
 *   skip-done     - already linted under the current rules
 *   skip-refused  - refused LINT_MAX_ROUNDS times over and the code has not
 *                   changed since. Not retried, and not quietly counted as
 *                   "already linted" either - the caller reports it.
 *
 * `rounds` is how many runs have already tried and failed on THIS exact code,
 * so the caller can record the next number. Editing the file resets it: new
 * code has never been refused, whatever happened to its predecessor.
 *
 * @param rec           state.problems[slug].lint[file], or undefined
 * @param currentPrint  shortPrint() of the file's code as it is right now
 * @param force         --force: the human asked for it, so nothing is skipped
 */
export function lintDecision(rec, currentPrint, force = false) {
  if (force) return { action: 'lint', rounds: 0 };
  if (!rec || rec.version !== LINT_FORMAT) return { action: 'lint', rounds: 0 };
  if (!rec.failed) return { action: 'skip-done', rounds: 0 };

  // Both sides must actually be known before a difference means anything. A
  // record written before this field existed has no print; select-backfill asks
  // this question over every folder in the repo without reading the files, so it
  // passes none either. Either missing means "no evidence the code changed" -
  // NOT "the code changed", which is how the queue and the loop briefly came to
  // disagree about whether a given-up file was owed another try.
  const changed = rec.print != null && currentPrint != null && rec.print !== currentPrint;
  if (changed) return { action: 'lint', rounds: 0 };

  const rounds = rec.attempts ?? 1;
  return rounds < LINT_MAX_ROUNDS ? { action: 'lint', rounds } : { action: 'skip-refused', rounds };
}

/** Does this file still want a lint pass? Used by select-backfill to build the queue. */
export const lintWanted = (rec, currentPrint) => lintDecision(rec, currentPrint).action === 'lint';
