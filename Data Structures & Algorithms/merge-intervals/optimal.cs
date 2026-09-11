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
 PATTERN : Sort by start, sweep once merging into the last block
 SOURCE  : Reference solution - not one you solved yourself - your own
           annotation at c76939d
 STATUS  : Optimal
================================================================================
INVARIANT
  At the top of every iteration, merged holds blocks that are sorted by start,
  pairwise disjoint, and separated by a real gap - for any two adjacent entries
  p and q in merged, p[1] < q[0] strictly. lastMerged is the block with the
  largest end seen so far, and every interval already consumed
  (intervals[0..i-1]) is contained in the union of merged. Both branches restore
  this: stretching lastMerged[1] upward cannot make it touch anything before it,
  because everything before it already ended strictly below lastMerged[0]; and
  the else branch only appends when currentStart > lastMerged[1], which is the
  gap condition itself.
WHY ONLY THE LAST BLOCK NEEDS CHECKING
  This is the correctness argument an interviewer will push on. After sorting by
  start, currentStart is greater than or equal to the start of every interval
  already processed, so it is also greater than or equal to lastMerged[0]. For
  current to overlap some earlier block b in merged, we would need currentStart
  <= b[1]. But b[1] < lastMerged[0] <= currentStart by the invariant.
  Contradiction. So a single comparison against merged[merged.Count - 1] is a
  complete overlap test, not a heuristic - there is no need to scan backwards or
  to re-merge afterwards.
THE TWO DECISIONS IN THE LOOP
  Both are easy to get wrong and both are asked about.

  1. currentStart <= lastMerged[1] uses <=, not <. That makes touching intervals
  merge: [1,4] and [4,5] become [1,5]. If the problem said touching intervals
  stay separate, this one character becomes <.

  2. lastMerged[1] = Math.Max(lastMerged[1], currentEnd), not a plain assignment
  to currentEnd. Sorting orders starts, not ends, so a later interval can be
  fully contained: [1,10] then [2,3] must stay [1,10]. A bare lastMerged[1] =
  currentEnd would shrink it to [1,3] and then wrongly emit anything in (3,10]
  as a separate block.
THE ALIASING DETAIL
  merged.Add(new int[] { intervals[0][0], intervals[0][1] }) and the same
  construction in the else branch copy the pair instead of storing the caller's
  row. This matters because lastMerged[1] = ... writes through the reference
  held by the list. Had the code done merged.Add(intervals[i]), that write would
  land in the caller's int[][], silently rewriting their input while merging.
  The copy is what makes the function non-destructive; note the sort itself
  still reorders the caller's outer array in place, so the input is not left
  untouched either way.
WATCH OUT
  Empty input crashes. merged.Add(new int[] { intervals[0][0], ... }) runs
  unguarded, so intervals.Length == 0 throws IndexOutOfRangeException. The judge
  guarantees at least one interval; production code needs an early return.

  The comparator is a[0].CompareTo(b[0]), not a[0] - b[0]. Subtraction overflows
  when the starts straddle the int range (for example int.MinValue and a
  positive start), producing a comparator that violates transitivity and an
  arbitrarily wrong order. CompareTo has no such failure mode.

  Array.Sort is not stable, so equal starts can come out in either order - and
  it does not matter. If two intervals share a start, whichever lands first
  becomes lastMerged with lastMerged[0] == currentStart, and since a well-formed
  interval has start <= end, currentStart <= lastMerged[1] holds and they merge.
  This relies on inputs being well-formed; a reversed pair like [5,2] breaks the
  invariant.
TRIGGER
  Reach for this shape when the input is a set of ranges and the answer depends
  on how they overlap, with no requirement to preserve input order: merge
  overlapping intervals, compute total covered length, count connected blocks.
  The tell is that a brute-force pairwise overlap check is quadratic and keeps
  needing re-merges after each combine, while one ordering decision up front
  collapses it into a linear scan with a single live block of state.
FOLLOW-UPS
  Insert Interval: the list arrives already sorted, so drop the sort and walk
  three phases - blocks ending before the new start, blocks overlapping it
  (absorb with min of starts and max of ends), blocks starting after the new
  end.

  Non-overlapping Intervals / Minimum Arrows: sort by END and greedily keep the
  earliest-ending interval. Worth knowing why the key flips - there you want to
  maximize how many fit, so the smallest end leaves the most room; here you want
  every overlap detected, which needs the start ordering.

  Sorting by end breaks this code specifically: [1,10] and [2,3] sort to [2,3]
  then [1,10], and the last-block-only argument above no longer holds because
  currentStart can be smaller than lastMerged[0].

  Meeting Rooms II asks for maximum concurrent overlap rather than the merged
  union - that wants a min-heap of end times, or separately sorted start and end
  arrays swept with two pointers.
COMPLEXITY
  Time  : O(n log n)
  Space : O(log n)
================================================================================
*/
