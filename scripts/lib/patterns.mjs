/**
 * Which pattern each problem belongs to.
 *
 * Grouping comes from two sources, in this order:
 *   1. PATTERNS below - the NeetCode roadmap category, edited by hand.
 *   2. Nothing. An unmapped problem lands in "Unsorted" on the index page.
 *
 * Deliberately not inferred from the classifier. approachKey describes the
 * technique used by one FILE ("sliding-window-prefix-sum"); a problem's pattern
 * is what section of the roadmap it teaches, and the two are not the same - the
 * brute-force file in a folder would drag the whole problem into the wrong
 * group. A visible Unsorted section is better than a confident mistake, so add
 * new problems here as you meet them.
 */
export const PATTERNS = {
  'two-integer-sum': 'Arrays & Hashing',
  'duplicate-integer': 'Arrays & Hashing',
  'is-anagram': 'Arrays & Hashing',
  'anagram-groups': 'Arrays & Hashing',
  'top-k-elements-in-list': 'Arrays & Hashing',
  'products-of-array-discluding-self': 'Arrays & Hashing',
  'longest-consecutive-sequence': 'Arrays & Hashing',

  'is-palindrome': 'Two Pointers',
  'two-integer-sum-ii': 'Two Pointers',
  'three-integer-sum': 'Two Pointers',
  'max-water-container': 'Two Pointers',
  'move-zeroes': 'Two Pointers',

  'buy-and-sell-crypto': 'Sliding Window',
  'longest-substring-without-duplicates': 'Sliding Window',
  'longest-repeating-substring-with-replacement': 'Sliding Window',
  'permutation-string': 'Sliding Window',
  'minimum-window-with-characters': 'Sliding Window',
  'sliding-window-maximum': 'Sliding Window',
  'minimum-size-subarray-sum': 'Sliding Window',

  'validate-parentheses': 'Stack',
  'minimum-stack': 'Stack',

  'maximum-subarray': 'Greedy',
  'merge-intervals': 'Intervals',
};

/** Section order on the index page - the order you work through them. */
export const PATTERN_ORDER = [
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
  'Dynamic Programming',
  'Greedy',
  'Intervals',
  'Bit Manipulation',
  'Math & Geometry',
  'Unsorted',
];

export const patternFor = (slug) => PATTERNS[slug] ?? 'Unsorted';
