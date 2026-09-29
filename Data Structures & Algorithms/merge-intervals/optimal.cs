// --------------------------------------------------------------------------
// -  optimal.cs            O(n log n) time / O(log n) space
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
 PROBLEM : You get an array of intervals, each [start, end], in any order.
           Merge all intervals that overlap and return the merged list.
           Intervals that only touch, like [1,4] and [4,5], count as
           overlapping. Example: [[1,3],[2,6],[8,10],[15,18]] ->
           [[1,6],[8,10],[15,18]]
 PATTERN : Intervals: sort by start + single linear sweep
================================================================================
IDEA
  Sort intervals by start. Keep the list merged and always compare against
  its last block, lastMerged. If currentStart <= lastMerged[1], they overlap,
  so stretch lastMerged[1] to the larger end. If not, lastMerged is final and
  a new block begins. This is correct because after sorting, any interval
  that can overlap lastMerged must come right after it, so none is missed.
EXAMPLE
  [[2,6],[1,3],[8,10],[9,9],[10,12]] -> sorted
  [1,3],[2,6],[8,10],[9,9],[10,12]
  [1,3]; [2,6] overlaps -> [1,6]; [8,10] gap -> new block; [9,9] contained,
  Max keeps 10; [10,12] touches (10<=10) -> [8,12]. Answer: [[1,6],[8,12]]
COMPLEXITY
  Time  O(n log n)  the sort dominates; the sweep touches each interval once
  Space O(log n)    recursion stack of the in-place sort; output list not
                    counted
PATH TO OPTIMAL
  Brute force: compare every pair and merge until nothing changes - O(n^2)+.
  Sort by start, then one sweep - O(n log n) - no pair checks needed. This
  is optimal.cs, and it is the only file in this folder.
KEYWORDS
  merge intervals, overlapping intervals, sort by start, sweep, greedy
WATCH OUT
  - Use Math.Max for the end, not currentEnd. [1,10] then [2,3] would wrongly
    shrink to [1,3].
  - Use <=, not <. With <, touching [1,4],[4,5] would not merge.
  - Empty input crashes: intervals[0] throws. Return an empty array first.
  - The comment says the caller's array is never mutated, but Array.Sort
    reorders the caller's intervals in place. Only the pairs are copied.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. Insert one new interval into a sorted, non-overlapping list?
     -> No sort needed. Add the blocks that end before it, merge the ones that
        overlap it, then add the rest. O(n) time.
  2. Intervals arrive as a stream?
     -> Keep blocks in a balanced tree keyed by start. Each insert finds its
        neighbors and merges them in O(log n), instead of re-sorting everything.
  3. Minimum intervals to remove so none overlap?
     -> Sort by END and greedily keep the one that ends first. Count the
        skips. O(n log n). Sorting by end is what makes the greedy choice safe.
  4. How many meeting rooms are needed at the same time?
     -> Sort starts and ends apart, sweep with two pointers or use a min-heap
        of end times. O(n log n) time, O(n) space.
TRIGGER
  When a problem gives ranges or [start, end] pairs and asks about overlap,
  sort by start first and sweep once, keeping only the last block.
================================================================================
*/
