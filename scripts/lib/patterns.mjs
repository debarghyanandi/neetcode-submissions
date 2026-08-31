/**
 * Which pattern each problem belongs to.
 *
 * Three sources, in strict order of precedence:
 *   1. PATTERNS below - a hand-written override. Wins over everything.
 *   2. The pattern recorded by classify.mjs, chosen by the model from the fixed
 *      list in PATTERN_ORDER. This is what covers a problem you just solved.
 *   3. "Unsorted" - visible at the bottom of the index, never silently misfiled.
 *
 * Note what is NOT used: approachKey. That describes the technique of one FILE
 * ("sliding-window-prefix-sum"), whereas a problem's pattern is the roadmap
 * section it teaches. The brute-force file in a folder would drag the whole
 * problem into the wrong group. So the model is asked the question directly,
 * from a closed list, exactly as it is asked for complexity.
 *
 * Disagree with what it picked? Add a line here and the override wins.
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

/**
 * @param slug   problem folder name
 * @param state  parsed .agent/state.json, optional
 */
export function patternFor(slug, state) {
  if (PATTERNS[slug]) return PATTERNS[slug];
  const recorded = state?.problems?.[slug]?.pattern;
  if (recorded && PATTERN_ORDER.includes(recorded)) return recorded;
  return 'Unsorted';
}
