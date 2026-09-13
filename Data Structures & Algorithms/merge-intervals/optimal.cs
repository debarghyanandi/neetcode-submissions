// --------------------------------------------------------------------------
// -  optimal.cs            O(n log n) time / O(log n) space
// -  sort by start, sweep and merge   [sort-sweep-merge-intervals]
// -  the only solution in this folder
// -
// -  Reference solution - not one you solved yourself
// -
// -  sorting by start reduces overlap detection to comparing each interval
// -  against the last merged block in one linear pass; auxiliary space is
// -  the sort's recursion stack
// --------------------------------------------------------------------------

public class Solution
{
    public int[][] Merge(int[][] intervals)
    {
        // Sorting by START is what makes a single pass sufficient: any
        // interval that can overlap the one being built must come next.
        Array.Sort(intervals, (a, b) => a[0].CompareTo(b[0]));

        var merged = new List<int[]>();

        // Copy rather than reference, so the caller's array is never mutated.
        merged.Add(new int[] { intervals[0][0], intervals[0][1] });

        for (int i = 1; i < intervals.Length; i++)
        {
            int currentStart = intervals[i][0];
            int currentEnd = intervals[i][1];

            int[] lastMerged = merged[merged.Count - 1];

            if (currentStart <= lastMerged[1])
            {
                // Overlap: absorb it by stretching the end.
                // Math.Max matters - the current interval may be fully
                // CONTAINED, e.g. [1,10] then [2,3] must stay [1,10].
                lastMerged[1] = Math.Max(lastMerged[1], currentEnd);
            }
            else
            {
                // Gap: the previous block is final, start a new one.
                merged.Add(new int[] { currentStart, currentEnd });
            }
        }

        return merged.ToArray();
    }
}

/*
================================================================================
 PATTERN : Sort by start, then one-pass merge into the last block
 SOURCE  : Reference solution - not one you solved yourself - your own
           annotation at c76939d
 STATUS  : Optimal
================================================================================
WHY THIS PATTERN
  Overlap is a relation between pairs, so the untrained instinct is pairwise:
  compare every interval against every other, or keep sweeping the list fusing
  any two that touch until a full pass changes nothing. Both are quadratic and
  both are fiddly to prove terminating. Sorting by start converts the global
  pairwise question into a local one: once the array is in start order, the only
  interval that can overlap the block you are currently building is the very
  next one you will read. Nothing behind you, nothing hiding further ahead.
INVARIANT
  At the top of each iteration of the i loop, merged holds pairwise-disjoint
  intervals in increasing start order, and it is exactly the correct answer for
  intervals[0..i-1]. The consequence you must be able to state out loud:
  merged[merged.Count-1] is the only entry that can still grow. Every earlier
  entry is permanently final.
WHY ONE COMPARISON SUFFICES
  This is the correctness argument an interviewer will push on - why look at
  lastMerged only, and never rescan?

  1. By sort order, currentStart is >= the start of every interval already
  placed.
  2. lastMerged therefore has the largest start in merged, and the earlier
  entries are disjoint from it, so each of their ends is below lastMerged[0].
  3. So if currentStart > lastMerged[1], then currentStart also exceeds every
  earlier end. No older block can be revived.

  That is exactly why the else branch is allowed to say the previous block is
  final and simply append, and why the if branch never has to walk backwards.
THE TWO TRAPS
  Math.Max is load-bearing. The current interval may be fully CONTAINED: [1,10]
  then [2,3]. A plain lastMerged[1] = currentEnd shrinks the block to [1,3] and
  silently loses coverage - and the bug stays invisible on any test where the
  ends happen to arrive in increasing order.

  The comparison is currentStart <= lastMerged[1], not <, so touching counts as
  overlapping: [1,4] then [4,5] yields [1,5]. That is what this problem wants. A
  half-open variant, or one asking for strictly overlapping intervals, flips
  that single character to <.

  Minor but real: the comparator uses a[0].CompareTo(b[0]) rather than a[0] -
  b[0]. The subtraction form overflows and returns the wrong sign at extreme int
  values.
ALIASING AND MUTATION
  Both merged.Add calls build a fresh new int[] instead of pushing intervals[i]
  itself. That is not stylistic. The merge step mutates in place - lastMerged[1]
  = Math.Max(...) writes through the reference held in the list - so without the
  copies that write would reach into the caller's inner arrays and corrupt their
  input.

  Be honest about the limit of that guarantee: Array.Sort reorders the outer
  intervals array in place. The caller's element arrays are protected; the
  ordering of the array they handed you is not.
EDGE CASE AND FOLLOW-UPS
  intervals[0] is dereferenced before the loop with no length check, so an empty
  input throws IndexOutOfRangeException. LeetCode guarantees at least one
  interval, so this passes, but it is the first thing a reviewer notices - a
  leading length-zero guard returning intervals costs one line.

  Likely follow-ups: you cannot beat the sort in a comparison model, since
  merging intervals solves element distinctness. Insert Interval drops to linear
  precisely because the input arrives already sorted, removing the only
  superlinear step here. And if the question turns into maximum concurrent
  overlap rather than merged spans, this loop cannot answer it - you need the
  sweep over separated +1/-1 endpoint events instead.
TRIGGER
  Input is a collection of [start, end] pairs and the answer depends on how they
  overlap: merge them, count rooms needed, find the free gaps. Reach for
  sort-by-start first. The moment the list is ordered, the remainder is a linear
  scan holding exactly one open block.
COMPLEXITY
  Time  : O(n log n)
  Space : O(log n)
================================================================================
*/
