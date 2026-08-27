// --------------------------------------------------------------------------
//  Reference solution - from NeetCode / other resource (submission-0 + submission-1)
//  Not one you solved yourself.
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
 PATTERN : Hash Set + Sequence-Start Detection
 SOURCE  : NeetCode / other resource (submission-0 + submission-1 merged,
           refactored: set built from the constructor)
 STATUS  : Optimal
================================================================================

WHY THIS PATTERN
  The problem says "consecutive", which screams SORT - and the constraint
  says O(n), which forbids it. The resolution: you do not need the values
  ordered, only the ability to ask "does x + 1 exist?" That is membership,
  and membership is a hash set.

BRUTE FORCE (and why it fails)
  Sort, then scan for runs: O(n log n) and genuinely simple - worth stating
  as your first answer, then improving. The stated O(n) requirement is the
  interviewer telling you to find the hashing solution.

  Naive hashing without the start check is worse: walking a run from EVERY
  element re-walks the same run once per member. For [1..n] that is
  1 + 2 + ... + n = O(n^2). The start check is the whole optimisation.

THE INSIGHT THAT MAKES IT LINEAR
  A number begins a run iff number - 1 is absent. Every run therefore has
  exactly ONE starting point, gets walked exactly ONCE, and the total work
  across all runs is the total number of elements. That is why an outer loop
  containing an inner while-loop is still O(n) here - the amortised argument
  matters more than the nesting.

INVARIANT
  Each element is visited by at most one inner while-loop: the one that
  started at its run's left edge.

ALGORITHM (NeetCode: "Hash Set")
  1. Put every value in a HashSet (duplicates collapse automatically).
  2. For each value in the set:
       - if value - 1 is present, skip it, it is not a run start.
       - otherwise walk value, value+1, value+2, ... counting length.
  3. Track the maximum length seen.

COMPLEXITY
  Time  : O(n) amortised - the set build is O(n), the outer loop is O(n)
          membership checks, and all inner while-loops together do O(n) work.
  Space : O(n) for the set.

TRIGGER
  "Longest consecutive / streak / chain" with an EXPLICIT O(n) constraint,
  and the input is unsorted with order irrelevant. The tell is: the problem
  is obviously solvable by sorting, and sorting has been ruled out.

C# NOTES
  - new HashSet<int>(nums) de-duplicates in the constructor - no manual loop.
  - Iterate the SET, not the array. Iterating nums repeats work for duplicate
    values; the set has each value once.
  - Contains on HashSet<int> is O(1) average with no boxing.
  - Mutating a HashSet while foreach-ing it throws InvalidOperationException.
    This code only reads, which is why it is safe.

WATCH OUT
  - Empty input must return 0. longestRun starts at 0 and the loop body never
    executes, so this is handled - do not add a special case.
  - Remove the `continue` guard and the solution is still CORRECT but O(n^2).
    A correct answer that fails the time limit is still a failed submission.
================================================================================
*/
