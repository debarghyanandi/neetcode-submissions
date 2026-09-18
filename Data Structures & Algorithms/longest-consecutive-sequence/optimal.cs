// --------------------------------------------------------------------------
// -  optimal.cs            O(n) time / O(n) space
// -  HashSet with left-edge iteration   [hashset-left-edge]
// -  the only solution in this folder
// -
// -  Reference solution - not one you solved yourself
// -
// -  Each number visited at most once across all while loops; HashSet
// -  enables O(1) membership checks at run boundaries.
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
 PATTERN : Hash Set - expand only from each run's left edge
 SOURCE  : Reference solution - not one you solved yourself - your own
           annotation at c76939d
 STATUS  : Optimal
================================================================================
VARIABLES
  numberSet    all values from nums, deduplicated, for O(1) membership tests
  longestRun   best run length seen so far across all start points
  runLength    how far the run starting at number has reached; number + runLength is the next value to test
WHY THIS PATTERN
  The problem asks for the longest block of consecutive integers, and the order
  they appear in nums does not matter. That means we only need to answer "is
  value v present?", which is exactly what a hash set gives in constant time.
  Once numberSet exists, a run can be walked forward one value at a time with
  numberSet.Contains(number + runLength), and the guard on number - 1 makes sure
  each run is walked from one place only.
BRUTE FORCE
  The natural first attempt is to sort nums, then sweep once counting
  consecutive steps and resetting on a gap, skipping equal neighbours. That is
  correct and only O(n log n) time with O(1) extra space, but the sort
  dominates. A worse attempt is, for each value, to scan the whole array looking
  for value + 1 repeatedly, which is O(n^2).
INVARIANT
  At the start of each foreach body, longestRun holds the longest run among all
  runs whose left edge has already been visited. The continue guarantees the
  body only runs when number - 1 is absent, so number is the smallest member of
  its run, and the while loop then counts that run completely. Every run has
  exactly one left edge, so every run is measured exactly once and longestRun
  ends up as the maximum over all of them.
WHY THE TOTAL WORK IS LINEAR
  The while loop looks nested, but it only executes for values that start a run,
  and it walks each run's members once. Summing the runs gives at most one visit
  per distinct value, so the inner loop does bounded total work across the whole
  foreach. Dropping the number - 1 check would keep the answer correct but turn
  a single long run into repeated full walks.
WATCH OUT
  Iterating numberSet rather than nums matters: with nums full of duplicates of
  the same left edge, a loop over nums would walk that run once per copy.
  Arithmetic here is unchecked, so int.MinValue - 1 wraps to int.MaxValue; if
  both extremes are in nums, the MinValue element is wrongly treated as mid-run
  and its run is never started. The same wrap can hit number + runLength near
  int.MaxValue. Empty nums is safe: the foreach body never runs and longestRun
  stays 0.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. Return the actual sequence, not just its length.
     Record bestStart = number whenever runLength > longestRun, then emit
     bestStart .. bestStart + longestRun - 1. No change to the complexity, one
     extra variable.
  2. Memory is tight and nums can be modified in place.
     Sort nums and sweep, comparing nums[i] to nums[i-1] and skipping equal
     values. O(1) extra space at the cost of O(n log n) time.
  3. Values arrive as a stream and you must report the longest run after each
  insert.
     Keep a dictionary from value to the length of the run it belongs to, and on
     insert merge with the runs at value - 1 and value + 1 by updating only the
     two endpoints. Each insert is O(1) amortized; the current code would have
     to rerun from scratch.
  4. The data does not fit on one machine.
     Partition by value range, compute runs per partition, then stitch runs that
     touch a partition boundary. The per-partition work is the same as here; the
     join is the new cost.
TRIGGER
  The answer depends on which values exist, not on their order or index, and you
  find yourself wanting to ask "is v+1 there?".
C# NOTE
  new HashSet<int>(nums) uses the default EqualityComparer<int>, so Contains
  needs no boxing and no custom comparer. The manual if (runLength > longestRun)
  can be written as longestRun = Math.Max(longestRun, runLength) for the same
  result in one line.
COMPLEXITY
  Time  : O(n)
  Space : O(n)
================================================================================
*/
