// --------------------------------------------------------------------------
// -  optimal.cs            O(n log n) time / O(log n) space
// -  sort by start, sweep and merge   [sort-sweep-merge-intervals]
// -  the only solution in this folder
// -
// -  Reference solution - not one you solved yourself
// -
// -  sorts intervals by start so overlap checks reduce to comparing each
// -  interval only against the last merged block in a single pass; space is
// -  the sort's recursion stack since the merged list is the required
// -  output
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
 PATTERN : Sort by start, sweep once, stretch the tail block
 SOURCE  : Reference solution - not one you solved yourself - your own
           annotation at c76939d
 STATUS  : Optimal
================================================================================
WHY THIS PATTERN
  Unordered, "does this interval overlap anything?" is a global question: you
  compare every pair, merge, and restart, because a merge can create new
  overlaps that did not exist before. Sorting by intervals[i][0] makes overlap
  purely local - after the sort, the only interval that can extend the block
  currently sitting at the tail of merged is intervals[i], the very next one.
  The whole problem collapses into one forward scan with no backtracking and no
  re-checking.
INVARIANT
  At the top of each iteration, merged holds the exact union of
  intervals[0..i-1], as disjoint blocks in increasing start order. Every element
  except the last is final; only merged[merged.Count-1] is still open and able
  to grow. Its end, lastMerged[1], is the maximum end among all intervals
  absorbed into it so far - that is precisely why the update is Math.Max and not
  a plain assignment.
WHY THE GAP IS FINAL
  The else branch carries the correctness argument. When currentStart >
  lastMerged[1], nothing at index i or later can ever touch lastMerged: starts
  are non-decreasing after the sort, so every remaining start is >= currentStart
  > lastMerged[1]. The block is closed forever, so appending a new one and never
  looking back is safe. This is also why the loop only ever reads the tail of
  merged - earlier blocks are provably untouchable.
ALIASING
  merged.Add(new int[] { ... }) copies deliberately. Two things ride on it.
  First, lastMerged[1] = Math.Max(...) writes through the reference the list
  holds, which is how the tail grows in place with no removal and re-insertion.
  Second, that reference points at a fresh array rather than at intervals[0], so
  the caller's rows are never rewritten. Change the first Add to
  merged.Add(intervals[0]) and the returned answer stays correct while the input
  array is silently corrupted. Be honest about scope though: Array.Sort still
  reorders the caller's outer array in place - only the inner pairs are
  protected.
WATCH OUT
  1. Math.Max guards containment: [1,10] then [2,3]. With a plain assignment
  lastMerged becomes [1,3], and a following [4,5] then stays separate, so
  [1,10],[2,3],[4,5] returns two blocks instead of one.
  2. currentStart <= lastMerged[1] is inclusive, so [1,4] and [4,5] merge into
  [1,5]. A scheduling variant that treats touching endpoints as non-overlapping
  needs a strict <.
  3. a[0].CompareTo(b[0]) rather than a[0] - b[0]: the subtraction overflows int
  when starts straddle the range, and a broken comparator quietly destroys the
  sorted order the entire argument rests on.
  4. intervals[0] is dereferenced before the loop, so an empty input throws
  IndexOutOfRangeException. Fine under the n >= 1 constraint, not fine as
  library code.
  5. The comparator has no tiebreak on end, and needs none: equal starts always
  satisfy currentStart <= lastMerged[1], so they merge into the same block
  whatever order the sort leaves them in.
TRIGGER AND NEIGHBORS
  Reach for sort-by-start plus a growable tail whenever the ask is the union of
  a collection of ranges. Do not reuse it reflexively on nearby problems: Insert
  Interval hands you an already-sorted list, so drop the sort and stay linear;
  Non-overlapping Intervals (minimum removals) sorts by END instead and counts
  greedily; Meeting Rooms II wants maximum concurrency rather than the union, so
  it needs a min-heap of end times or a +1/-1 sweep over split endpoints.
COMPLEXITY
  Time  : O(n log n)
  Space : O(log n)
================================================================================
*/
