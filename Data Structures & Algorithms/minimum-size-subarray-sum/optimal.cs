// ##########################################################################
// #  optimal.cs            O(n) time / O(1) space
// #  sliding window, shrink while valid   [two-pointer-shrink]
// #  ranks above suboptimal.cs (O(n) time / O(n) space)
// #
// #  YOU SOLVED THIS YOURSELF
// #
// #  Each element is added and removed from the sum at most once as the two
// #  pointers traverse the array.
// ##########################################################################

public class Solution
{
    public int MinSubArrayLen(int target, int[] nums)
    {
        int sum = 0;
        int left = 0;
        int result = int.MaxValue;

        for (int right = 0; right < nums.Length; right++)
        {
            sum = sum + nums[right];   // admit the new right-hand element

            // Shrink while the window still qualifies - the first failure
            // means every shorter window ending here fails too.
            while (sum >= target)
            {
                result = Math.Min(result, right - left + 1);
                sum = sum - nums[left];  // evict, THEN move the edge
                left++;
            }
        }

        return result == int.MaxValue ? 0 : result;
    }
}

/*
================================================================================
 PATTERN : Sliding Window (variable size) - shrink while valid
 SOURCE  : YOUR OWN SOLUTION - your own annotation at c76939d
 STATUS  : Optimal
================================================================================
VARIABLES
  sum      sum of nums[left..right], the current window
  left     index of the first element still in the window
  result   shortest valid window length seen so far; int.MaxValue means none found yet
WHY THIS PATTERN
  The problem asks for the shortest contiguous subarray whose sum reaches
  target. "Contiguous" plus "shortest or longest" is the usual sign of a sliding
  window. The values are positive, so adding nums[right] can only raise sum and
  removing nums[left] can only lower it. That means both edges only ever move
  forward, and each index enters and leaves the window at most once.
BRUTE FORCE
  For every start index, extend an end index and keep a running total. Stop at
  the first end where the total reaches target, and record the length. This is
  O(n^2) time and O(1) space, and it is correct. It loses because it adds up the
  same elements again for every start, while the window reuses the sum it
  already has.
INVARIANT
  After the inner while loop ends, sum < target. So no window that ends at right
  and starts at left or later reaches target. Any window that starts before left
  and ends at right was already recorded in result when that start index was
  removed, or it is longer than one that was recorded. So when right reaches the
  end, every window that could be shortest has been measured.
RECORD BEFORE YOU SHRINK
  The line result = Math.Min(result, right - left + 1) runs inside the while
  loop, before the eviction. At that point the window is still valid, so each
  valid length gets recorded before the window gets shorter. The order "sum -=
  nums[left], then left++" matters. If you swap the two lines, you subtract the
  wrong element.
WATCH OUT
  The comment "the first failure means every shorter window ending here fails
  too" is only true when every value is positive. If there are negative numbers,
  a shorter window can have a larger sum, and this code gives a wrong answer. If
  target <= 0, the while loop never stops on its own: left moves past right and
  nums[left] finally throws IndexOutOfRangeException. The final check result ==
  int.MaxValue ? 0 : result is what returns 0 when no window qualifies. If you
  remove it, the code returns int.MaxValue.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. Can you do it in O(n log n) another way?
     Build a prefix sum array. For each start i, binary search for the first j
     where prefix[j] - prefix[i] >= target. This is slower and uses O(n) extra
     memory. It still depends on the values being positive, because that keeps
     the prefix array sorted.
  2. What if nums can contain negative numbers?
     Use prefix sums with a monotonic deque. A deque is a list you can add to or
     remove from at both ends. Keep start indices in it with increasing prefix
     values. Remove from the front while prefix[j] - prefix[front] >= target,
     and remove from the back any index whose prefix value is not smaller than
     prefix[j]. This is O(n) time but needs O(n) space.
  3. What if you must return the subarray itself, not its length?
     Keep a bestLeft variable and set it to left each time result gets smaller.
     Return nums[bestLeft .. bestLeft + result - 1]. The extra cost is one
     variable.
TRIGGER
  Look for this pattern when you need the shortest or longest contiguous
  subarray that meets a sum threshold and all values are non-negative, so the
  sum only grows as the window grows.
C# NOTE
  By default C# integer math is unchecked. If sum = sum + nums[right] overflows,
  it wraps around to a negative number without any error and breaks the while
  condition. If the totals can be large, declare sum as long, or wrap the
  addition in checked so it throws instead.
COMPLEXITY
  Time  : O(n)
  Space : O(1)
================================================================================
*/
