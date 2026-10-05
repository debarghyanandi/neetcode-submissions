// --------------------------------------------------------------------------
// -  optimal.cs            O(n) time / O(1) space
// --------------------------------------------------------------------------

public class Solution
{
    public int[][] Insert(int[][] intervals, int[] newInterval)
    {

        int n = intervals.Length;
        var res = new List<int[]>();
        int i = 0;

        // Left NonOverlapping part
        while (i < n && (intervals[i][1] < newInterval[0]))
        {
            res.Add(new int[] { intervals[i][0], intervals[i][1] });
            i = i + 1;
        }

        // Merge overlapping compartments and store in newInterval
        while (i < n && intervals[i][0] <= newInterval[1])
        {
            newInterval[0] = Math.Min(newInterval[0], intervals[i][0]);
            newInterval[1] = Math.Max(newInterval[1], intervals[i][1]);

            i = i + 1;
        }

        res.Add(newInterval);

        // Right NonOverlapping part
        while (i < n)
        {
            res.Add(new int[] { intervals[i][0], intervals[i][1] });
            i = i + 1;
        }

        return res.ToArray();
    }
}

/*
================================================================================
 PROBLEM : You get a list of intervals that do not overlap and are sorted by
           start, plus one newInterval. Insert it and merge any overlaps, so
           the result is still sorted and has no overlaps. Touching ends count
           as overlap. Example: [[1,3],[6,9]], [2,5] -> [[1,5],[6,9]].
 PATTERN : Intervals: three-phase linear scan (before, merge, after)
================================================================================
IDEA
  One index i walks the list in three phases. Phase 1 copies every interval
  whose end is before newInterval[0]. Phase 2 absorbs every interval whose
  start is <= newInterval[1], growing newInterval with Math.Min and Math.Max.
  Then newInterval is added once, and phase 3 copies the rest. It is correct
  because the input is sorted, so the overlapping intervals form one block.
EXAMPLE
  [[1,2],[3,5],[6,7],[8,10],[12,16]], new [4,8]
  P1: [1,2] (2<4). P2: [3,5]->[3,8], [6,7]->[3,8], [8,10]->[3,10] (8<=8)
  Stop at [12,16] (12>10). Add [3,10]. P3: add [12,16].
  Answer: [[1,2],[3,10],[12,16]]
COMPLEXITY
  Time  O(n)  i only moves forward and visits each interval once
  Space O(1)  only i and the merged newInterval, besides the output list
PATH TO OPTIMAL
  Append newInterval, sort, then merge - O(n log n) - simple, reuses Merge.
  Binary search the insert spot, then merge - still O(n) for the copy.
  Three-phase scan (this file) - O(n) - no sort, one pass, simplest code.
KEYWORDS
  insert interval, merge intervals, sorted intervals, overlap, linear scan
WATCH OUT
  - Phase 1 uses strict <, phase 2 uses <=. Swapping them breaks touching
    ends: [[1,2]] with [2,3] must give [[1,3]], not two intervals.
  - The code changes the caller's newInterval array. Copy it first if the
    caller may reuse it.
  - Do not forget to add newInterval when the list is empty or it goes last;
    here res.Add(newInterval) runs outside all loops, so it is always added.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. What if the intervals are not sorted?
     -> Sort by start first, then merge in one pass. O(n log n) time, O(n)
        space for the sort and output.
  2. Many inserts arrive one by one (streaming)?
     -> Keep intervals in a balanced tree keyed by start (SortedDictionary).
        Each insert finds neighbours in O(log n) plus merged items, amortized.
  3. Can you find the merge block faster?
     -> Binary search the first end >= newStart and last start <= newEnd.
        O(log n) to locate, but building the new array is still O(n).
  4. Why does stopping at the first non-overlap work?
     -> Starts are sorted, so once a start is past newInterval[1] all later
        starts are too; nothing after can overlap.
TRIGGER
  Sorted, non-overlapping intervals plus one new range to add or merge.
================================================================================
*/
