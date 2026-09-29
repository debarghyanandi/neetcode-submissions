// --------------------------------------------------------------------------
// -  optimal.cs            O(n log n) time / O(n) space
// -  sort and greedy merge   [sort-and-merge]
// -  the only solution in this folder
// -
// -  Reference solution - not one you solved yourself
// -
// -  Sorting by start point ensures overlaps are adjacent; single pass
// -  merges in-place with Math.Max for contained intervals.
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
 PATTERN : Intervals / Sort by start - merge in one pass
 SOURCE  : Reference solution - not one you solved yourself - your own
           annotation at c76939d
 STATUS  : Optimal
================================================================================
VARIABLES
  merged        the output so far; each entry is one closed block, except the last one
  lastMerged    merged[merged.Count - 1], the block that is still growing
  currentStart  start of intervals[i], checked against lastMerged[1]
  currentEnd    end of intervals[i], used to stretch lastMerged[1]
WHY THIS PATTERN
  The problem asks you to join intervals that overlap, and the input is in no
  order. After Array.Sort by start, any interval that can overlap lastMerged
  must come right after it in the array. So one left-to-right pass can decide
  each interval with one comparison: currentStart <= lastMerged[1].
BRUTE FORCE
  Keep merging pairs until nothing changes. On each round, compare every pair of
  intervals, join any two that overlap, and start again. Each round is O(n^2),
  and you may need up to n rounds, so the worst case is O(n^3). It loses because
  it never uses the order that sorting gives for free.
INVARIANT
  Before step i, merged holds the merge of intervals[0..i-1]. Every block except
  the last is final and does not overlap any later interval. This holds because
  later intervals start at currentStart or later, and when a new block is added,
  currentStart > lastMerged[1] means there is a gap. The last block is the only
  one still open. Math.Max keeps its end right even when intervals[i] sits fully
  inside it, for example [1,10] then [2,3].
TOUCHING INTERVALS MERGE
  The test is <= and not <, so [1,4] and [4,5] become [1,5]. If you change it to
  <, touching intervals stay apart. Ask the interviewer which rule they want
  before you write the comparison.
WATCH OUT
  If intervals is empty, intervals[0] throws IndexOutOfRangeException before the
  loop starts. Add a guard that returns an empty array. The comment "the
  caller's array is never mutated" is only half true. The copy protects the
  inner arrays, but Array.Sort reorders the caller's outer array in place. If
  the caller must keep its original order, sort a copy (for example,
  intervals.ToArray()).
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. The input is already sorted and has no overlaps. How do you insert one new
  interval?
     Skip the sort. Copy the intervals that end before the new one starts. Then
     join every interval that overlaps it into one block, and copy the rest.
     That is O(n) time, but it only works because the input is already clean.
  2. Intervals come in as a stream, and you must answer "current merged set" at
  any time.
     Keep the blocks in a balanced tree keyed by start (SortedDictionary or
     SortedSet in C#). For each new interval, find its neighbors and join them.
     Each insert costs O(log n) plus the blocks it absorbs. You give up the
     simple array for fast updates.
  3. Instead of merging, return the smallest number of rooms needed for all the
  meetings.
     Sort the starts and the ends separately and walk both with two pointers, or
     use a min-heap of end times. The answer is the most intervals open at the
     same moment. You no longer build the merged list.
TRIGGER
  The input is a list of [start, end] ranges, and the task is to combine, count
  or check overlaps. Sort by start, then scan once.
C# NOTE
  The comparer uses a[0].CompareTo(b[0]) and not a[0] - b[0]. Subtraction can
  overflow when the starts have large opposite signs, and then the sort order is
  wrong.
COMPLEXITY
  Time  : O(n log n)
  Space : O(log n)
================================================================================
*/
