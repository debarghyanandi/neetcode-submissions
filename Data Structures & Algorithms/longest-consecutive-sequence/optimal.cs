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
 PATTERN : Hash Set / Sequence Start - count only from left edges
 SOURCE  : Reference solution - not one you solved yourself - your own
           annotation at c76939d
 STATUS  : Optimal
================================================================================
VARIABLES
  numberSet    all distinct values from nums, for fast lookup
  longestRun   longest run of consecutive values found so far
  runLength    length of the run that starts at the current number
WHY THIS PATTERN
  The problem asks for the longest run of consecutive values in any order, in
  linear time. That rules out sorting. It only needs "is x present?" questions,
  and a hash set answers each one in average O(1) time. numberSet answers those
  questions. The check on number - 1 makes sure each run is walked only once,
  from its smallest value.
BRUTE FORCE
  The first correct version most people write is to sort nums, then scan once.
  Skip equal neighbours, and reset the count when a gap is bigger than 1. That
  costs O(n log n) time because of the sort. It loses because the problem asks
  for O(n). A simpler brute force counts upward from every value using the set,
  with no left-edge check. That is O(n^2) on one long run.
INVARIANT
  When a number passes the number - 1 check, it is the smallest value of its
  run. The while loop then walks the full run upward, so runLength is that run's
  true length. Every run has exactly one left edge, so every run is measured
  exactly once. The while loop does a total of at most n steps across all edges.
  longestRun is the maximum over all runs, and that is the answer.
LOOP OVER THE SET, NOT THE ARRAY
  The foreach runs over numberSet, not nums. If it ran over nums and one left
  edge appeared many times, the same run would be walked again for every copy.
  With many duplicates of the start of a long run, that becomes O(n^2). Looping
  over the set means each distinct value is visited once.
WATCH OUT
  The arithmetic can wrap around at the int limits. For the input {int.MinValue,
  int.MaxValue}, number - 1 on int.MinValue wraps to int.MaxValue, so
  int.MinValue is skipped as a start. Then number + runLength on int.MaxValue
  wraps to int.MinValue, and the code returns 2 when the right answer is 1. Use
  long for these two checks if the input can reach the limits. Also, the first
  comment says O(1). That is only the average case for a hash lookup, not a
  guarantee. A null nums throws in the HashSet constructor.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. Return the sequence itself, not just its length?
     Keep a bestStart value and update it together with longestRun. Rebuild the
     run as bestStart .. bestStart + longestRun - 1. This adds O(1) extra space.
  2. Memory is tight. Can you avoid the O(n) set?
     Sort nums in place, then scan it once. Extra space drops to O(1), or the
     sort's stack. Time grows to O(n log n), and the caller's array is changed.
  3. Numbers arrive one at a time, and you must report the longest run after
  each one.
     Use union-find (a structure that groups items into sets and merges them).
     On each new x, union it with x - 1 and x + 1 if they are present, and track
     each group's size. Each step costs near O(1) amortized, but it needs more
     code and more memory.
TRIGGER
  The problem asks for consecutive values or a chain in an unsorted array, in
  O(n), and only needs membership checks.
C# NOTE
  You cannot call numberSet.Remove inside this foreach to skip visited values.
  Changing a HashSet while you enumerate it throws InvalidOperationException. If
  you want removal, loop over nums instead and remove from the set there.
COMPLEXITY
  Time  : O(n)
  Space : O(n)
================================================================================
*/
