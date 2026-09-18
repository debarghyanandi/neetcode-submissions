/**
 * The version of the formatting rules, and whether a file still wants a pass.
 *
 * Lives in its own module rather than in lint.mjs because lint.mjs is a CLI script:
 * importing a constant from it would run the whole thing. Anything two scripts both
 * need belongs under lib/.
 *
 * This file used to carry a retry ladder - LINT_MAX_ROUNDS, `failed: true` records,
 * a rule for how many runs could attempt a file before giving up on it. All of that
 * existed because a model rewrote the code and could be refused. Lint calls no model
 * now (see lint.mjs for the 92-rename audit that ended it), so there is nothing to
 * refuse and nothing to retry: `dotnet format` either runs or it does not.
 *
 * Bump LINT_FORMAT when .editorconfig changes, so files formatted under the old
 * rules are revisited by the next backfill.
 *
 * 2 = formatting only, by dotnet. 1 = the old model rewrite.
 */
export const LINT_FORMAT = 2;

/**
 * Does this file still want a formatting pass?
 *
 * Asked per file by lint.mjs and over the whole repo by select-backfill.mjs, from the
 * same function on purpose - two implementations of one question is how a pipeline
 * starts disagreeing with itself about what is done.
 *
 * @param rec state.problems[slug].lint[file], or undefined
 */
export const lintWanted = (rec) => !rec || rec.version !== LINT_FORMAT;
