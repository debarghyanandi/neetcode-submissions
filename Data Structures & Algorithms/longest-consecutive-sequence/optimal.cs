// --------------------------------------------------------------------------
// -  optimal.cs            O(n) time / O(n) space
// -  Hash set, left-edge counting   [hashset-left-edge]
// -  the only solution in this folder
// -
// -  Reference solution - not one you solved yourself
// -
// -  Each number is visited at most once because iteration only starts from
// -  sequence left edges where n-1 doesn't exist.
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
 PATTERN : Hash Set - expand each run only from its left edge
 SOURCE  : Reference solution - not one you solved yourself - your own
           annotation at c76939d
 STATUS  : Optimal
================================================================================
VARIABLES
  numberSet    all distinct values of nums, for O(1) membership tests
  longestRun   best run length seen so far across all starts
  runLength    length of the run that begins at the current number
WHY THIS PATTERN
  The problem asks for the longest block of consecutive integers, and the input
  order does not matter - only which values are present. That is a membership
  question, so numberSet turns "is number + 1 here?" into a constant-time
  lookup. Once lookups are free, a run can be walked forward one value at a time
  with runLength, and no sorting is needed.
BRUTE FORCE
  The first thing most people write is sort the array, then scan once and count
  consecutive values, skipping equal neighbours. That is correct and costs O(n
  log n) time because of the sort. It loses only on the sort step; the hash set
  replaces the ordering with direct lookups and drops the log factor.
INVARIANT
  The inner while loop is entered only when numberSet does not contain number -
  1, so number is the smallest value of its run. Each run therefore has exactly
  one starting point, and its full length is measured once, from that point.
  Since longestRun keeps the maximum over all starts, and every run has a
  smallest element that will be reached by the foreach, the answer is the length
  of the longest run.
WHY THE GUARD KEEPS IT LINEAR
  Without the `continue` guard the code is still correct but quadratic: a run of
  length m would be walked from each of its m members. With the guard, every
  value is touched by an inner while loop only for the single run it belongs to,
  so the total work of all while loops together is bounded by the size of
  numberSet. The guard is what makes the linear bound real, not just an
  optimisation.
WATCH OUT
  `number + runLength` is unchecked int arithmetic. If int.MaxValue sits at the
  end of a run, the next probe wraps around to a negative value, and if that
  wrapped value happens to be in numberSet the run keeps growing and the answer
  is too large. The comment says an "earlier (or later) iteration will count the
  run" - "earlier" is misleading: HashSet iteration order is not defined, so it
  is simply some other iteration, and you must not rely on the order. An empty
  nums gives an empty numberSet, the foreach body never runs, and 0 is returned,
  which is the right answer.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. Return the actual sequence, not just its length.
     Store the winning start value alongside longestRun when you update it, then
     rebuild start..start+longestRun-1 at the end. Same complexity, one extra
     int.
  2. Can you do it without the extra hash set memory?
     Sort a copy (or nums itself if mutation is allowed) and scan, which is O(1)
     extra space beyond the sort but O(n log n) time. You trade time for space.
  3. The numbers arrive as a stream too large for memory - now what?
     Hold the runs instead of the values: a map from endpoint to run length,
     merging a new value with the run on its left and on its right in O(1) each.
     Memory then scales with the number of distinct runs, not all values.
  4. What if the input can have huge gaps but you want the longest run of values
  differing by at most 2?
     The left-edge test becomes "no value in [number-2, number-1]" and the walk
     probes a small window forward, which is still linear because the window is
     constant size.
TRIGGER
  You need the longest or largest group of values related by +1 or by exact
  equality, and the input order is irrelevant - reach for a hash set and expand
  from group edges only.
C# NOTE
  `new HashSet<int>(nums)` does the dedup in one pass, so the foreach walks
  distinct values only; iterating nums instead would repeat work on duplicates.
  Note that the loop only reads numberSet - a tempting variant that calls Remove
  on visited numbers inside the same foreach would throw
  InvalidOperationException because the collection was modified during
  enumeration.
COMPLEXITY
  Time  : O(n)
  Space : O(n)
================================================================================
*/
