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
 PROBLEM : You get an array numbers sorted in non-decreasing order and an int
           target. Return the 1-based indices [i, j] with i < j where the two
           values add up to target. You may not use the same element twice,
           and there is exactly one answer. Example: [2,7,11,15], target 9 ->
           [1,2].
 PATTERN : Two Pointers (converging from both ends)
================================================================================
IDEA
  Put left at the smallest value and right at the largest value.
  If sum is too big, the only way to make it smaller is right--.
  If sum is too small, the only way to make it bigger is left++.
  This is correct because each move throws away a value that cannot be in
  any pair: numbers[right] is too big even with the smallest partner left.
EXAMPLE
  numbers = [1,2,3,4,6], target = 6
  (0,4): 1+6=7 > 6 -> right=3 | (0,3): 1+4=5 < 6 -> left=1
  (1,3): 2+4=6 == target -> return [2,4] (1-based)
COMPLEXITY
  Time  O(n)  each step moves left or right inward, so at most n-1 steps
  Space O(1)  only left, right and sum are stored
PATH TO OPTIMAL
  Brute force, check all pairs - O(n^2) / O(1) - the baseline.
  Binary search for target - numbers[i] per i - O(n log n) / O(1) - uses sort.
  Hash map of value -> index - O(n) / O(n) - one pass, but needs extra memory.
  Two pointers (this file) - O(n) / O(1) - same speed, no extra memory.
KEYWORDS
  two pointers, sorted array, two sum, pair sum, 1-based indices, greedy
WATCH OUT
  - Return left + 1 and right + 1. Returning 0-based indices is wrong here.
  - Loop must be left < right, not <=. With <= you can pair an element with
    itself, e.g. [3,4] target 6 would wrongly use 3 twice.
  - sum is an int. Two large values can overflow and give a wrong sign;
    use long sum if the values can be near int limits.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. What if the array is not sorted (classic Two Sum)?
     -> Use a hash map of value -> index in one pass: O(n) time, O(n) space.
        Sorting first costs O(n log n) and loses the original indices.
  2. How do you solve 3Sum (all unique triplets summing to 0)?
     -> Sort, fix i, then run this two-pointer loop on the rest: O(n^2) time.
        Skip equal neighbours for i, left and right to avoid duplicate triplets.
  3. What if you must return all pairs, not just one?
     -> On a match, record it, then move both left++ and right--, skipping
        duplicate values. Still O(n) time and O(1) extra space.
  4. Why can you never skip the right answer?
     -> When sum > target, numbers[right] plus any value at or after left is
        at least sum, so it is too big too. The case sum < target is symmetric.
TRIGGER
  A sorted array plus a pair condition on a sum or difference means two
  pointers moving inward from both ends.
================================================================================
*/
