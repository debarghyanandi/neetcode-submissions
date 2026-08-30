// --------------------------------------------------------------------------
//  Reference solution - from NeetCode / other resource (submission-0 + submission-2)
//  Not one you solved yourself.
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
 PATTERN : Intervals - Sort by Start, then Sweep and Absorb
 SOURCE  : NeetCode / other resource (submission-0 + submission-2 merged; the
           index counter dropped in favour of reading merged[Count-1])
 STATUS  : Optimal
================================================================================

WHY THIS PATTERN
  Unsorted intervals can overlap anything, so every pair must be checked -
  O(n^2). Sort by START and that collapses: while sweeping left to right, the
  ONLY interval a new one can overlap is the block currently being built.
  Everything earlier has a smaller start AND has already been finalised.
  Sorting turns a pairwise problem into a local one. That is the entire idea
  behind the intervals category, and it recurs in insert-interval,
  non-overlapping-intervals, and meeting-rooms.

BRUTE FORCE (and why it fails)
  Repeatedly scan for any overlapping pair, merge, restart: O(n^2) or worse,
  and fiddly to terminate correctly.

WHY SORTING BY START (not by end)
  With starts ascending, `currentStart <= lastMerged[1]` is a complete
  overlap test - no need to also check the other direction, because
  currentStart >= lastMerged[0] is guaranteed by the sort. Sorting by END is
  the right call for a DIFFERENT problem: greedy interval scheduling, where
  you keep the most intervals that do not overlap. Same category, opposite
  key, different answer - know which question you are answering.

INVARIANT
  Every interval in `merged` is disjoint from the others, and only the LAST
  one can still grow.

ALGORITHM (NeetCode: "Sorting")
  1. Sort intervals by start ascending.
  2. Seed the output with the first interval.
  3. For each remaining interval:
       - start <= last end  -> overlap: last end = max(last end, this end)
       - otherwise          -> no overlap: append it as a new block
  4. Return the output.

COMPLEXITY
  Time  : O(n log n) - dominated entirely by the sort; the sweep is O(n).
          O(n) is impossible in general, since sorting reduces to this.
  Space : O(n) for the output, plus O(log n) for introsort's recursion.

TRIGGER
  Any input shaped as [start, end] pairs. Sorting by one endpoint is
  essentially always step one. Then ask which endpoint the sweep needs.

C# NOTES
  - Array.Sort(T[], Comparison<T>) takes a lambda directly - no IComparer
    class needed. a[0].CompareTo(b[0]) is safer than a[0] - b[0], which can
    overflow for extreme ints and silently invert the ordering.
  - Array.Sort is NOT STABLE (introsort). It does not matter here because
    equal starts merge anyway - but it is the kind of assumption that bites
    elsewhere. OrderBy in LINQ is stable, at the cost of an allocation.
  - int[][] is a JAGGED array (array of arrays), so its rows are references.
    Adding intervals[0] directly and then writing to it would MUTATE THE
    CALLER'S DATA - which the earlier version did. Copying the row on entry
    is the fix, and the general lesson about reference types in C#.

WATCH OUT
  - Math.Max on the end is required, not cosmetic: [[1,10],[2,3]] must give
    [[1,10]]. Assigning currentEnd blindly shrinks it to [[1,3]].
  - `<=` not `<`: touching intervals [1,4] and [4,5] merge into [1,5] under
    this problem's definition. Always confirm whether touching counts.
  - intervals[0] assumes non-empty input. The constraints guarantee it; a
    production version needs the guard.
================================================================================
*/
