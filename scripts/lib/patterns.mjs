/**
 * Which pattern a problem belongs to, for the index page.
 *
 * The groups are the NeetCode roadmap's own sections, because that is the map I
 * actually study from - not a taxonomy invented here. Ordering a page by the
 * same sections as the roadmap means "what have I not covered in Trees yet" is
 * answerable by looking at it.
 *
 * WHERE THE ANSWER COMES FROM, in order of trust:
 *
 *   1. `structures`, recorded by classify from the code itself. Strongest, and
 *      the only one derived from what the solution actually builds.
 *   2. the slug, which is NeetCode's own and therefore stable.
 *   3. the PATTERN line the teach step writes into each file.
 *
 * All three are already in the repo, so the page groups correctly today rather
 * than after some future backfill. (3) alone would be wrong often enough to
 * matter: "Two Pointers - fixed n-gap window plus dummy head" is a linked list
 * problem, and "Monotonic deque of indices" is a sliding window one, so a plain
 * keyword sweep over the prose files both of them in the wrong place.
 *
 * RULES ARE ORDERED AND FIRST MATCH WINS. The order below is load-bearing:
 * Trees sits above Binary Search so `binary-tree-diameter` is not read as a
 * search, and Sliding Window sits above Stack so a monotonic deque is filed by
 * what it is for rather than by what it is made of.
 *
 * When something lands in the wrong group, fix the rule here - it is the only
 * place that decides. The page prints each problem's raw PATTERN line under its
 * name, so a misfiling is visible rather than silent.
 */

/** Display order, and the complete set of group names. NeetCode's sections. */
export const GROUPS = [
  'Arrays & Hashing',
  'Two Pointers',
  'Sliding Window',
  'Stack',
  'Binary Search',
  'Linked List',
  'Trees',
  'Heap / Priority Queue',
  'Backtracking',
  'Tries',
  'Graphs',
  '1-D DP',
  '2-D DP',
  'Intervals',
  'Greedy',
  'Math & Bit Manipulation',
  'Other',
];

/** A one-line reason the group exists, shown under its heading. */
export const GROUP_NOTE = {
  'Arrays & Hashing': 'Trade memory for a lookup, and a nested loop collapses to one pass.',
  'Two Pointers': 'Two indices moving under a rule that never lets you miss an answer.',
  'Sliding Window': 'A range that grows on the right and shrinks on the left, never restarting.',
  'Stack': 'The most recent unresolved thing is the one you need next.',
  'Binary Search': 'Halve a space that is ordered — sometimes the array, sometimes the answer.',
  'Linked List': 'Pointer surgery. The trick is almost always a dummy head or a second pointer.',
  'Trees': 'Recursion that returns something useful from every subtree.',
  'Heap / Priority Queue': 'You need the best few, not the sorted whole.',
  'Backtracking': 'Build a candidate, undo the last choice, try the next.',
  'Tries': 'A tree keyed by prefix, so shared beginnings are stored once.',
  'Graphs': 'Nodes and edges — the traversal is the same, the bookkeeping is the problem.',
  '1-D DP': 'A row of subproblem answers, each built from the ones before it.',
  '2-D DP': 'A table of subproblem answers, filled in an order that never looks forward.',
  'Intervals': 'Sort by one end, then sweep once.',
  'Greedy': 'The locally best choice is provably the globally best one.',
  'Math & Bit Manipulation': 'The structure is in the arithmetic, not in a data structure.',
  'Other': 'Not yet filed. Add a rule in scripts/lib/patterns.mjs.',
};

const has = (structures, ...want) => want.some((w) => (structures || []).includes(w));

/**
 * Ordered. First match wins. Each test gets {slug, pattern, structures}.
 * `pattern` is the PATTERN line, lowercased, or '' when the folder has not been
 * taught yet - so no rule may depend on it being present.
 */
const RULES = [
  // Pointer surgery first: several of these describe themselves as two-pointer
  // or fast/slow problems, which they are - but the thing being pointed at is a
  // list, and that is what you revise them as.
  { group: 'Linked List', test: (c) =>
      has(c.structures, 'linked-list') ||
      /linked-list|lru-cache|add-two-numbers|reorder-linked|remove-node-from-end/.test(c.slug) ||
      /dummy head|floyd|linked list|splice|sentinels/.test(c.pattern) },

  { group: 'Trees', test: (c) =>
      has(c.structures, 'tree', 'trie') ||
      // Anchored on purpose: a bare /bst/ matches "su(bst)ring", which quietly
      // filed both longest-substring problems under Trees.
      /(^|-)tree|(^|-)bst(-|$)|binary-search-tree/.test(c.slug) ||
      /\bbst\b|subtree|post-order|in-order|preorder|level order/.test(c.pattern) },

  { group: 'Graphs', test: (c) =>
      has(c.structures, 'graph', 'union-find') ||
      // Spelled out rather than /rot/, which matches "(rot)ated-sorted-array".
      /island|graph|connected-component|course-schedule|clone-graph|pacific|rotting-orange/.test(c.slug) ||
      /adjacen|flood fill|union.find|disjoint set|topological/.test(c.pattern) },

  { group: 'Tries', test: (c) => has(c.structures, 'trie') || /trie|prefix-tree/.test(c.slug) },

  { group: 'Heap / Priority Queue', test: (c) =>
      has(c.structures, 'heap') || /\bheap\b|priority queue/.test(c.pattern) },

  { group: 'Intervals', test: (c) =>
      has(c.structures, 'interval') || /interval|meeting-room/.test(c.slug) },

  // Above Stack on purpose: a monotonic deque is a sliding-window technique,
  // and filing it under Stack because of what it is built from hides that.
  { group: 'Sliding Window', test: (c) =>
      /sliding window/.test(c.pattern) ||
      /sliding-window|substring-without|repeating-substring|permutation-string|minimum-window|minimum-size-subarray|buy-and-sell/.test(c.slug) },

  { group: 'Stack', test: (c) =>
      has(c.structures, 'stack', 'queue', 'deque') ||
      /\bstack\b|monotonic/.test(c.pattern) ||
      /parenthes|stack|polish-notation|daily-temperatures/.test(c.slug) },

  { group: 'Binary Search', test: (c) =>
      /binary search/.test(c.pattern) ||
      /binary-search|rotated-sorted|eating-bananas|search-2d|koko/.test(c.slug) },

  { group: 'Backtracking', test: (c) =>
      /backtrack|permutation of|combination|subsets/.test(c.pattern) ||
      /subsets|combination|permutations|word-search|n-queens/.test(c.slug) },

  { group: 'Two Pointers', test: (c) =>
      /two[- ]pointers?/.test(c.pattern) ||
      /is-palindrome|two-integer-sum-ii|three-integer-sum|max-water|move-zeroes|remove-duplicates-from-sorted|trapping-rain/.test(c.slug) },

  { group: '2-D DP', test: (c) =>
      has(c.structures, 'dp-table') && /matrix|grid|2-d|two-dimensional/.test(c.pattern) },

  { group: '1-D DP', test: (c) =>
      has(c.structures, 'dp-table') ||
      /dynamic programming|memoi[sz]|tabulat/.test(c.pattern) },

  { group: 'Greedy', test: (c) => /kadane|greedy/.test(c.pattern) },

  { group: 'Math & Bit Manipulation', test: (c) =>
      has(c.structures, 'bitmask') || /bit mask|bitwise|xor|modul/.test(c.pattern) },

  { group: 'Arrays & Hashing', test: (c) =>
      has(c.structures, 'hash-map', 'hash-set') ||
      /hash|counting array|prefix|suffix|bucket|framing|canonical key/.test(c.pattern) },
];

/**
 * @param {{slug: string, pattern?: string, structures?: string[]}} problem
 * @returns {string} one of GROUPS
 */
export function groupFor(problem) {
  const ctx = {
    slug: String(problem.slug || '').toLowerCase(),
    pattern: String(problem.pattern || '').toLowerCase(),
    structures: problem.structures || [],
  };
  for (const r of RULES) {
    try { if (r.test(ctx)) return r.group; } catch { /* a bad rule must not take the page down */ }
  }
  return 'Other';
}

/** The PATTERN line the teach step writes, or '' when the file has not been taught. */
export function patternLine(source) {
  const m = String(source).match(/^[^A-Za-z\n]*PATTERN\s*:\s*(.+)$/m);
  return m ? m[1].trim() : '';
}

/** GROUPS order, for sorting a list of group names. */
export const groupRank = (name) => {
  const i = GROUPS.indexOf(name);
  return i < 0 ? GROUPS.length : i;
};
