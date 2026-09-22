// --------------------------------------------------------------------------
// -  optimal.cs            O(n log n) time / O(log n) space
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
 PATTERN : Sort by start + single sweep - merge overlapping intervals
 SOURCE  : Reference solution - not one you solved yourself - your own
           annotation at c76939d
 STATUS  : Optimal
================================================================================
VARIABLES
  merged        list of finished blocks; merged[last] is the one still open
  lastMerged    reference to the last array inside merged, mutated in place
  currentStart  intervals[i][0], the start of the interval being tested
  currentEnd    intervals[i][1], its end
WHY THIS PATTERN
  The problem asks to combine any intervals that touch or overlap, and overlap
  is not tied to input order. Once the array is sorted by start, every interval
  that could overlap the block currently being built appears immediately after
  it, so one left-to-right pass is enough. The test currentStart <=
  lastMerged[1] is the whole overlap rule, and Math.Max keeps the block's reach
  correct when the new interval ends earlier.
BRUTE FORCE
  Without sorting you repeatedly scan the whole list, merge any two intervals
  that overlap, and restart until one full pass changes nothing. That is correct
  but costs O(n^2) per pass and up to O(n) passes, so O(n^3) in the bad case,
  because merging two intervals can create a new overlap with something far to
  the left. Sorting removes that backtracking: after sorting, a gap at index i
  means nothing later can reach back.
INVARIANT
  After processing index i, merged holds the disjoint merged result of
  intervals[0..i], sorted by start, and only its last element can still grow.
  This holds because the array is start-sorted: currentStart is at least as
  large as every start already seen, so if currentStart > lastMerged[1] it is
  past the end of every earlier block, not just the last one. Therefore closing
  the last block is safe, and the list is final when the loop ends.
MUTATION THROUGH A REFERENCE
  lastMerged is not a copy - it is the same int[] object stored in merged.
  Writing lastMerged[1] = Math.Max(...) updates the list entry directly, with no
  need to write merged[merged.Count - 1] back. This is the one place the code
  depends on int[] being a reference type; if the code used a value type such as
  a (int start, int end) tuple or a struct, the assignment would be lost and the
  merge would silently fail.
WATCH OUT
  intervals[0] is read before the loop with no length check, so an empty input
  throws IndexOutOfRangeException; a null intervals throws in Array.Sort. The
  comment says the caller's array is never mutated, but Array.Sort reorders
  intervals in place, so the caller does see a change - only the inner int[]
  pairs are protected by the copy. The comparator compares only a[0]; equal
  starts keep an arbitrary relative order, which is harmless here because
  Math.Max handles either order. Note also that this merges touching intervals
  like [1,4] and [4,5] into [1,5], since the test uses <= and not <.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. The input is already sorted by start and you must insert one new interval.
  How does the code change?
     Skip the sort and do a single pass: copy blocks that end before the new
     start, merge the overlapping run into the new interval, then copy the rest.
     That is O(n) time and no sort at all.
  2. Can you avoid the extra List and write the answer back into intervals?
     Yes - keep a write index that starts at 0 and, on a gap, advance it and
     store the block, otherwise stretch intervals[write][1]. The result is the
     first write+1 rows, which saves the list but destroys the caller's data.
  3. The input does not fit in memory. What now?
     Sort the intervals externally (sort chunks on disk, then k-way merge them
     by start) and stream the same sweep, emitting each block as soon as a gap
     appears. The sweep itself needs only one open block in memory.
  4. Instead of the merged list, you only need the total length covered by all
  intervals. What changes?
     Keep the same sweep but accumulate (blockEnd - blockStart) each time a
     block closes, plus the final one, and store no arrays at all - O(1) extra
     space beyond the sort.
TRIGGER
  Pairs of (start, end) where the answer depends on which ones touch or overlap,
  and the input order carries no meaning - sort by start and sweep.
C# NOTE
  Array.Sort(intervals, (a, b) => a[0].CompareTo(b[0])) takes a
  Comparison<int[]> delegate; using a[0].CompareTo(b[0]) instead of a[0] - b[0]
  avoids integer overflow when starts are far apart in sign. merged.ToArray()
  copies only the references, so the returned int[][] shares the same inner
  arrays the loop just mutated - fine here because nothing else holds them.
COMPLEXITY
  Time  : O(n log n)
  Space : O(log n)
================================================================================
*/
