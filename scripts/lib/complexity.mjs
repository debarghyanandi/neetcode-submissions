/**
 * A totally ordered complexity ladder.
 *
 * The model is not asked to write "O(n)" freehand - it must pick from this list.
 * That is the whole trick: a constrained enum is comparable, so the *naming*
 * decision below is deterministic code, not a second judgement call. The model
 * reads; the script names. If a solution genuinely doesn't fit the ladder the
 * model returns "other", and we refuse to auto-name rather than guess.
 */
export const COMPLEXITY = [
  'O(1)',
  'O(log n)',
  'O(sqrt n)',
  'O(n)',
  'O(n log n)',
  'O(n * k)',
  'O(n^2)',
  'O(n^2 log n)',
  'O(n^3)',
  'O(2^n)',
  'O(n!)',
  'other',
];

export const rank = (c) => {
  const i = COMPLEXITY.indexOf(c);
  return i === -1 ? COMPLEXITY.length : i;
};

export const isUnrankable = (c) => c === 'other' || rank(c) >= COMPLEXITY.length - 1;

/**
 * Turn classified solutions into filenames.
 *
 * Rules, from the repo owner:
 *   - best time complexity wins the name optimal.cs
 *   - another solution at the SAME best time complexity but a different
 *     approach becomes optimal-variant.cs (then -2, -3, ...)
 *   - everything slower is suboptimal.cs (then -2, -3, ...)
 *
 * Two stability rules that matter more than they look:
 *   1. A file already called optimal.cs that is still in the best tier KEEPS
 *      that name. Otherwise every run reshuffles names and the git history
 *      becomes unreadable.
 *   2. Arrival order is not quality order - a suboptimal solution pushed after
 *      an optimal one must not displace it. Ranking is by complexity only.
 */
export function assignNames(solutions) {
  if (solutions.some((s) => isUnrankable(s.time))) {
    return { ok: false, reason: 'at least one solution has an unrankable complexity', names: null };
  }

  const best = Math.min(...solutions.map((s) => rank(s.time)));
  const bestTier = solutions.filter((s) => rank(s.time) === best);
  const rest = solutions.filter((s) => rank(s.time) > best);

  // Within the best tier: an incumbent optimal.cs holds its name; otherwise the
  // tie breaks on space, then on submission order.
  const incumbent = bestTier.find((s) => s.file === 'optimal.cs');
  const others = bestTier
    .filter((s) => s !== incumbent)
    .sort((a, b) => rank(a.space) - rank(b.space) || a.file.localeCompare(b.file, undefined, { numeric: true }));
  const tierOrder = incumbent ? [incumbent, ...others] : others;

  const names = new Map();
  tierOrder.forEach((s, i) => {
    names.set(s.file, i === 0 ? 'optimal.cs' : i === 1 ? 'optimal-variant.cs' : `optimal-variant-${i}.cs`);
  });

  rest
    .sort((a, b) => rank(a.time) - rank(b.time) || rank(a.space) - rank(b.space) || a.file.localeCompare(b.file, undefined, { numeric: true }))
    .forEach((s, i) => {
      names.set(s.file, i === 0 ? 'suboptimal.cs' : `suboptimal-${i + 1}.cs`);
    });

  return { ok: true, reason: null, names };
}

/**
 * Did you mark this one as your own in the NeetCode editor?
 *
 * Observed in your repo as //My solution, //My Solution, // My solution,
 * //my solution., //mY solution. - so: case-insensitive, optional space,
 * optional trailing punctuation.
 *
 * Absence proves nothing. It means "unmarked", never "not yours" - you can
 * forget to type it, and a header that asserts you didn't solve something you
 * did is worse than one that stays quiet.
 */
export const SELF_MARK_RE = /\/\/\s*my\s+solution\s*\.?/i;
export const isSelfMarked = (src) => SELF_MARK_RE.test(src);
