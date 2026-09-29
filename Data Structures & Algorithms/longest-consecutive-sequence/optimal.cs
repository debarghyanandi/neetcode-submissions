// --------------------------------------------------------------------------
// -  optimal.cs            O(n) time / O(n) space
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
 PROBLEM : Given an unsorted int array nums, return the length of the longest
           run of consecutive integers (x, x+1, x+2, ...). The values can be
           in any order in the array, and duplicates count once. Example:
           [100,4,200,1,3,2] -> 4.
 PATTERN : Hash Set + start only at sequence left edge
================================================================================
IDEA
  Put every value in numberSet so each lookup is O(1) and duplicates vanish.
  A number starts a run only if number - 1 is NOT in the set. From each start,
  grow runLength while number + runLength is in the set, and keep the max in
  longestRun. This is correct because every run has exactly one left edge, so
  each run is counted once, and it is counted in full.
EXAMPLE
  nums = [100,4,200,1,3,2,1] -> numberSet {100,4,200,1,3,2} (duplicate 1 gone)
  100: 99 missing -> run 1. 4: 3 present -> skip. 200: run 1.
  1: 0 missing -> 1,2,3,4 present, 5 missing -> run 4. 3 and 2 -> skip.
  Answer: longestRun = 4
COMPLEXITY
  Time  O(n)  the while loop runs only from left edges, so each value is
              walked once
  Space O(n)  numberSet holds up to n distinct values
PATH TO OPTIMAL
  Brute force: for each x, scan the array for x+1, x+2, ... - O(n^3) - no
    extra memory. There is no sibling file for this step.
  Sort, then count adjacent diffs of 1 and skip equal values - O(n log n)
    - avoids the repeated scans.
  Hash set + left-edge check (this file) - O(n) - no sort needed.
KEYWORDS
  hash set, consecutive sequence, left edge, amortized O(n), array, union find
WATCH OUT
  - Without the "number - 1 in set" skip, a run like 1..n is walked from every
    value, which is O(n^2). The skip is the whole trick.
  - Overflow: C# wraps int by default. [2147483647, -2147483648] returns 2,
    not 1, because MaxValue + 1 wraps to MinValue. Use long, or stop at
    MaxValue.
  - Loop over numberSet, not nums. If nums has many copies of a left edge,
    the same run is walked again and again.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. Why is it O(n) when there is a while loop inside the foreach?
     -> The while loop starts only at left edges, and runs never overlap. So
        the total while steps over all starts is at most n. Amortized O(n).
  2. What if you must use O(1) extra memory?
     -> Sort nums in place and scan once. Reset the count when the gap is more
        than 1, and ignore equal neighbours. O(n log n) time, but no hash set.
  3. Return the sequence itself, not just its length.
     -> Also save the start number when runLength beats longestRun. Then
        output start .. start + longestRun - 1. Time and space stay the same.
  4. Numbers arrive as a stream. Can you answer after each insert?
     -> Keep a map from each run's edge to its length (or use union-find). On
        insert, join with the runs at x-1 and x+1. Amortized O(1) per insert.
TRIGGER
  Unsorted input, you want the length of a run of consecutive values, and the
  time limit rules out sorting: use a hash set and start only at the left
  edge.
================================================================================
*/
