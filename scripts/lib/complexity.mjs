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
  // Inverse Ackermann - union-find with path compression and union by rank. Below 5 for any
  // input that fits in memory, but it is not O(1) and the ladder does not pretend it is.
  'O(alpha(n))',
  // Below O(log n) for the same reason O(k) sits below O(n): k is bounded by n by the
  // problem's own definition and may approach it. merge-k-sorted-linked-lists is the case
  // that found this one - a divide-and-conquer merge recurses log k deep.
  'O(log k)',
  'O(log n)',
  'O(log(m*n))',  // a matrix treated as one flat sorted array
  'O(log^2 n)',   // a binary search whose step is itself a binary search
  'O(sqrt n)',
  // A bounded structure - a heap capped at k, a window of k - where k <= n by
  // the problem's own definition. Placed directly below O(n) for the same
  // reason O(n log k) sits below O(n log n): k is bounded by n and may
  // approach it, so ranking it any lower would claim a guarantee the code
  // does not make.
  'O(k)',
  'O(n)',
  'O(n * alpha(n))',  // a union-find pass over the whole input
  'O(n + m)',     // two independent inputs, each scanned once
  'O(n log log n)', // a sieve - count-primes lives here
  'O(k log k)',   // sorting the bounded set, not the whole input. k <= n, so below O(n log k).
  'O(n log k)',   // k <= n, so this sits below O(n log n)
  'O(n log n)',
  // Two independent inputs again: neither O(n log m) nor O(m log n) dominates the other, so
  // they sit adjacent and the order between THEM is arbitrary, like O(m) beside O(n).
  'O(n log m)',   // a binary search over a value range, one pass per probe - koko-style
  'O(m log n)',   // a binary search per row - m rows, log n columns
  'O(n * k)',     // n items of length k - the whole input, once
  'O(m * n)',     // every cell of a matrix, or of a two-input DP table
  'O(m * n * log(m * n))', // a heap over every cell - Dijkstra on a grid
  'O(n * k log k)', // ...and again with a sort per item. anagram-groups lives here.
  'O(n^2)',
  'O(n^2 log n)',
  'O(n^3)',
  'O(2^n)',
  'O(n * 2^n)',   // every subset, and O(n) to copy each one out
  'O(n^2 * 2^n)', // bitmask DP over pairs of states - Held-Karp
  'O(4^n / sqrt n)', // the nth Catalan number - generate-parentheses
  'O(n!)',
  'O(n * n!)',    // every permutation, and O(n) to copy each one out
  'other',
];

/**
 * The same rung wearing a different letter.
 *
 * The ladder above is a list of TIERS, and a tier's letter is an accident of which input the
 * author happened to call n. O(m) is not a worse O(n); it IS O(n), for the other string. The
 * ladder cannot express two entries at one rank - rank is an array index - so a renamed tier
 * lives here and ranks as the thing it points at. Exact equality, not a near-miss ordering.
 *
 * These are OFFERED TO THE MODEL as well (see CHOICES), so it can answer O(m) directly and the
 * header can print the honest letter, rather than going through "other" and a repair.
 *
 * Nothing here changes a magnitude. Every entry is its target with the variables renamed, which
 * is the one kind of off-ladder answer that can be resolved without guessing.
 */
export const ALIAS = {
  'O(m)': 'O(n)',
  'O(log m)': 'O(log n)',
  'O(sqrt m)': 'O(sqrt n)',
  'O(m + n)': 'O(n + m)',
  'O(n * m)': 'O(m * n)',
  'O(m log m)': 'O(n log n)',
  'O(m log k)': 'O(n log k)',
  'O(m * k)': 'O(n * k)',
  'O(m^2)': 'O(n^2)',
  'O(m^3)': 'O(n^3)',
  'O(2^m)': 'O(2^n)',
  'O(m!)': 'O(n!)',
  'O(log(n*m))': 'O(log(m*n))',
  'O((log n)^2)': 'O(log^2 n)',
  'O(2^n * n)': 'O(n * 2^n)',
  'O(n! * n)': 'O(n * n!)',
  // Graph problems are written with V and E as often as with n and m. Capitals are outside
  // canonical()'s rename set (it only touches single lowercase letters), so they are listed
  // here rather than resolved.
  'O(V + E)': 'O(n + m)',
  'O(E log V)': 'O(m log n)',
  'O(V + E log V)': 'O(m log n)',
};

/** What the model may answer: every rung, every alias, and "other" as the last resort. */
export const CHOICES = [
  ...COMPLEXITY.filter((c) => c !== 'other'),
  ...Object.keys(ALIAS),
  'other',
];

/** Spacing and case are not meaning: "O(m*n)", "O(m * n)" and "o(M * N)" are one answer. */
const key = (s) => String(s).replace(/\s+/g, '').toLowerCase();
const BY_KEY = new Map();
for (const c of COMPLEXITY) BY_KEY.set(key(c), c);
for (const [a, target] of Object.entries(ALIAS)) BY_KEY.set(key(a), target);

/**
 * A freehand complexity mapped onto the ladder, or null if it genuinely is not on it.
 *
 * This is the rule that stops a new letter from stopping the pipeline. classify's enum only
 * lets the model answer from CHOICES, so anything else arrives as "other" plus a written
 * actualComplexity - and the run then REFUSED the whole folder, which is what happened to
 * longest-common-subsequence: its rolling two-row DP is O(m) space, a tier the ladder held
 * under another name.
 *
 * Resolution is by variable rename ONLY. Single-letter variables are renamed onto n and m,
 * every injective way, and a result is accepted only if it lands exactly on a rung. So
 * "O(a * b)" resolves to O(m * n) and "O(p)" to O(n), while "O(n * 2^n)" resolves to nothing
 * and is still refused - the pipeline should stop for a genuinely new shape, and only for one.
 *
 * k is left alone deliberately: O(k) and O(n log k) are rungs whose whole meaning is "bounded
 * by n but not n", so renaming k would erase the distinction the rung exists to make.
 *
 * When two renames both land, the WORSE rung wins. A rename is inference, and inference here
 * should never promote a solution it does not understand.
 */
export function canonical(text) {
  if (typeof text !== 'string') return null;
  const direct = BY_KEY.get(key(text));
  if (direct) return direct;

  const vars = [...new Set(text.match(/\b[a-z]\b/g) ?? [])].filter((v) => v !== 'k');
  if (!vars.length || vars.length > 2) return null;

  const maps = vars.length === 1
    ? [{ [vars[0]]: 'n' }, { [vars[0]]: 'm' }]
    : [{ [vars[0]]: 'n', [vars[1]]: 'm' }, { [vars[0]]: 'm', [vars[1]]: 'n' }];

  let worst = null;
  for (const map of maps) {
    const hit = BY_KEY.get(key(text.replace(/\b([a-z])\b/g, (ch) => map[ch] ?? ch)));
    if (hit && (worst === null || COMPLEXITY.indexOf(hit) > COMPLEXITY.indexOf(worst))) worst = hit;
  }
  return worst;
}

/**
 * Where a complexity sits on the ladder. An alias or a renamed variable ranks as the tier it
 * is, so O(m) and O(n) come out equal rather than one rung apart - which is the truth, and
 * means a folder holding both reads as a tie instead of a ranking.
 */
export const rank = (c) => {
  const i = COMPLEXITY.indexOf(c);
  if (i !== -1) return i;
  const canon = canonical(c);
  return canon === null ? COMPLEXITY.length : COMPLEXITY.indexOf(canon);
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
