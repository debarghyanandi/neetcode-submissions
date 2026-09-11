// --------------------------------------------------------------------------
// -  optimal.cs            O(n) time / O(n) space
// -  hash set, walk runs from left edge only   [hashset-sequence-start]
// -  the only solution in this folder
// -
// -  Reference solution - not one you solved yourself
// -
// -  Builds a HashSet for O(1) membership and only starts a walk when
// -  number-1 is absent, so each element is visited by exactly one
// -  run-walk, giving amortized O(n) total work.
// --------------------------------------------------------------------------

public class Solution
{
    public int LongestConsecutive(int[] nums)
    {
        // O(1) membership tests, and duplicates collapse for free.
        var numberSet = new HashSet<int>(nums);

        int longestRun = 0;

        foreach (int number in numberSet)
        {
            // Only start counting from the LEFT EDGE of a run.
            // If number - 1 exists, this number is mid-run and some earlier
            // (or later) iteration will count the run that contains it.
            if (numberSet.Contains(number - 1))
                continue;

            int runLength = 0;

            while (numberSet.Contains(number + runLength))
            {
                runLength++;
            }

            if (runLength > longestRun)
                longestRun = runLength;
        }

        return longestRun;
    }
}

/*
================================================================================
 PATTERN : Hash set + start-of-run guard, expand each run once
 SOURCE  : Reference solution - not one you solved yourself - your own
           annotation at c76939d
 STATUS  : Optimal
================================================================================
WHY THIS PATTERN
  The obvious solution is sort then scan for adjacency, which costs O(n log n)
  and is dominated by the sort. The insight here is that consecutiveness is a
  membership question, not an ordering question: to know whether a run continues
  past number you only need to ask "is number + 1 present?" A HashSet answers
  that in O(1), so the sort is pure waste. numberSet also collapses duplicates
  for free, which the sort-based version has to handle with an explicit skip.
THE ONE IDEA: LEFT-EDGE GUARD
  if (numberSet.Contains(number - 1)) continue; is the whole algorithm. Without
  it, every element of a run walks the rest of that run, and a single run of
  length m does 1 + 2 + ... + m work. With it, only the smallest element of each
  run is allowed to expand. Every other element bails after one lookup.

  Correctness: every non-empty run has exactly one element with no predecessor
  in the set - its minimum. So each run is expanded exactly once, never zero
  times, never twice. No run is missed and none is double-counted.
WHY THE NESTED LOOP IS NOT QUADRATIC
  This is the follow-up an interviewer will ask, because the code reads like two
  nested loops over the same data. The counting argument: the while loop only
  runs for left edges, and the total number of iterations across all while loops
  equals the total length of all runs, which is exactly the size of numberSet.
  Each element is touched by at most one while loop - the one belonging to its
  own run's minimum. So the inner work summed over the whole foreach is linear,
  plus one wasted Contains per non-edge element.
WHY FOREACH OVER NUMBERSET AND NOT NUMS
  Iterating the deduplicated set matters for the runtime argument, not just for
  style. If you wrote foreach (int number in nums), a duplicated left edge would
  re-expand its entire run once per copy. Take nums = [1, 1, 1, ..., 1, 2, 3,
  ..., m] with d copies of 1: the guard passes d times and each pass walks m
  elements, giving d*m work, which is quadratic in n = d + m. Deduplicating
  first caps the foreach at one visit per distinct value.
OFF-BY-ONE AND THE ZERO CASE
  runLength starts at 0 and the test is Contains(number + runLength), so the
  first probe re-checks number itself - a lookup you know will succeed. That is
  deliberate: it makes runLength end as the count of members rather than the
  offset of the last member, so no +1 is needed at the end.

  Empty input returns 0 because longestRun is initialized to 0 and the foreach
  body never executes. A single element yields runLength 1. There is no
  special-casing anywhere.
WATCH OUT
  number + runLength is unchecked int arithmetic. If the set contains
  int.MaxValue as a left edge, the second probe computes int.MaxValue + 1, which
  wraps to int.MinValue. If int.MinValue also happens to be in nums, the run
  falsely continues and longestRun is overstated. LeetCode's constraints keep
  values away from the boundary so the file is correct as submitted, but name
  this if asked to harden it - the fix is to break when number + runLength would
  overflow, or to widen the probe to long.
TRIGGER
  Reach for set-plus-left-edge whenever the problem asks about maximal chains,
  intervals, or islands over unordered values and you catch yourself reaching
  for a sort. The general shape: build O(1) membership, identify a canonical
  starting element per group (here: no predecessor), and let only canonical
  elements do the expansion. The same trick drives grid island counting, where
  the guard is "this cell is unvisited" instead of "number - 1 is absent".
COMPLEXITY
  Time  : O(n)
  Space : O(n)
================================================================================
*/
