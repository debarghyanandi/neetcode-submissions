// --------------------------------------------------------------------------
// -  optimal.cs            O(n) time / O(n) space
// -  HashSet with left-edge optimization   [hashset-left-edge]
// -  the only solution in this folder
// -
// -  Reference solution - not one you solved yourself
// -
// -  Each number is visited at most once; counting starts only from
// -  sequence left edges
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
  numberSet   all values from nums, deduplicated, for O(1) membership tests
  longestRun  best run length seen so far
  runLength   how far the current run reaches from its starting number
WHY THIS PATTERN
  The problem asks for the longest block of consecutive integers, and the order
  in the input does not matter. That means the only question we ever ask is
  "does value x exist?", which a hash set answers in constant time. So numberSet
  turns the whole problem into repeated membership tests, and the left-edge rule
  keeps the total number of those tests linear.
BRUTE FORCE
  The natural first attempt is to sort nums, then walk it once, resetting a
  counter whenever the gap to the previous value is not 1 and skipping equal
  values. That is correct and uses O(1) extra space, but sorting costs O(n log
  n). The hash set trades that log factor for extra memory.
INVARIANT
  Every number the outer loop actually expands from satisfies "number - 1 is not
  in numberSet", so it is the smallest element of its run. Each run therefore
  has exactly one starting point, and the inner while walks that run end to end
  exactly once. Summed over all runs, the inner loop does at most one step per
  distinct value, which is why the whole scan stays linear despite the nested
  loop.
WHY ITERATE THE SET, NOT THE ARRAY
  The foreach runs over numberSet, not nums. If it ran over nums instead, a
  duplicated left edge such as [1,2,3,1,2,3] would re-walk the same run once per
  copy, so the work becomes O(n * run length). Iterating the deduplicated set
  guarantees each starting number is expanded at most once.
WATCH OUT
  The expression number + runLength can overflow. If numberSet holds
  int.MaxValue, the check after it wraps to int.MinValue, and if int.MinValue
  happens to be in the set the run keeps growing and the answer is wrong. An
  empty nums is fine: the foreach never runs and longestRun stays 0. Do not add
  or remove items from numberSet inside the foreach - modifying a HashSet while
  enumerating it throws InvalidOperationException.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. Return the actual sequence, not just its length.
     Remember the starting number alongside longestRun whenever runLength beats
     it, then rebuild start..start+longestRun-1 at the end. Cost is unchanged.
  2. Memory is tight - no hash set allowed.
     Sort nums in place and do a single linear pass with a counter, skipping
     equal neighbours. O(1) extra space, but O(n log n) time.
  3. Numbers arrive one at a time in a stream and you must report the best run
  after each insert.
     Keep a dictionary from value to the length of the run it borders, or use
     union-find. On insert, merge with the run ending at value-1 and the one
     starting at value+1 and update the boundary entries. Each insert is
     amortized near constant, but the bookkeeping is much easier to get wrong
     than this code.
  4. What if a gap of at most one missing number is still allowed inside a run?
     The left-edge trick breaks, because a start is no longer defined by
     "value-1 missing". Sort the distinct values and use a sliding window over
     the sorted list, tracking how many gaps the window has consumed.
TRIGGER
  Order does not matter and you only ever need to ask "does value x exist" -
  reach for a hash set and expand from one canonical endpoint per group.
C# NOTE
  new HashSet<int>(nums) does the copy and the deduplication in one constructor
  call, and HashSet<int>.Contains uses the default int comparer with no boxing,
  so there is no reason to build a Dictionary or call
  nums.Distinct().ToHashSet() here.
COMPLEXITY
  Time  : O(n)
  Space : O(n)
================================================================================
*/
