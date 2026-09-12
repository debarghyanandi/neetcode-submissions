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
  'O(log(m*n))',  // a matrix treated as one flat sorted array
  'O(sqrt n)',
  // A bounded structure - a heap capped at k, a window of k - where k <= n by
  // the problem's own definition. Placed directly below O(n) for the same
  // reason O(n log k) sits below O(n log n): k is bounded by n and may
  // approach it, so ranking it any lower would claim a guarantee the code
  // does not make.
  'O(k)',
  'O(n)',
  'O(n + m)',     // two independent inputs, each scanned once
  'O(n log k)',   // k <= n, so this sits below O(n log n)
  'O(n log n)',
  'O(m log n)',   // a binary search per row - m rows, log n columns
  'O(n * k)',     // n items of length k - the whole input, once
  'O(m * n)',     // every cell of a matrix     // n items of length k - the whole input, once
  'O(n * k log k)', // ...and again with a sort per item. anagram-groups lives here.
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
 * Which position in the optimal sequence a filename already holds:
 * optimal.cs is 0, optimal-variant.cs is 1, optimal-variant-N.cs is N.
 * Anything else - a raw submission - has no claim on a slot and sorts last.
 */
const slot = (file) => {
  if (file === 'optimal.cs') return 0;
  if (file === 'optimal-variant.cs') return 1;
  const m = file.match(/^optimal-variant-(\d+)\.cs$/);
  return m ? Number(m[1]) : Number.MAX_SAFE_INTEGER;
};

/**
 * Turn classified solutions into filenames.
 *
 * Rules, from the repo owner:
 *   - the best solution is called optimal.cs
 *   - a second solution that is JUST AS GOOD becomes optimal-variant.cs
 *     (then -2, -3, ...)
 *   - anything worse is suboptimal.cs (then -2, -3, ...)
 *
 * "Just as good" means equal on time AND space. An earlier version ranked on
 * time alone, and it was wrong: minimum-size-subarray-sum holds two O(n) window
 * solutions, one keeping a prefix array at O(n) space and one collapsing it to a
 * running int at O(1). Ranking on time alone promoted the prefix-array version
 * to optimal-variant.cs; the repo owner had called it suboptimal.cs, and the
 * repo owner is right - same time, more space, strictly worse.
 *
 * So: order lexicographically by (time, space). Ties on both are variants.
 *
 * Two stability rules that matter more than they look:
 *   1. A file already called optimal.cs that is still in the best tier KEEPS
 *      that name. Otherwise every run reshuffles names and the git history
 *      becomes unreadable.
 *   2. Arrival order is not quality order - a suboptimal solution pushed after
 *      an optimal one must not displace it. Ranking is by complexity only.
 */
export function assignNames(solutions) {
  if (solutions.some((s) => isUnrankable(s.time) || isUnrankable(s.space))) {
    return { ok: false, reason: 'at least one solution has an unrankable complexity', names: null };
  }

  const key = (s) => [rank(s.time), rank(s.space)];
  const cmp = (a, b) => rank(a.time) - rank(b.time) || rank(a.space) - rank(b.space);
  const sorted = [...solutions].sort(cmp);
  const [bt, bs] = key(sorted[0]);

  // The best tier is everything equal on BOTH axes - genuinely interchangeable.
  const bestTier = sorted.filter((s) => rank(s.time) === bt && rank(s.space) === bs);
  const rest = sorted.filter((s) => !(rank(s.time) === bt && rank(s.space) === bs));

  // Order within the best tier, most-deserving of `optimal.cs` first.
  //
  // 1. A solution you solved yourself. Equal on both axes means nothing else
  //    separates them, and this is a study log: when two answers are equally
  //    good the one you wrote is the one worth opening first. It outranks the
  //    incumbent deliberately, so a folder that got this wrong before is
  //    corrected the next time it is processed rather than frozen that way.
  //    The swap happens once - afterwards the self-marked file IS optimal.cs,
  //    and both rules agree.
  // 2. Whichever file is already called optimal.cs. Renaming for no reason
  //    churns the repo and every link into it.
  // 3. The slot the file already occupies, so a curated folder keeps the names
  //    it has. This used to be a plain numeric filename sort, and numeric
  //    collation puts optimal-variant-2.cs BEFORE optimal-variant.cs - so a
  //    three-way tie renamed those two past each other on every single run,
  //    invalidating the header, the teaching block and the visualizer each
  //    time. Raw submissions have no slot and sort after, by number.
  const tierOrder = [...bestTier].sort((a, b) =>
    (a.selfMarked ? 0 : 1) - (b.selfMarked ? 0 : 1) ||
    slot(a.file) - slot(b.file) ||
    a.file.localeCompare(b.file, undefined, { numeric: true }));

  const names = new Map();
  tierOrder.forEach((s, i) => {
    names.set(s.file, i === 0 ? 'optimal.cs' : i === 1 ? 'optimal-variant.cs' : `optimal-variant-${i}.cs`);
  });

  rest.forEach((s, i) => {
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
