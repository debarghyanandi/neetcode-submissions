// ##########################################################################
// #  YOU SOLVED THIS YOURSELF  (submission-0, marked '//My solution')
// #  prefix-sum framing - correct, but carries O(n) memory it does not need
// ##########################################################################

public class Solution
{
    public int MinSubArrayLen(int target, int[] nums)
    {
        // prefix[i] = sum of the first i elements, so the sum of nums[l..r]
        // is prefix[r + 1] - prefix[l] in O(1).
        int[] prefix = new int[nums.Length + 1];

        for (int i = 0; i < nums.Length; i++)
        {
            prefix[i + 1] = prefix[i] + nums[i];
        }

        int left = 0;
        int result = int.MaxValue;

        for (int right = 0; right < nums.Length; right++)
        {
            // Shrink while the window is still big enough to qualify.
            while (prefix[right + 1] - prefix[left] >= target)
            {
                result = Math.Min(result, right - left + 1);
                left++;
            }
        }

        return result == int.MaxValue ? 0 : result;
    }
}

/*
================================================================================
 PATTERN : Sliding Window - Shrinkable, with the window sum read from a
           PREFIX-SUM ARRAY
 SOURCE  : YOUR OWN SOLUTION (submission-0, marked '//My solution')
 STATUS  : Sub-optimal (O(n) time, but O(n) extra space)
================================================================================

WHY THIS PATTERN
  The window's sum is the one thing the algorithm has to ask about on every
  step. Prefix sums answer that question in O(1) for ANY pair (l, r), so the
  window logic gets to stay pure: extend right, shrink while valid, record.

  Reaching for prefix sums is a good instinct and the right one on problems
  where the queried ranges are arbitrary. Here the ranges are not arbitrary -
  they are one window that only ever moves forward - which is exactly the
  condition under which the array is unnecessary. See optimal.cs.

BRUTE FORCE (and why it fails)
  Every (l, r) pair, summing each: O(n^2) sums, O(n^3) naive. Prefix sums
  alone cut that to O(n^2) - still every pair. The window is what makes it
  linear; the prefix array is only how the sum is fetched.

INVARIANT
  On entering the while loop, result holds the shortest qualifying window
  found among all sub-windows ending at or before `right`. `left` never moves
  backward, so each index is admitted once and evicted once.

WHY THIS IS SUB-OPTIMAL
  O(n) auxiliary memory for a quantity that is one int wide. The window's sum
  changes by exactly nums[right] when right advances and by exactly nums[left]
  when left advances - a running total tracks it with no array at all. Same
  O(n) time, O(1) space. That is optimal.cs, and it is the version to give
  in an interview.

  Two smaller costs worth naming: a second full pass to build the array, and
  prefix[i] can overflow int on large inputs where the individual window sums
  never would (all values positive, 10^5 elements at 10^4 each = 10^9, close
  to int.MaxValue).

ALGORITHM
  1. Build prefix[0..n], prefix[0] = 0.
  2. left = 0, result = int.MaxValue.
  3. For each right, while prefix[right+1] - prefix[left] >= target:
       record (right - left + 1), left++.
  4. Return 0 if nothing ever qualified.

COMPLEXITY
  Time  : O(n) - one pass to build, one pass to scan; left advances at most n
          times across the whole run, so the inner while is amortised O(1).
  Space : O(n) - the prefix array.

TRIGGER
  "Shortest / smallest subarray whose sum is at least X", with all values
  non-negative. Non-negativity is what makes the sum monotonic in the window
  size, which is what makes shrinking safe. With negatives this whole family
  collapses and the answer needs a prefix-sum + monotonic deque instead.

C# NOTES
  - int.MaxValue as the "not found" sentinel is idiomatic; the alternative is
    a nullable int, which costs a branch on every comparison.
  - long[] prefix removes the overflow risk for the price of 4 bytes an entry.

WATCH OUT
  - The window is right - left + 1, not right - left.
  - result must be recorded BEFORE left++, or the length is off by one.
  - The inner loop is a while, not an if: several windows ending at `right`
    can qualify, and only the shortest one counts.
================================================================================
*/
