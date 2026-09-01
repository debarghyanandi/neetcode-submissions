/**
 * The version of the lint rules.
 *
 * Lives in its own module rather than in lint.mjs because lint.mjs is a CLI
 * script: importing a constant from it runs the whole thing, model calls and
 * all. Anything two scripts both need belongs under lib/.
 *
 * Bump when the rules in lint.mjs change, so files linted under the old rules
 * are revisited by the next backfill.
 */
export const LINT_FORMAT = 1;
