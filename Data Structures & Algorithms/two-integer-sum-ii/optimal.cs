// --------------------------------------------------------------------------
// -  optimal.cs            O(n) time / O(1) space
// --------------------------------------------------------------------------

public class Solution
{
    public int[] TwoSum(int[] numbers, int target)
    {
        int left = 0;
        int right = numbers.Length - 1;

        while (left < right)
        {
            int sum = numbers[left] + numbers[right];

            if (sum == target)
                return new int[] { left + 1, right + 1 };   // problem wants 1-based

            if (sum > target)
                right--;      // shrink from the large end
            else
                left++;       // grow from the small end
        }

        return Array.Empty<int>();
    }
}

/*
================================================================================
 PATTERN : Two Pointers - squeeze a sorted array from both ends
 SOURCE  : Reference solution - not one you solved yourself - your own
           annotation at c76939d
 STATUS  : Optimal
================================================================================
VARIABLES
  left     the index of the smallest value still in play
  right    the index of the largest value still in play
  sum      numbers[left] + numbers[right], the pair being tested
WHY THIS PATTERN
  The input is sorted and we need one pair that adds up to target. Because the
  array is sorted, one comparison of sum with target tells us which end to move.
  If sum is too big, only a smaller value can help, so right moves left. If sum
  is too small, only a bigger value can help, so left moves right. Each step
  removes one element from the search.
BRUTE FORCE
  Try every pair with two nested loops, i < j, and check numbers[i] + numbers[j]
  == target. This is correct but takes O(n^2) time. It ignores the sorted order,
  which is the most useful fact the problem gives you. A binary search for
  target - numbers[i] is better at O(n log n), but it is still slower than the
  linear scan here.
INVARIANT
  If a valid pair exists, both of its indices are inside [left, right]. When sum
  > target, numbers[right] plus any value at or above left is too big, so right
  cannot be in the answer and we drop it. When sum < target, numbers[left] plus
  any value at or below right is too small, so we drop left. We never throw away
  a real answer, so the loop must find it before left meets right.
WATCH OUT
  The line numbers[left] + numbers[right] is plain int addition. If the values
  can be close to int.MaxValue or int.MinValue, it can overflow and pick the
  wrong direction. Cast to long if the limits are unknown. The code quietly
  assumes the array is sorted in ascending order. On unsorted input it gives
  wrong answers and no error. If no pair exists, it returns an empty array and
  not null, so the caller must check Length.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. What if the array is not sorted?
     Use a Dictionary from value to index and look up target - x in one pass.
     This is O(n) time but O(n) space. Sorting first costs O(n log n), and you
     must keep the original indices.
  2. How do you extend this to 3Sum?
     Sort, fix one index i, then run this same two-pointer scan on the part
     after i with target - numbers[i]. That is O(n^2). Skip equal neighbors to
     avoid duplicate triples.
  3. What if you must return all pairs, not just one?
     On a match, record it, then move both pointers. Skip runs of equal values
     so the same pair is not reported twice. It is still one linear pass.
  4. What if you want the pair whose sum is closest to target?
     Use the same moves, but track the smallest |sum - target| you have seen
     instead of returning on equality.
TRIGGER
  A sorted array plus a question about a pair (or pairs) that meets a sum or
  difference condition.
C# NOTE
  Array.Empty<int>() returns one shared, cached empty array, so the no-answer
  path does not allocate a new int[0] on each call.
COMPLEXITY
  Time  : O(n)
  Space : O(1)
================================================================================
*/
